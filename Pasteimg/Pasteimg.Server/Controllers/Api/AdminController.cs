using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models.Error;

namespace TestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPasteImgLogic logic;

        public AdminController(IPasteImgLogic logic)
        {
            this.logic = logic;
        }

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

        [HttpGet]
        public ActionResult GetAllImage()
        {
            return Ok(logic.GetAllImage());
        }

        [HttpGet]
        public ActionResult GetAllUpload()
        {
            return Ok(logic.GetAllUpload());
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