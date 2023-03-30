using Microsoft.AspNetCore.Session;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Backend.Logic
{
    public interface IPasteImgPublicLogic
    {
        string CreateSession();
        void EnterPassword(string uploadId, string password, string? sessionKey);
        Image GetImage(string id, string? sessionKey);
        Image GetImageWithSourceFile(string id, string? sessionKey);
        Image GetImageWithThumbnailFile(string id, string? sessionKey);
        Upload GetUpload(string id, string? sessionKey);
        Upload GetUploadWithSourceFiles(string id, string? sessionKey);
        Upload GetUploadWithThumbnailFiles(string id, string? sessionKey);
        ValidationConfiguration GetValidationConfiguration();
        string PostUpload(Upload upload, string? sessionKey);
    }

    public class PasteImgPublicLogic : IPasteImgPublicLogic
    {
        private const string DateTimeFormat = @"MM/dd/yyyy HH:mm";
        private IPasteImgLogic logic;
        private ISessionHandler sessionHandler;
        public PasteImgPublicLogic(IPasteImgLogic logic, ISessionHandler sessionHandler)
        {
            this.logic = logic;
            this.sessionHandler = sessionHandler;
        }

        public string CreateSession()
        {
            return sessionHandler.CreateNewSession();
        }

        /// <inheritdoc/>
        /// <exception cref="LockoutException"></exception>
        /// <exception cref="WrongPasswordException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public void EnterPassword(string uploadId, string password, string? sessionKey)
        {
            ISession? session = sessionHandler.GetSession(sessionKey);
            if (session is null)
            {
                return;
            }

            Upload upload = logic.GetUpload(uploadId);
            if (upload.Password is null)
            {
                return;
            }

            if (IsLockedOut(uploadId, session))
            {
                throw new LockoutException() { UploadId = uploadId };
            }

            password = logic.CreateHash(password);
            if (upload.Password == password)
            {

                session.SetString(uploadId, password);
                session.Remove(GetSessionAttemptsKey(uploadId));
                session.CommitAsync().Wait();
            }
            else
            {
                char sep = ';';
                string? attemptsString = session.GetString(GetSessionAttemptsKey(uploadId));
                DateTime now = DateTime.Now;
                string formattedNow = now.ToString(DateTimeFormat, CultureInfo.CurrentCulture);
                int lockoutTime = logic.Configuration.Visitor.LockoutTresholdInMinutes;
                int maxAttempt = logic.Configuration.Visitor.MaxFailedAttempt;

                if (attemptsString is null)
                {
                    session.SetString(GetSessionAttemptsKey(uploadId), formattedNow);
                }
                else
                {
                    DateTime lastTime = DateTime.ParseExact(attemptsString.Split(sep)[^1], DateTimeFormat, CultureInfo.CurrentCulture);
                    if ((now - lastTime).TotalMinutes > lockoutTime)
                    {
                        session.SetString(GetSessionAttemptsKey(uploadId), formattedNow);
                    }
                    else
                    {
                        session.SetString(GetSessionAttemptsKey(uploadId), attemptsString + sep + formattedNow);
                    }
                }

                int remaining = maxAttempt - session.GetString(GetSessionAttemptsKey(uploadId)).Split(sep).Length;
                session.CommitAsync().Wait();
                throw new WrongPasswordException(remaining) { UploadId = uploadId };
            }
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImage(string id, string? sessionKey)
        {
            return GetImageAndCheckPassword(id, logic.GetImage, sessionKey);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImageWithSourceFile(string id, string? sessionKey)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithSourceFile, sessionKey);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImageWithThumbnailFile(string id, string? sessionKey)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithThumbnailFile, sessionKey);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUpload(string id, string? sessionKey)
        {
            return GetUploadAndCheckPassword(id, logic.GetUpload, sessionKey);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUploadWithSourceFiles(string id, string? sessionKey)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithSourceFiles, sessionKey);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUploadWithThumbnailFiles(string id, string? sessionKey)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithThumbnailFiles, sessionKey);
        }

        /// <inheritdoc/>
        public ValidationConfiguration GetValidationConfiguration()
        {
            return logic.Configuration.Validation;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidEntityException"></exception>
        /// <exception cref="SomethingWrongException"></exception>
        public string PostUpload(Upload upload, string? sessionKey)
        {
            try
            {
                ISession? session = sessionHandler.GetSession(sessionKey);
                string uploadId = logic.PostUpload(upload);

                if (session is not null && upload.Password is not null)
                {
                    session.SetString(upload.Id, upload.Password);
                }
                session.CommitAsync().Wait();
                return uploadId;
            }
            catch (PasteImgException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SomethingWrongException(ex, ex.Message);
            }

        }


        /// <summary>
        /// Retrieves an image and checks whether a password is required for it. If a password is required and the session does
        /// not contain it, then a PasswordRequiredException is thrown.
        /// </summary>
        /// /// <param name="id">The ID of the image to retrieve.</param>
        /// <param name="getUpload">A function that retrieves an image by its ID.</param>
        /// <param name="session">The session containing the password, if any.</param>
        /// <returns>The retrieved image.</returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        private Image GetImageAndCheckPassword(string id, Func<string, Image> getImage, string? sessionKey)
        {
            Image image = getImage(id);
            ISession? session = sessionHandler.GetSession(sessionKey);
            if (image.Upload.Password is not null && 
                (session is null || image.Upload.Password != GetSessionPassword(image.Upload.Id, session)))
            {
                throw new PasswordRequiredException() { ImageId = id, UploadId = image.UploadId };
            }
            return image;
        }

        /// <summary>
        /// Get the session key for the attempts string for a given upload ID.
        /// </summary>
        /// <param name="uploadID">The ID of the upload.</param>
        /// <returns>The session key for the attempts string.</returns>
        private string GetSessionAttemptsKey(string uploadID)
        {
            return uploadID + "_attempts";
        }

        /// <summary>
        /// Retrieves the password associated with the specified upload ID from the session.
        /// </summary>
        /// <param name="uploadID">The upload ID associated with the password to retrieve.</param>
        /// <param name="session">The session object to retrieve the password from.</param>
        /// <returns>The password associated with the upload ID, or null if it does not exist.</returns>
        private string? GetSessionPassword(string uploadID, ISession? session)
        {
            return session?.GetString(uploadID);
        }

        /// <summary>
        /// Retrieves an upload and checks whether a password is required for it. If a password is required and the session does
        /// not contain it, then a PasswordRequiredException is thrown.
        /// </summary>
        /// /// <param name="id">The ID of the upload to retrieve.</param>
        /// <param name="getUpload">A function that retrieves an upload by its ID.</param>
        /// <param name="session">The session containing the password, if any.</param>
        /// <returns>The retrieved upload.</returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        private Upload GetUploadAndCheckPassword(string id, Func<string, Upload> getUpload, string? sessionKey)
        {
            ISession? session = sessionHandler.GetSession(sessionKey);
            Upload upload = getUpload(id);
            if (upload.Password is not null &&
                (session is null||upload.Password != GetSessionPassword(upload.Id, session)))
            {
                throw new PasswordRequiredException() { UploadId = id };
            }
            return upload;
        }

        /// <summary>
        /// Determines whether the specified upload ID is locked out due to too many failed attempts for current session.
        /// </summary>
        /// <param name="uploadID">The upload ID to check for lockout status.</param>
        /// <param name="session">The current session that containing lockout status information.</param>
        /// <returns>True if the upload ID is locked out for current session, false otherwise.</returns>
        private bool IsLockedOut(string uploadID, ISession? session)
        {
            if (session is null)
            {
                return false;
            }
            else
            {
                return session.GetString(GetSessionAttemptsKey(uploadID)) is string attempts &&
                attempts.Split(";").Length >= logic.Configuration.Visitor.MaxFailedAttempt;

            }
        }
    }
}