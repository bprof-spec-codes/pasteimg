using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Logic.Exceptions;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Repository;
using System;
using System.Text;

namespace Pasteimg.Backend.Logic
{
    /// <summary>
    /// Interface for Admin Logic.
    /// </summary>
    public interface IAdminLogic : IPasteImgSessionLogic
    {
        /// <summary>
        /// Deletes an image.
        /// </summary>
        /// <param name="id">The ID of the image to delete.</param>
        /// <param name="sessionKey">The session key of the user.</param>
        void DeleteImage(string id, string? sessionKey);
        /// <summary>
        /// Deletes an upload.
        /// </summary>
        /// <param name="id">The ID of the upload to delete.</param>
        /// <param name="sessionKey">The session key of the user.</param>
        void DeleteUpload(string id, string? sessionKey);
        /// <summary>
        /// Edits an image.
        /// </summary>
        /// <param name="id">The ID of the image to edit.</param>
        /// <param name="model">The model containing the new image data.</param>
        /// <param name="sessionKey">The session key of the user.</param>
        /// <returns>The edited image.</returns>
        Image EditImage(string id, EditImageModel model, string? sessionKey);
        /// <summary>
        /// Gets all images.
        /// </summary>
        /// <param name="sessionKey">The session key of the user.</param>
        /// <returns>A collection of all images.</returns>
        IEnumerable<Image> GetAllImage(string? sessionKey);
        /// <summary>
        /// Gets all uploads.
        /// </summary>
        /// <param name="sessionKey">The session key of the user.</param>
        /// <returns>A collection of all uploads.</returns>
        IEnumerable<Upload> GetAllUpload(string? sessionKey);
        /// <summary>
        /// Gets the configuration settings.
        /// </summary>
        /// <param name="sessionKey">The session key of the user.</param>
        /// <returns>The PasteImg configuration settings.</returns>
        PasteImgConfiguration GetConfiguration(string? sessionKey);
        bool IsAdmin(string? sessionKey);

        /// <summary>
        /// Logs in an admin user.
        /// </summary>
        /// <param name="loginModel">The admin login model.</param>
        /// <param name="sessionKey">The session key of the user.</param>
        void Login(Admin loginModel, string sessionKey);
        /// <summary>
        /// Logs out the admin user.
        /// </summary>
        /// <param name="sessionKey">The session key of the user.</param>
        void Logout(string sessionKey);

        /// <summary>
        /// Generate a one time use code for admin regsitration.
        /// </summary>
        int GenerateRegisterKey(string sessionKey);

        /// <summary>
        /// Check if regKey is valid, and if is, remove it.
        /// </summary>
        /// <param name="key">The regKey.</param>
        bool RegisterKeyValidator(int key);

        IEnumerable<Upload> GetUploads(string? sessionKey, int number, int pageNum);
    }
    /// <summary>
    /// Interface for Admin Logic.
    /// </summary>
    public class AdminLogic : IAdminLogic
    {
        private const string Admin = "ADMIN";
        private static List<RegisterKey> _registerKeys = new List<RegisterKey>();
        private readonly IRepository<Admin> adminRepository;
        private readonly IPasteImgLogic logic;
        private readonly ISessionHandler sessionHandler;
        /// <summary>
        /// Initializes a new instance of the AdminLogic with the specified dependencies.
        /// </summary>
        /// <param name="logic">The IPasteimgLogic implementation to use.</param>
        /// <param name="adminRepository">The IRepository implementation for Admin entities.</param>
        /// <param name="sessionHandler">The ISessionHandler implementation to use for session management.</param>
        public AdminLogic(IPasteImgLogic logic,
                          IRepository<Admin> adminRepository,
                          ISessionHandler sessionHandler)
        {
            this.logic = logic;
            this.sessionHandler = sessionHandler;
            this.adminRepository = adminRepository;
        }

        public PasteImgConfiguration Configuration => logic.Configuration;
        /// <inheritdoc/>
        public string CreateSession()
        {
            return sessionHandler.CreateNewSession();
        }
        /// <inheritdoc/>
        /// <exception cref="SomethingWentWrongException"/>
        /// <exception cref="NotFoundException"/>
        public void DeleteImage(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            logic.DeleteImage(id);
        }
        /// <inheritdoc/>
        /// <exception cref="SomethingWentWrongException"/>
        /// <exception cref="NotFoundException"/>
        public void DeleteUpload(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            logic.DeleteUpload(id);
        }
        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        /// <exception cref="InvalidEntityException"/>
        public Image EditImage(string id, EditImageModel model, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.EditImage(id, model);
        }

