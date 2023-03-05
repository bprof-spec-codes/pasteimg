using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models;

namespace Pasteimg.Server.Controllers
{
    public class DebugViewController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly DebugLogic _logic;
        public DebugViewController(ILogger<HomeController> logger, DebugLogic logic)
        {
            _logger = logger;
            _logic = logic;

        }

        [HttpGet]
        public IActionResult GetBlankItem()
        {
            return PartialView("_BlankItem", new UploadModel());
        }

        public IActionResult Upload()
        {
            return View("UploadModel",new UploadModel());
        }

        [HttpPost]
        public IActionResult SubmitUpload(UploadModel upload)
        {
            return RedirectToAction(nameof(Upload));
        }
        public IActionResult UploadList()
        {
            throw new NotImplementedException();
        }
        public IActionResult Images(string uploadKey)
        {
            throw new NotImplementedException();
        }
    }
}
