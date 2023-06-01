using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Controllers
{
    /// <summary>
    /// Controller for Admin API endpoints.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : LogicController<IAdminLogic>
    {
     
        public AdminController(IAdminLogic logic) : base(logic)
        { }
        /// <summary>
        /// Deletes an image with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the image to delete.</param>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpDelete("{id}")]
        public ActionResult DeleteImage(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.DeleteImage(id, sessionKey);
            return Ok();
        }
        /// <summary>
        /// Deletes an upload with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the upload to delete.</param>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpDelete("{id}")]
        public ActionResult DeleteUpload(string id, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.DeleteUpload(id, sessionKey);
            return Ok();
        }
        /// <summary>
        /// Edits the metadata of an image with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the image to edit.</param>
        /// <param name="model">The EditImageModel containing the updated metadata.</param>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpPut("{id}")]
        public ActionResult EditImage(string id, [FromBody] EditImageModel model, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.EditImage(id, model, sessionKey);
            return Ok();
        }
        /// <summary>
        /// Gets a list of all images in the system.
        /// </summary>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpGet]
        public ActionResult GetAllImage([FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return Ok(logic.GetAllImage(sessionKey));
        }
        /// <summary>
        /// Gets a list of all uploads in the system.
        /// </summary>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpGet]
        public ActionResult GetAllUpload([FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return Ok(logic.GetAllUpload(sessionKey));
        }
        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpGet]
        public ActionResult GetConfiguration([FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return Ok(logic.GetConfiguration(sessionKey));
        }
        /// <summary>
        /// Authenticates an admin user.
        /// </summary>
        /// <param name="loginModel">The admin user to authenticate.</param>
        /// <param name="sessionKey">The session key associated with the user's session.</param>
        [HttpPost]
        [HttpPost]
        public ActionResult Login([FromBody] Admin loginModel, [FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.Login(loginModel, sessionKey);
            return Ok();
        }

        /// <summary>
        /// Logs out an admin user.
        /// </summary>
        /// <param name="sessionKey">The session key associated with the user's session.</param>
        [HttpPost]
        public ActionResult Logout([FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            logic.Logout(sessionKey);
            return Ok();
        }

        [HttpGet]
        public ActionResult IsAdmin([FromHeader(Name =SessionKeyHeader)] string? sessionKey) 
        {
            return Ok(logic.IsAdmin(sessionKey));
        }

        /// <summary>
        /// Gets a one time use register key.
        /// </summary>
        /// <param name="sessionKey">The session key of the user performing the action.</param>
        [HttpGet]
        public ActionResult GetRegisterKey([FromHeader(Name = SessionKeyHeader)] string? sessionKey)
        {
            return Ok(logic.GenerateRegisterKey(sessionKey));
        }
    }
}