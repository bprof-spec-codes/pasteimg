using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a user is not authorized to access a resource.
    /// </summary>
    [HttpError(HttpStatusCode.Unauthorized, 8)]
    public class UnauthorizedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with the default message.
        /// </summary>
        public UnauthorizedException() : base("Please login!")
        { }
    }
}