using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Configurations;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Models.Error;

namespace TestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IPasteImgPublicLogic logic;

        public PublicController(IPasteImgPublicLogic logic)
        {
            this.logic = logic;
        }

        [HttpGet("{id}")]
        public ActionResult GetImage(string id)
        {
            return GetContent(id, logic.GetImage);
        }

        [HttpGet("{id}")]
        public ActionResult GetImageWithSourceFile(string id)
        {
            return GetContent(id, logic.GetImageWithSourceFile);
        }

        [HttpGet("{id}")]
        public ActionResult GetImageWithThumbnailFile(string id)
        {
            return GetContent(id, logic.GetImageWithThumbnailFile);
        }

        [HttpGet("{id}")]
        public ActionResult GetUpload(string id)
        {
            return GetContent(id, logic.GetUpload);
        }

        [HttpGet("{id}")]
        public ActionResult GetUploadWithSourceFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithSourceFiles);
        }

        [HttpGet("{id}")]
        public ActionResult GetUploadWithThumbnailFiles(string id)
        {
            return GetContent(id, logic.GetUploadWithThumbnailFiles);
        }

        [HttpGet]
        public ActionResult<ValidationConfiguration> GetValidationConfiguration()
        {
            return logic.GetValidationConfiguration();
        }

        [HttpPost]
        public ActionResult PostUpload([FromForm] Upload upload)
        {
            try
            {
                logic.PostUpload(upload, HttpContext.Session);
                return Ok();
            }
            catch (InvalidEntityException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        [HttpPost("{uploadId},{password}")]
        public ActionResult SetPassword(string uploadId, string password)
        {
            try
            {
                logic.SetPassword(uploadId, password, HttpContext.Session);
                return Ok();
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }

        [HttpPost("{value}")]
        public ActionResult SetShowNsfw(bool value)
        {
            logic.SetShowNsfw(value, HttpContext.Session);
            return Ok();
        }

        private ActionResult GetContent<T>(string id, Func<string, ISession, T> get)
        {
            try
            {
                return Ok(get(id, HttpContext.Session));
            }
            catch (PasteImgException ex)
            {
                return new PasteImgErrorResult(ex);
            }
        }
    }
}