namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// A view model class representing an error that occurred during the processing of a request.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The <see cref="PasteImgErrorResult"/> that occurred during the processing of the request.
        /// </summary>
        public PasteImgErrorResult? Error { get; set; }
    }
}