        public int GenerateRegisterKey(string sessionKey)
        {
            CheckIsAdmin(sessionKey);
            int key;
            DateTime date = DateTime.Now;

            RegisterKey regKey = new RegisterKey() { Creation = date }; 
            do
            {
                key = System.DateTime.Now.GetHashCode();
                regKey.Key = key;
            } while (_registerKeys.Any(x => x.Key == regKey.Key));
            _registerKeys.Add(regKey);
            return key;
        }

        /// <inheritdoc/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public IEnumerable<Image> GetAllImage(string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetAllImage();
        }

        /// <inheritdoc/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public IEnumerable<Upload> GetAllUpload(string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetAllUpload();
        }

        /// <inheritdoc/>
       
        public PasteImgConfiguration GetConfiguration(string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.Configuration;
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public Image GetImage(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetImage(id);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public Image GetImageWithSourceFile(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetImageWithSourceFile(id);
        }

        /// <inheritdoc/>
        public Image GetImageWithThumbnailFile(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetImageWithThumbnailFile(id);
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public Upload GetUpload(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetUpload(id);
        }

        public IEnumerable<Upload> GetUploads(string? sessionKey, int number, int pageNum)
        {
            CheckIsAdmin(sessionKey);
            List<Upload> uploads = new List<Upload>(logic.GetAllUpload());
            List<Upload> result = new List<Upload>(number);
            if (pageNum > Math.Ceiling((double)uploads.Count / 2)) pageNum = 1;

            for (int i = (pageNum - 1) * number; i < (pageNum) * number && i < uploads.Count; i++)
            {
                result.Add(uploads[i]);
            }

            return result.AsEnumerable<Upload>();
            
        }

        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public Upload GetUploadWithSourceFiles(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetUploadWithSourceFiles(id);
        }
        /// <inheritdoc/>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public Upload GetUploadWithThumbnailFiles(string id, string? sessionKey)
        {
            CheckIsAdmin(sessionKey);
            return logic.GetUploadWithThumbnailFiles(id);
        }
        /// <inheritdoc/>
        /// <exception cref="SessionKeyMissingException"/>
        /// <exception cref="FailedLoginException"/>
        /// <exception cref="SomethingWentWrongException"/>
        public void Login(Admin loginModel, string? sessionKey)
        {
            ISession? session = sessionHandler.GetSession(sessionKey);

            if (session is null)
            {
                throw new SessionKeyMissingException();
            }

            Admin? admin = adminRepository.Read(loginModel.Email);
            if (admin is null)
            {
                throw new FailedLoginException();
            }
            string? hashedPass = logic.CreateHash(loginModel.Password);
            if (hashedPass != admin.Password)
            {
                throw new FailedLoginException();
            }
            if (sessionKey is null)
            {
                sessionKey = sessionHandler.CreateNewSession();
            }
            session.SetString(Admin, Admin);
            session.CommitAsync().Wait();
        }

        /// <exception cref="UnauthorizedException"/>
        /// <exception cref="SomethingWentWrongException"/>
        /// <inheritdoc/>
        public void Logout(string? sessionKey)
        {
            ISession session = CheckIsAdmin(sessionKey);
            session.Remove(Admin);
            session.CommitAsync();
        }

        public bool RegisterKeyValidator(int key)
        {
            RegisterKey regKey = _registerKeys.FirstOrDefault(x => x.Key == key);
            if (regKey is null) throw new RegisterError("Wrong register key");
            if (regKey.Creation < DateTime.Now.AddHours(-24))
            {
                _registerKeys.Remove(regKey);
                throw new RegisterError("Key expired");
            }
            _registerKeys.Remove(regKey);
            return true;
            
        }

        /// <inheritdoc/>



        /// <summary>
        /// Checks if the session identified by the given session key belongs to an admin user.
        /// Throws an UnauthorizedException if the session is invalid or the user is not an admin.
        /// Returns the session object if the user is an admin.
        /// </summary>
        /// <param name="sessionKey">The session key identifying the user's session.</param>
        /// <returns>The session object if the user is an admin.</returns>
        /// <exception cref="UnauthorizedException"/>
        private ISession CheckIsAdmin(string? sessionKey)
        {
            ISession? session = sessionHandler.GetSession(sessionKey);
            if (session?.GetString(Admin) is null)
            {
                throw new UnauthorizedException();
            }
            return session;
        }

        public bool IsAdmin(string? sessionKey)
        {
            ISession? session = sessionHandler.GetSession(sessionKey);
            return session?.GetString(Admin) != null;
        }
    }
}