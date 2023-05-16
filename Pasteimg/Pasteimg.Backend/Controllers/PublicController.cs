using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Logic.Exceptions;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Repository;

namespace Pasteimg.Backend.Controllers
{
    /// <summary>
    /// Controller for Admin API endpoints.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : LogicController<IPublicLogic>
    {

        private readonly HttpErrorMapper mapper;
        private readonly IRepository<Image> imageRepo;
        public PublicController(IPublicLogic logic, IRepository<Image> imageRepo, HttpErrorMapper mapper) : base(logic)
        {
            this.mapper = mapper;
            this.imageRepo = imageRepo;
        }

        /// <summary>
        /// Validates a password for a specified upload.
        /// </summary>
        /// <param name="uploadId">The ID of the upload to validate the password for.</param>
        /// <param name="password">The password to validate.</param>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpPost("{uploadId}")]
        public ActionResult EnterPassword(string uploadId, [FromBody] string password, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.EnterPassword(uploadId, password, sessionKey);
            return Ok();
        }

        /// <summary>
        /// Gets a list of available error types.
        /// </summary>
        [HttpGet]
        public ActionResult GetErrorTypes()
        {
            return Ok(mapper.GetErrorTypes());
        }
        /// <summary>
        /// Gets the validation schema for uploads.
        /// </summary>

        [HttpGet]
        public ActionResult<ValidationConfiguration> GetValidationSchema()
        {
            return logic.GetValidationConfiguration();
        }
        /// <summary>
        /// Posts a new upload.
        /// </summary>
        /// <param name="upload">The <see cref="Upload"/> data to post.</param>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        /// <returns>An <see cref="ActionResult"/> representing the result of the action.</returns>
        [HttpPost]
        public ActionResult PostUpload([FromBody] Upload upload, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            var testUpload = new Upload()
            {
                TimeStamp = DateTime.UtcNow,
                Images = new List<Models.Image>
                {
                    new Models.Image
                    {
                        Description = "teszt",
                        Content = new Content("C:\\Users\\Samu\\Pictures\\ns5903-image-kwvyax57.jpg","img/jpeg")
                    }
                }
            };
            string json = JsonConvert.SerializeObject(testUpload);
            string json2= JsonConvert.SerializeObject(upload);
            string uploadId = logic.PostUpload(upload, sessionKey); // ????
            return Ok(uploadId);
        }

        [HttpPost]
        public ActionResult PostImage([FromForm] Image imageObject, IFormFile imageFile)
        {
            using (var stream = imageFile.OpenReadStream())
            {
                byte[] buffer = new byte[imageFile.Length];
                stream.Read(buffer, 0, (int)imageFile.Length);

                Image i = new Image()
                {
                    Description = imageObject.Description,
                    NSFW = imageObject.NSFW,
                    UploadId = imageObject.UploadId,
                    Content = new Content()
                    {
                        Data = buffer,
                        FileName = imageFile.FileName,                        
                        ContentType = imageFile.ContentType
                    }
                };


                this.imageRepo.Create(i);
                return Ok(i);
            }
        }
    }
}