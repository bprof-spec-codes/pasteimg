using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception thrown when resource locked out for current session
    /// </summary>
    [HttpError(HttpStatusCode.BadRequest, 3)]
    public class LockoutException : PasteImgResourceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockoutException"/> class with a default error message.
        /// </summary>
        public LockoutException() :
        base(message: $"Resource access is locked out.")
        {
        }
    }
}