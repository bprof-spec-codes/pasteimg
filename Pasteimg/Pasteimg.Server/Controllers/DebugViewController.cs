using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models;
using System.Diagnostics;

namespace Pasteimg.Server.Controllers
{
    public class DebugViewController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IPasteImgLogic _logic;

        public DebugViewController(ILogger<HomeController> logger, IPasteImgLogic logic)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public IActionResult GetBlankItem()
        {
            return PartialView("_BlankItem", new Upload());
        }

        public IActionResult ListUploads()
        {
            return View(_logic);
        }

        public IActionResult Upload()
        {
            return View(new Upload());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitUpload(Upload upload)
        {
            await _logic.UploadAndStoreResultsAsync(upload);
            return RedirectToAction(nameof(ListUploads));
        }

    }
}