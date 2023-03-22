using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Models.Error;
using TestApi.Controllers;
using Pasteimg.Server.ImageTransformers._Debug;

namespace Pasteimg.Server.Controllers._Debug
{

    public class ImageTransformationTestController : Controller
    {
        private readonly ILogger<ImageTransformationTestController> logger;
        private readonly ImageTransformerTester tester;
        public ImageTransformationTestController(ILogger<ImageTransformationTestController> logger,ImageTransformerTester tester)
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