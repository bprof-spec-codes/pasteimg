using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteimg.Backend.Controllers;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Pasteimg.Backend.DebugMvc.Controllers
{
    public class PublicController : Controller
    {
        HttpClient client;
        private readonly ILogger<HomeController> logger;
        const string api = "https://localhost:7063/api";

        public PublicController(ILogger<HomeController> logger)
        {
            this.logger = logger;
            client = new HttpClient();
       
        
        }
        private void SetSession()
        {
            if (HttpContext?.Session is not null)
            {
                string? sessionKey = HttpContext?.Session.GetString("sessionKey");
                if (sessionKey is null)
                {
                    sessionKey = client.GetAsync($"{api}/Public/CreateSession")
                               .Result.Content.ReadAsStringAsync().Result;
                    HttpContext.Session.SetString("sessionKey", sessionKey);
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", sessionKey);
            }
        }

        [HttpGet]
        public IActionResult GetBlankItem()
        {
            return PartialView("_BlankItem", new Upload());
        }

        [HttpGet]
        public IActionResult Images(string id)
        {
            SetSession(); 
            var response = client.GetAsync($"{api}/Public/GetUpload/{id}").Result;
            if (response.StatusCode==HttpStatusCode.OK)
            {
                Upload upload = response.Content.ReadFromJsonAsync<Upload>().Result;

                return View(upload);
            }
            else
            {
                return Error(response);
            }
        }

   
        [HttpGet]
        public IActionResult Source(string id)
        {
            SetSession();
            var response = client.GetAsync($"{api}/Public/GetImage/{id}").Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Image image = response.Content.ReadFromJsonAsync<Image>().Result;
                return View(image);
            }
            else
            {
                return Error(response);
            }
        }

        [HttpGet]
        public IActionResult SourceFile(string id)
        {
            SetSession();
            return GetFile($"{api}/Public/GetImageWithSourceFile/{id}");
        }

        [HttpPost]
        public IActionResult SubmitPassword(string uploadId, string? imageId, string? password)
        {
            SetSession();
            var response = client.PostAsJsonAsync($"{api}/Public/EnterPassword/{uploadId},{password??"_"}", "").Result;
            if(response.IsSuccessStatusCode)
            {
                if(imageId is null)
                {
                    return RedirectToAction(nameof(Images), new { id = uploadId });
                }
                else
                {
                    return RedirectToAction(nameof(Source), new { id =imageId });
                }
            }
            else
            {
                return Error(response);
            }
        }

        [HttpPost]
        public IActionResult SubmitUpload([ModelBinder(typeof(UploadModelBinder))]Upload upload)
        {
            SetSession();
            var response =client.PostAsJsonAsync($"{api}/Public/PostUpload", upload).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Images), new { id = responseContent });
            }
            else
            {
                return Error(response);
            }
        }
       
        [HttpGet]
        public IActionResult ThumbnailFile(string id)
        {
            return GetFile($"{api}/Public/GetImageWithThumbnailFile/{id}");
        }

        public IActionResult Upload()
        {
            SetSession();
            return View(new Upload());
        }

        private IActionResult Error(HttpResponseMessage response)
        {
            try
            {
                ErrorDetails errorDetails = response.Content.ReadFromJsonAsync<ErrorDetails>().Result;
                if (errorDetails.StatusCode == PasteImgErrorStatusCode.PasswordRequired)
                {
                    return View("AskPassword", errorDetails);
                }
                else
                {
                    return View("Error", errorDetails);
                }
            }
            catch
            {

                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        
        }

        private IActionResult GetFile(string uri)
        {
            SetSession();
            var response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Image image = response.Content.ReadFromJsonAsync<Image>().Result;

                return File(image.Content.Data,image.Content.ContentType);
            }
            else
            {
                return Error(response);
            }
        }
    }
}