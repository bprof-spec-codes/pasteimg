namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// Alkalmazás hiba-modellje.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Modell kivétele.
        /// </summary>
        public PasteImgException? Error { get; set; }
    }
}