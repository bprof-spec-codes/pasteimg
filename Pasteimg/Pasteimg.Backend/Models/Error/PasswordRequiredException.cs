namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when password is required for resource access.
    /// </summary>
    public class PasswordRequiredException : PasteImgException
    {
     
        public PasswordRequiredException()
            : base($"Password required!")
        { }
       
        protected override PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.PasswordRequired;
        }
    }
}