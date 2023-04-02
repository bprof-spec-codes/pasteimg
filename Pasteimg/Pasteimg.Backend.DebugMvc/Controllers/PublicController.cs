using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteimg.Backend.Controllers;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Logic.Exceptions;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Pasteimg.Backend.DebugMvc.Controllers
{
    public class PublicController : Controller
    {
        private readonly HttpClient client;
        private readonly ILogger<HomeController> logger;
        const string api = "https://localhost:7063/api";

        public PublicController(ILogger<HomeController> logger,HttpClient client)
        {
            this.logger = logger;
           this.client=client;
       
        
        }
        private async Task SetSessionAsync()
        {
            if (HttpContext?.Session is not null)
            {
                string apiKey = "API-SESSION-KEY";
                string? apiKeyValue = HttpContext?.Session.GetString(apiKey);
                if (apiKeyValue is null)
                {
                    var response = await client.GetAsync($"{api}/Public/CreateSessionKey");
                        apiKeyValue=await response.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString(apiKey, apiKeyValue);
                }
                client.DefaultRequestHeaders.Add(apiKey, apiKeyValue);
            }
        }

        [HttpGet]
        public IActionResult GetBlankItem()
        {
            return PartialView("_BlankItem", new Upload());
        }

        [HttpGet]
        public async Task<IActionResult> Images(string id)
        {
            await SetSessionAsync(); 
            var response = await client.GetAsync($"{api}/Public/GetUpload/{id}");
            if (response.StatusCode==HttpStatusCode.OK)
            {
                Upload upload =await response.Content.ReadFromJsonAsync<Upload>();

                return View(upload);
            }
            else
            {
                return await Error(response);
            }
        }

   
        [HttpGet]
        public async Task<IActionResult> Source(string id)
        {
            await SetSessionAsync();
            var response =await client.GetAsync($"{api}/Public/GetImage/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Image image = await response.Content.ReadFromJsonAsync<Image>();
                return View(image);
            }
            else
            {
                return await Error(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SourceFile(string id)
        {
            return await GetFile($"{api}/Public/GetImageWithSourceFile/{id}");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPassword(string uploadId, string? imageId, string? password)
        {
            await SetSessionAsync();
            var response =await client.PostAsJsonAsync($"{api}/Public/EnterPassword/{uploadId}", password);
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
                return await Error(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitUpload([ModelBinder(typeof(UploadModelBinder))]Upload upload)
        {
            await SetSessionAsync();
            var response =await client.PostAsJsonAsync($"{api}/Public/PostUpload", upload);
            string responseContent = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Images), new { id = responseContent });
            }
            else
            {
                return await Error(response);
            }
        }
       
        [HttpGet]
        public async Task<IActionResult> ThumbnailFile(string id)
        {
            return await GetFile($"{api}/Public/GetImageWithThumbnailFile/{id}");
        }

        public async Task<IActionResult> Upload()
        {
            await SetSessionAsync();
            return View(new Upload());
        }
        private bool ThatException(ErrorModel model,Type exceptionType)
        {
            return model.Name == exceptionType.Name.ToLower().Replace("exception", "");
        }
        private async Task<IActionResult> Error(HttpResponseMessage response)
        {
            try
            {
                ErrorModel? error = await response.Content.ReadFromJsonAsync<ErrorModel?>();
                if(error is null||error is null)
                {
                    throw new NullReferenceException(nameof(error));
                }

                if (error.Name=="passwordrequired")
                {
                    return View("AskPassword", error);
                }
                else
                {
                    return View("Error", error);
                }
            }
            catch
            {

                return StatusCode((int)response.StatusCode,await  response.Content.ReadAsStringAsync());
            }
        
        }

        private async Task<IActionResult> GetFile(string uri)
        {
            await SetSessionAsync();
            var response = await client.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Image image =await response.Content.ReadFromJsonAsync<Image>();

                return File(image.Content.Data,image.Content.ContentType);
            }
            else
            {
                return await Error(response);
            }
        }
    }
}