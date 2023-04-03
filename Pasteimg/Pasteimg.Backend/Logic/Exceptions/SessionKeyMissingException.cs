using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception that is thrown when a session key is missing.
    /// </summary>
    [HttpError(HttpStatusCode.Unauthorized, 6)]
    public class SessionKeyMissingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionKeyMissingException"/> class.
        /// </summary>
        public SessionKeyMissingException() : base("Session key is required!")
        { }
    }
}