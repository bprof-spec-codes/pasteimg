using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;

namespace Pasteimg.Backend.ControllersApi
{
    /// <summary>
    /// Provides API endpoints for admin functions related to the PasteImg service.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPasteImgLogic logic;

        /// <summary>
        /// Initializes a new instance of the AdminController class with the specified IPasteImgLogic instance.
        /// </summary>
        /// <param name="logic">An IPasteImgLogic instance to use for handling administrative requests.</param>
        public AdminController(IPasteImgLogic logic)
        {
            this.logic = logic;
        }

        /// <summary>
        /// Deletes the image with the given ID.
        /// </summary>
        /// <param name="id">The ID of the image to delete.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteImage(string id)
        {
            try
            {
                logic.DeleteImage(id);
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        /// <summary>
        /// Deletes the upload with the given ID.
        /// </summary>
        /// <param name="id">The ID of the upload to delete.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteUpload(string id)
        {
            try
            {
                logic.DeleteUpload(id);
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        /// <summary>
        /// Updates the description and NSFW status of the image with the given ID.
        /// </summary>
        /// <param name="id">The ID of the image to edit.</param>
        /// <param name="description">The new description for the image.</param>
        /// <param name="nsfw">The new NSFW status for the image.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Successful request.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        ///     <item><strong>BadRequest 400:</strong> The modelstate is invalid.</item>
        /// </list>
        /// </returns>
        [HttpPut]
        public ActionResult EditImage(string id, string? description, bool nsfw)
        {
            try
            {
                logic.EditImage(id, description, nsfw);
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        /// <summary>
        /// Returns all images currently stored without files.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        /// </list>
        /// </returns>

        [HttpGet]
        public ActionResult GetAllImage()
        {
            return Ok(logic.GetAllImage());
        }

        /// <summary>
        /// Returns all uploads currently stored without files.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        /// </list>
        /// </returns>
        [HttpGet]
        public ActionResult GetAllUpload()
        {
            return Ok(logic.GetAllUpload());
        }

        /// <summary>
        /// Gets an image by ID without file.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImage(string id)
        {
            return GetContent(id, logic.GetImage);
        }

        /// <summary>
        /// Gets an image with its source file by ID.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImageWithSourceFile(string id)
        {
            return GetContent(id, logic.GetImageWithSourceFile);
        }

        /// <summary>
        /// Gets an image with its thumbnail file by ID.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The image with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetImageWithThumbnailFile(string id)
        {
            return GetContent(id, logic.GetImageWithThumbnailFile);
        }

        /// <summary>
        /// Gets an upload by ID wit.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUpload(string id)
        {
            return GetContent(id, logic.GetUpload);
        }

        /// <summary>
        /// Gets an upload with its source files by ID.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithSourceFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithSourceFiles);
        }

        /// <summary>
        /// Gets an upload with its thumbnail files by ID.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>Ok 200:</strong> Requested content.</item>
        ///     <item><strong>NotFound 404:</strong> The upload with given ID not found.</item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult GetUploadWithThumbnailFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithThumbnailFiles);
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
        public ActionResult PostUpload(Upload upload)
        {
            try
            {
                logic.PostUpload(upload);
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        /// <summary>
        /// Retrieves content of type T associated with a given id using the provided "get" function.
        /// </summary>
        /// <typeparam name="T">The type of the content to retrieve.</typeparam>
        /// <param name="id">The id associated with the content to retrieve.</param>
        /// <param name="get">A function that takes in a string and returns an object of type T.</param>
        /// <returns>
        /// An HTTP actionresult containing the retrieved content.
        /// </returns>
        private ActionResult GetContent<T>(string id, Func<string, T> get)
        {
            try
            {
                return Ok(get(id));
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }
    }
}