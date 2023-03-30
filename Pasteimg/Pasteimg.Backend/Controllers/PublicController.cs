using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;
using System.Runtime.CompilerServices;

namespace Pasteimg.Backend.Controllers
{


    /// <summary>
    /// Provides public API endpoints for accessing image and upload content.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IPasteImgPublicLogic logic;
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicController"/> class.
        /// </summary>
        /// <param name="logic">The logic class for handling public API requests.</param>
        public PublicController(IPasteImgPublicLogic logic)
        {
            this.logic = logic;
        }
        [HttpGet]
        public ActionResult CreateSession()
        {
            return Ok(logic.CreateSession());
        }
        
        private string? GetSessionKey()
        {
            string[]? auth=HttpContext.Request.Headers.Authorization.FirstOrDefault(s => s.ToLower().Contains("basic"))?.Split(' ');
            if (auth is not null && auth.Length == 2 && !string.IsNullOrWhiteSpace(auth[1]))
            {
                return auth[1];
            }
            else return null;
        }
        /// <summary>
        /// Enters the password for an upload into the session, and tracks failed attempts.
        /// </summary>
        /// <param name="uploadId">The ID of the upload to enter the password for.</param>
        /// <param name="password">The password to enter.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> The entered password is wrong.</item>
        ///     <item><strong>BadRequest 400:</strong> Session is locked out for this resource access.</item>
        /// </list>
        /// </returns>
        [HttpPost("{uploadId},{password}")]
        public ActionResult EnterPassword(string uploadId, string password)
        {
            try
            {
                logic.EnterPassword(uploadId,password,GetSessionKey());
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return ex.GetErrorResult();
            }
        }

        /// <summary>
        /// Retrieves an image with the given identifier without file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImage(string id)
        {
            return GetContent(id, logic.GetImage);
        }

        /// <summary>
        /// Retrieves an image with the given identifier with the attached source file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImageWithSourceFile(string id)
        {
            return GetContent(id, logic.GetImageWithSourceFile);
        }

        /// <summary>
        /// Retrieves an image with the given identifier with the attached thumbnail file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImageWithThumbnailFile(string id)
        {
            return GetContent(id, logic.GetImageWithThumbnailFile);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier without associated files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUpload(string id)
        {
            return GetContent(id, logic.GetUpload);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated source files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithSourceFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithSourceFiles);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated thumbnail files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> Password was not entered.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithThumbnailFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithThumbnailFiles);
        }

        /// <summary>
        /// Gets the validation configuration.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        /// </list>
        /// </returns>
        [HttpGet]
        public ActionResult<ValidationConfiguration> GetValidationConfiguration()
        {
            return logic.GetValidationConfiguration();
        }

        /// <summary>
        /// Stores the uploaded images, if modelstate is valid.
        /// </summary>
        /// <param name="upload">The object containing the images to be uploaded.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        ///     <item><strong>BadRequest 400:</strong> The modelstate is invalid.</item>
        ///     <item><strong>InternalServerError 500:</strong> Something wrong happened.</item>
        /// </list>
        /// </returns>
        [HttpPost]
        public ActionResult PostUpload([FromBody] Upload upload)
        {
            try
             {
               string uploadId=logic.PostUpload(upload,GetSessionKey());
                return Ok(uploadId);
            }
            catch (PasteImgException ex)
            {
                return ex.GetErrorResult();
            }
        }
      
        /// <summary>
        /// Sets whether NSFW content should be shown for the current session.
        /// </summary>
        /// <param name="value">True if NSFW content should be shown, false otherwise.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        /// </list>
        /// </returns>


        /// <summary>
        /// Retrieves content of type T associated with a given id using the provided "get" function.
        /// </summary>
        /// <typeparam name="T">The type of the content to retrieve.</typeparam>
        /// <param name="id">The id associated with the content to retrieve.</param>
        /// <param name="get">A function that takes in a string and session returns an object of type T.</param>
        /// <returns>
        /// An HTTP actionresult containing the retrieved content.
        /// </returns>
        private ActionResult GetContent<T>(string id, Func<string,string?,T> get)
        {
            try
            {
                return Ok(get(id,GetSessionKey()));
            }
            catch (PasteImgException ex)
            {
                var result = ex.GetErrorResult();
                return result;
            }
        }
    }
}