using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception thrown when a login attempt fails due to an incorrect email or password.
    /// </summary>
    [HttpError(HttpStatusCode.Unauthorized, 1)]
    public class FailedLoginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedLoginException"/> class with a default message.
        /// </summary>
        public FailedLoginException() : base("Wrong email or password!")
        { }
    }
}