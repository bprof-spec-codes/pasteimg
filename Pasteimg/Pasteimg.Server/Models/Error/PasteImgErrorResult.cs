using Microsoft.AspNetCore.Mvc;

namespace Pasteimg.Server.Models.Error
{
    public class PasteImgErrorResult : ObjectResult
    {
        public PasteImgErrorResult(PasteImgException ex) : base(ex)
        {
            Exception = ex;
        }

        public PasteImgException Exception { get; }
    }
}