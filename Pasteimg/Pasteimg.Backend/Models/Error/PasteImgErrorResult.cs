using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json.Serialization;

namespace Pasteimg.Backend.Models.Error
{
    public enum PasteImgErrorStatusCode
    {
        NotFound = HttpStatusCode.NotFound,
        SomethingWrong = HttpStatusCode.InternalServerError,
        PasswordRequired = 452,
        WrongPassword = 453,
        Lockout = 454,
        InvalidEntity = 455
    }
    public class ErrorDetails
    {
        public string Message { get; set; }
        public PasteImgErrorStatusCode StatusCode { get; set; }
        public Dictionary<string,string> KeyValues { get; set; }
    }
    public class PasteImgErrorResult : BadRequestObjectResult
    {
        public PasteImgErrorResult(ErrorDetails details):base(details)
        {
        }
    }
}