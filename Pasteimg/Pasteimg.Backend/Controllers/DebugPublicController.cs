using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.ControllersApi;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;

namespace Pasteimg.Backend.Controllers
{
    public class DebugViewController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IPasteImgPublicLogic logic;

        public DebugViewController(ILogger<HomeController> logger, IPasteImgPublicLogic logic)
        {
            this.logger = logger;
            this.logic = logic;
        }

        private PublicController PublicController => new PublicController(logic) { ControllerContext = ControllerContext };

        [HttpGet]
        public IActionResult GetBlankItem()
        {
            return PartialView("_BlankItem", new Upload());
        }

        [HttpGet]
        public IActionResult Images(string id)
        {
            var result = PublicController.GetUpload(id);
            if (result is OkObjectResult ok)
            {
                return View(ok.Value);
            }
            else if (result is not PasteImgErrorResult error)
            {
                return result;
            }
            else if (error.Exception is PasswordRequiredException ex)
            {
                return View("AskPassword", ex);
            }
            else
            {
                return Error(error);
            }
        }

        [HttpPost]
        public IActionResult SetShowNsfw(bool value)
        {
            return PublicController.SetShowNsfw(value);
        }

        [HttpGet]
        public IActionResult Source(string id)
        {
            var result = PublicController.GetImage(id);
            if (result is OkObjectResult ok)
            {
                return View(ok.Value);
            }
            else if (result is not PasteImgErrorResult error)
            {
                return result;
            }
            else if (error.Exception is PasswordRequiredException ex)
            {
                return View("AskPassword", ex);
            }
            else
            {
                return Error(error);
            }
        }

        [HttpGet]
        public IActionResult SourceFile(string id)
        {
            return GetFile(PublicController.GetImageWithSourceFile(id));
        }

        [HttpPost]
        public IActionResult SubmitPassword(string uploadId, string? imageId, string? password)
        {
            return HandleAction(PublicController.EnterPassword(uploadId, password),
                    r => imageId is null ?
                        RedirectToAction(nameof(Images), new { id = uploadId })
                        : RedirectToAction(nameof(Source), new { id = imageId }));
        }

        [HttpPost]
        public IActionResult SubmitUpload(Upload upload)
        {
            return HandleAction(PublicController.PostUpload(upload),
                r => RedirectToAction(nameof(Images), new { id = upload.Id }));
        }

        [HttpGet]
        public IActionResult ThumbnailFile(string id)
        {
            return GetFile(PublicController.GetImageWithThumbnailFile(id));
        }

        public IActionResult Upload()
        {
            return View(new Upload());
        }

        private IActionResult Error(PasteImgErrorResult error)
        {
            return View("Error", new ErrorViewModel() { Error = error });
        }

        private IActionResult GetFile(IActionResult result)
        {
            if (result is OkObjectResult ok)
            {
                Image image = ok.Value as Image;
                return File(image.Content.ToArray(), image.Content.ContentType);
            }
            else if (result is not PasteImgErrorResult error)
            {
                return result;
            }
            else if (error.Exception is PasswordRequiredException ex)
            {
                return View("AskPassword", ex);
            }
            else
            {
                return Error(error);
            }
        }

        private IActionResult HandleAction(IActionResult result, Func<IActionResult, IActionResult> okAction)
        {
            if (result is OkResult || result is OkObjectResult)
            {
                return okAction(result);
            }
            else if (result is PasteImgErrorResult error)
            {
                return Error(error);
            }
            else
            {
                return result;
            }
        }
    }
}