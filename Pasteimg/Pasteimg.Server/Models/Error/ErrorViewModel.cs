namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// Alkalmaz�s hiba-modellje.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Modell kiv�tele.
        /// </summary>
        public PasteImgException? Error { get; set; }
    }
}