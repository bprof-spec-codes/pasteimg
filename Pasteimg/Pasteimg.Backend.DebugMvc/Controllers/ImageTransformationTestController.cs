using Microsoft.AspNetCore.Mvc;
using Pasteimg.Backend.DebugMvc.ImageTransformers;

namespace Pasteimg.Backend.DebugMvc.Controllers
{
    public class ImageTransformationTestController : Controller
    {
        private readonly ILogger<ImageTransformationTestController> logger;
        private readonly ImageTransformerTester tester;

        public ImageTransformationTestController(ILogger<ImageTransformationTestController> logger, ImageTransformerTester tester)
        {
            this.logger = logger;
            this.tester = tester;
        }

        public IActionResult Index()
        {
            return View(tester.Results);
        }
    }
}