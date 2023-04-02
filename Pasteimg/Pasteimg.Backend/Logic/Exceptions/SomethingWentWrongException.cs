using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when something goes wrong during an operation.
    /// </summary>
    [HttpError(HttpStatusCode.InternalServerError, 7)]
    public class SomethingWentWrongException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SomethingWentWrongException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SomethingWentWrongException(Exception innerException, string message = "Something went wrong!") : base(message, innerException)
        {
        }
        /// <summary>
        /// Gets additional information about the exception.
        /// </summary>
        [HttpErrorDetail]
        public IReadOnlyDictionary<string, string>? AdditionalInformation { get; init; }
    }
}