using Microsoft.AspNetCore.Mvc;

namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// Becsomagolja a kiváltott kivételt egy http válaszba (400-as hibakód).
    /// </summary>
    public class PasteImgErrorResult : BadRequestObjectResult
    {
        public PasteImgErrorResult(PasteImgException ex) : base(ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// Becsomagolt kivétel.
        /// </summary>
        public PasteImgException Exception { get; }
    }
}