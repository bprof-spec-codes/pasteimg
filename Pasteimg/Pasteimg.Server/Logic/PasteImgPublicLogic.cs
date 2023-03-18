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

        public bool IsLockedOut(string uploadId, ISession session)
        {
            DateTime? lockout = GetSessionLockout(uploadId, session);
            return lockout is not null && IsLockedOut(lockout.Value);
        }

        public bool IsLockedOut(DateTime lockout)
        {
            return (DateTime.Now - lockout).TotalMinutes < logic.Configuration.Visitor.LockoutTimeInMinutes;
        }

        public void PostUpload(Upload upload, ISession session)
        {
            logic.PostUpload(upload);
            if (upload.Password != null)
            {
                session.SetString(upload.Id, upload.Password);
            }
        }

        [HttpPost]
        public void SetPassword(string uploadId, string password, ISession session)
        {
            Upload upload = logic.GetUpload(uploadId);
            if (upload.Password is not null)
            {
                DateTime? lockout = GetSessionLockout(uploadId, session);
                if (lockout is null || !IsLockedOut(lockout.Value))
                {
                    if (lockout is not null)
                    {
                        RemoveSessionLockout(uploadId, session);
                        RemoveSessionAttemptCount(uploadId, session);
                    }

                    password = logic.CreateHash(password);

                    if (upload.Password == password)
                    {
                        session.SetString(uploadId, password);
                        RemoveSessionAttemptCount(uploadId, session);
                    }
                    else
                    {
                        int? count = GetSessionAttemptCount(uploadId, session);
                        count = count is null ? 1 : count.Value + 1;
                        SetSessionAttemptCount(uploadId, count.Value, session);

                        int maxAttempt = logic.Configuration.Visitor.MaxFailedAttempt;

                        if (count >= maxAttempt)
                        {
                            SetSessionLockout(uploadId, session);
                        }

                        throw new WrongPasswordException(uploadId, maxAttempt - count.Value);
                    }
                }
                else
                {
                    TimeSpan lockoutTime = TimeSpan.FromMinutes(logic.Configuration.Visitor.LockoutTimeInMinutes);
                    throw new LockoutException(upload.Id, lockoutTime - (DateTime.Now - lockout.Value));
                }
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

        private int? GetSessionAttemptCount(string uploadId, ISession session)
        {
            return session.GetInt32(GetSessionAttemptKey(uploadId));
        }

        private string GetSessionAttemptKey(string uploadId)
        {
            return uploadId + "_attempt";
        }

        private DateTime? GetSessionLockout(string uploadId, ISession session)
        {
            string? lockout = session.GetString(GetSessionLockoutKey(uploadId));
            if (DateTime.TryParseExact(lockout, DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private string GetSessionLockoutKey(string uploadId)
        {
            return uploadId + "_lockout";
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

        private void RemoveSessionAttemptCount(string uploadId, ISession session)
        {
            session.Remove(GetSessionAttemptKey(uploadId));
        }

        private void RemoveSessionLockout(string uploadId, ISession session)
        {
            session.Remove(GetSessionLockoutKey(uploadId));
        }

        private void SetSessionAttemptCount(string uploadId, int value, ISession session)
        {
            session.SetInt32(GetSessionAttemptKey(uploadId), value);
        }

        private void SetSessionLockout(string uploadId, ISession session)
        {
            session.SetString(GetSessionLockoutKey(uploadId), DateTime.Now.ToString(DateTimeFormat, CultureInfo.CurrentCulture));
        }
    }
}