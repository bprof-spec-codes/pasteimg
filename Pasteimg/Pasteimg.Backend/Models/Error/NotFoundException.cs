namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when requested resource is not found.
    /// </summary>
    public class NotFoundException : PasteImgException
    {
        
        public NotFoundException()
            : base($"The requested resource is not found.")
        {
        }
        protected override PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.NotFound;
        }
    }
}