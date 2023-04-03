using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception thrown when the password provided is incorrect.
    /// </summary>
    [HttpError(HttpStatusCode.Unauthorized, 9)]
    public class WrongPasswordException : PasteImgResourceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongPasswordException"/> class.
        /// </summary>
        /// <param name="remainingAttempt">The number of remaining attempts before the resource is locked out.</param>
        public WrongPasswordException(int remainingAttempt) :
            base($"Password incorrect! Remaining attempt: {remainingAttempt}")
        {
            RemainingAttempt = remainingAttempt;
        }
        /// <summary>
        /// Gets the number of remaining attempts before the resource is locked out.
        /// </summary>
        [HttpErrorDetail]
        public int RemainingAttempt { get; init; }
    }
}