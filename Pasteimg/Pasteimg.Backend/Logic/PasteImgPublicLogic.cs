using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;
using System.Globalization;
using System.Text;

namespace Pasteimg.Backend.Logic
{
    /// <summary>
    /// Interface for managing the public logic of PasteImg.
    /// </summary>
    public interface IPasteImgPublicLogic
    {
        /// <summary>
        /// Enters a password for an upload into the session, and tracks failed attempts.
        /// </summary>
        /// <param name="uploadId">The ID of the upload to enter the password for.</param>
        /// <param name="password">The password to enter.</param>
        /// <param name="session">The session to store the password in.</param>
        void EnterPassword(string uploadId, string password, ISession session);

        /// <summary>
        /// Retrieves an image with the given identifier without file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The image with the given identifier and if the password (if set) is entered correctly by the user.</returns>
        Image GetImage(string id, ISession session);

        /// <summary>
        /// Retrieves an image with the given identifier  with the attached source file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The image with the given identifier, with attached source file and if the password (if set) is entered correctly by the user.</returns>
        Image GetImageWithSourceFile(string id, ISession session);

        /// <summary>
        /// Retrieves an image with the given identifier with the attached thumbnail file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The image with the given identifier, with attached thumbnail file and if the password (if set) is entered correctly by the user.</returns>
        Image GetImageWithThumbnailFile(string id, ISession session);

        /// <summary>
        /// Retrieves the upload with the given identifier without associated files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The upload with the given identifier and if the password (if set) is entered correctly by the user.</returns>
        Upload GetUpload(string id, ISession session);

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated source files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The upload with the given identifier and all associated source files, if the password (if set) is entered correctly by the user.</returns>
        Upload GetUploadWithSourceFiles(string id, ISession session);

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated thumbnail files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <param name="session">The session associated with the current user.</param>
        /// <returns>The upload with the given identifier and all associated thumbnail files, if the password (if set) is entered correctly by the user.</returns>
        Upload GetUploadWithThumbnailFiles(string id, ISession session);

        /// <summary>
        /// Gets the validation configuration.
        /// </summary>
        ValidationConfiguration GetValidationConfiguration();

        /// <summary>
        /// Stores the uploaded images, if modelstate is valid.
        /// </summary>
        /// <param name="upload">The object containing the images to be uploaded.</param>
        /// <param name="session">The session associated with the current user.</param>
        void PostUpload(Upload upload, ISession session);

        /// <summary>
        /// Sets whether NSFW content should be shown for the current session.
        /// </summary>
        /// <param name="value">True if NSFW content should be shown, false otherwise.</param>
        /// <param name="session">The session associated with the current user.</param>
        void SetShowNsfw(bool value, ISession session);
    }

    public class PasteImgPublicLogic : IPasteImgPublicLogic
    {
        private const string DateTimeFormat = @"MM/dd/yyyy HH:mm";
        private IPasteImgLogic logic;

        public PasteImgPublicLogic(IPasteImgLogic logic)
        {
            this.logic = logic;
        }

        /// <inheritdoc/>
        /// <exception cref="LockoutException"></exception>
        /// <exception cref="WrongPasswordException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public void EnterPassword(string uploadID, string password, ISession session)
        {
            Upload upload = logic.GetUpload(uploadID);
            if (upload.Password is null)
            {
                return;
            }
            if (IsLockedOut(uploadID, session))
            {
                throw new LockoutException(uploadID);
            }

            password = logic.CreateHash(password);
            if (upload.Password == password)
            {
                session.SetString(uploadID, password);
                session.Remove(GetSessionAttemptsKey(uploadID));
            }
            else
            {
                char sep = ';';
                string? attemptsString = session.GetString(GetSessionAttemptsKey(uploadID));
                DateTime now = DateTime.Now;
                string formattedNow = now.ToString(DateTimeFormat, CultureInfo.CurrentCulture);
                int lockoutTime = logic.Configuration.Visitor.LockoutTresholdInMinutes;
                int maxAttempt = logic.Configuration.Visitor.MaxFailedAttempt;

                if (attemptsString is null)
                {
                    session.SetString(GetSessionAttemptsKey(uploadID), formattedNow);
                }
                else
                {
                    DateTime lastTime = DateTime.ParseExact(attemptsString.Split(sep)[^1], DateTimeFormat, CultureInfo.CurrentCulture);
                    if ((now - lastTime).TotalMinutes > lockoutTime)
                    {
                        session.SetString(GetSessionAttemptsKey(uploadID), formattedNow);
                    }
                    else
                    {
                        session.SetString(GetSessionAttemptsKey(uploadID), attemptsString + sep + formattedNow);
                    }
                }

                int remaining = maxAttempt - session.GetString(GetSessionAttemptsKey(uploadID)).Split(sep).Length;
                throw new WrongPasswordException(uploadID, remaining);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImage(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImage, session);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImageWithSourceFile(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithSourceFile, session);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Image GetImageWithThumbnailFile(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithThumbnailFile, session);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUpload(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUpload, session);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUploadWithSourceFiles(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithSourceFiles, session);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="PasswordRequiredException"></exception>
        public Upload GetUploadWithThumbnailFiles(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithThumbnailFiles, session);
        }

        /// <inheritdoc/>
        public ValidationConfiguration GetValidationConfiguration()
        {
            return logic.Configuration.Validation;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidEntityException"></exception>
        /// <exception cref="SomethingWrongException"></exception>
        public void PostUpload(Upload upload, ISession session)
        {
            logic.PostUpload(upload);
            if (upload.Password != null)
            {
                session.SetString(upload.Id, upload.Password);
            }
        }

        /// <inheritdoc/>
        public void SetShowNsfw(bool value, ISession session)
        {
            if (value)
            {
                session.SetString("nsfw", "");
            }
            else
            {
                session.Remove("nsfw");
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
        private Image GetImageAndCheckPassword(string id, Func<string, Image> getImage, ISession session)
        {
            Image image = getImage(id);
            if (image.Upload.Password is not null && image.Upload.Password != GetSessionPassword(image.Upload.Id, session))
            {
                throw new PasswordRequiredException(typeof(Image), image.Upload.Id, image.Id);
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
        private string? GetSessionPassword(string uploadID, ISession session)
        {
            return session.GetString(uploadID);
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
        private Upload GetUploadAndCheckPassword(string id, Func<string, Upload> getUpload, ISession session)
        {
            Upload upload = getUpload(id);
            if (upload.Password is not null && upload.Password != GetSessionPassword(upload.Id, session))
            {
                throw new PasswordRequiredException(typeof(Upload), upload.Id, null);
            }
            return upload;
        }

        /// <summary>
        /// Determines whether the specified upload ID is locked out due to too many failed attempts for current session.
        /// </summary>
        /// <param name="uploadID">The upload ID to check for lockout status.</param>
        /// <param name="session">The current session that containing lockout status information.</param>
        /// <returns>True if the upload ID is locked out for current session, false otherwise.</returns>
        private bool IsLockedOut(string uploadID, ISession session)
        {
            return session.GetString(GetSessionAttemptsKey(uploadID)) is string attempts &&
                attempts.Split(";").Length >= logic.Configuration.Visitor.MaxFailedAttempt;
        }
    }
}