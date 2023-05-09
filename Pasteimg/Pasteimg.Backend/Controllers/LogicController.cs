using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Controllers
{
    /// <summary>
    /// A base controller for implementing logic-based controllers.
    /// </summary>
    /// <typeparam name="TLogic">The type of logic implementation used by the controller.</typeparam>
    public abstract class LogicController<TLogic> : ControllerBase where TLogic : IPasteImgSessionLogic
    {
        public const string SessionKeyHeader = "API-SESSION-KEY";
        protected readonly TLogic logic;
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicController{TLogic}"/> class with the provided logic implementation.
        /// </summary>
        public LogicController(TLogic logic)
        {
            this.logic = logic;
        }

        [HttpGet("Image")]
        public IActionResult GetImageFIle(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            Image img = this.logic.GetImageWithSourceFile(id, sessionKey); 
            if (img == null)
            {
                return NotFound();
            }

            return new FileContentResult(img.Content.Data, img.Content.ContentType);
        }

        /// <summary>
        /// Creates a new session key and returns it in the response.
        /// </summary>
        [HttpGet]
        public ActionResult CreateSessionKey()
        {
            return Ok(logic.CreateSession());
        }

        /// <summary>
        /// Gets the name of the session key header.
        /// </summary>
        [HttpGet]
        public ActionResult GetSessionKeyHeaderName()
        {
            return Ok(SessionKeyHeader);
        }
        /// <summary>
        /// Retrieves an image with the given identifier without file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetImage(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetImage);
        }

        /// <summary>
        /// Retrieves an image with the given identifier with the attached source file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetImageWithSourceFile(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetImageWithSourceFile);
        }

        /// <summary>
        /// Retrieves an image with the given identifier with the attached thumbnail file, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetImageWithThumbnailFile(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetImageWithThumbnailFile);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier without associated files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetUpload(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetUpload);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated source files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithSourceFiles(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetUploadWithSourceFiles);
        }

        /// <summary>
        /// Retrieves the upload with the given identifier and all associated thumbnail files, and checks if a password is required and entered correctly by the user.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithThumbnailFiles(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return GetContent(id, sessionKey, logic.GetUploadWithThumbnailFiles);
        }

        /// <summary>
        /// Retrieves content of type T associated with a given id using the provided "get" function.
        /// </summary>
        /// <typeparam name="T">The type of the content to retrieve.</typeparam>
        /// <param name="id">The id associated with the content to retrieve.</param>
        /// <param name="get">A function that takes in a string and session returns an object of type T.</param>
        /// <returns>
        /// An HTTP actionresult containing the retrieved content.
        /// </returns>
        private ActionResult GetContent<T>(string id, string? sessionKey, Func<string, string?, T> get)
        {
            return Ok(get(id, sessionKey));
        }
    }
}