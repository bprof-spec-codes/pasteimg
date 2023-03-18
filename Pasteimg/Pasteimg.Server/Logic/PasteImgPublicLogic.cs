using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Configurations;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Models.Error;
using System.Globalization;
using System.Text;

namespace Pasteimg.Server.Logic
{
    public interface IPasteImgPublicLogic
    {
        Image GetImage(string id, ISession session);

        Image GetImageWithSourceFile(string id, ISession session);

        Image GetImageWithThumbnailFile(string id, ISession session);

        Upload GetUpload(string id, ISession session);

        Upload GetUploadWithSourceFiles(string id, ISession session);

        Upload GetUploadWithThumbnailFiles(string id, ISession session);

        ValidationConfiguration GetValidationConfiguration();

        void PostUpload(Upload upload, ISession session);

        void SetPassword(string uploadId, string password, ISession session);

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

        public Image GetImage(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImage, session);
        }

        public Image GetImageWithSourceFile(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithSourceFile, session);
        }

        public Image GetImageWithThumbnailFile(string id, ISession session)
        {
            return GetImageAndCheckPassword(id, logic.GetImageWithThumbnailFile, session);
        }

        public Upload GetUpload(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUpload, session);
        }

        public Upload GetUploadWithSourceFiles(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithSourceFiles, session);
        }

        public Upload GetUploadWithThumbnailFiles(string id, ISession session)
        {
            return GetUploadAndCheckPassword(id, logic.GetUploadWithThumbnailFiles, session);
        }

        public ValidationConfiguration GetValidationConfiguration()
        {
            return logic.GetValidationConfiguration();
        }


        public void PostUpload(Upload upload, ISession session)
        {
            logic.PostUpload(upload);
            if (upload.Password != null)
            {
                session.SetString(upload.Id, upload.Password);
            }
        }

        private bool IsLockedOut(string uploadId, ISession session)
        {
            return session.GetString(GetSessionAttemptsKey(uploadId)) is string attempts && 
                attempts.Split(";").Length>=logic.Configuration.Visitor.MaxFailedAttempt;
        }

        [HttpPost]
        public void SetPassword(string uploadId,string password,ISession session)
        {
           
            Upload upload = logic.GetUpload(uploadId);
            if(upload.Password is null)
            {
                return;
            }
            if(IsLockedOut(uploadId,session))
            {
                throw new LockoutException(uploadId);
            }

            password = logic.CreateHash(password);
            if(upload.Password==password)
            {
                session.SetString(uploadId, password);
                session.Remove(GetSessionAttemptsKey(uploadId));
            }
            else
            {
                char sep = ';';
                string? attemptsString = session.GetString(GetSessionAttemptsKey(uploadId));
                DateTime now = DateTime.Now;
                string formattedNow = now.ToString(DateTimeFormat, CultureInfo.CurrentCulture);
                int lockoutTime = logic.Configuration.Visitor.LockoutTresholdInMinutes;
                int maxAttempt = logic.Configuration.Visitor.MaxFailedAttempt;

                if(attemptsString is null)
                {
                    session.SetString(GetSessionAttemptsKey(uploadId), formattedNow);
                }
                else
                {
                    DateTime lastTime = DateTime.ParseExact(attemptsString.Split(sep)[^1], DateTimeFormat, CultureInfo.CurrentCulture);
                    if((now-lastTime).TotalMinutes>lockoutTime)
                    {
                        session.SetString(GetSessionAttemptsKey(uploadId), formattedNow);
                    }
                    else
                    {
                        session.SetString(GetSessionAttemptsKey(uploadId), attemptsString + sep + formattedNow);
                    }
                }

                int remaining = maxAttempt - session.GetString(GetSessionAttemptsKey(uploadId)).Split(sep).Length;
                throw new WrongPasswordException(uploadId, remaining);
            }

        }

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

        private Image GetImageAndCheckPassword(string id, Func<string, Image> getImage, ISession session)
        {
            Image image = getImage(id);
            if (image.Upload.Password is not null && image.Upload.Password != GetSessionPassword(image.Upload.Id, session))
            {
                throw new PasswordRequiredException(typeof(Image), image.Upload.Id, image.Id);
            }
            return image;
        }
    

        private string GetSessionAttemptsKey(string uploadId)
        {
            return uploadId + "_attempts";
        }

        private string? GetSessionPassword(string uploadId, ISession session)
        {
            return session.GetString(uploadId);
        }

        private Upload GetUploadAndCheckPassword(string id, Func<string, Upload> getUpload, ISession session)
        {
            Upload upload = getUpload(id);
            if (upload.Password is not null && upload.Password != GetSessionPassword(upload.Id, session))
            {
                throw new PasswordRequiredException(typeof(Upload), upload.Id, null);
            }
            return upload;
        }

    }
}