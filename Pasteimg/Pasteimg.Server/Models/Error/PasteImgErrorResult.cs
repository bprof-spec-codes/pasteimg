using Microsoft.AspNetCore.Mvc;

namespace Pasteimg.Server.Models.Error
{
    public class PasteImgErrorResult : BadRequestObjectResult
    {
        public PasteImgErrorResult(PasteImgException ex) : base(ex)
        {
            Exception = ex;
        }

        public PasteImgException Exception { get; }
    }
}