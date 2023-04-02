using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception thrown when a password is required for accessing a resource.
    /// </summary>
    [HttpError(HttpStatusCode.Unauthorized, 5)]
    public class PasswordRequiredException : PasteImgResourceException
    {
        public PasswordRequiredException() : base($"Password required!")
        {
        }
    }
}