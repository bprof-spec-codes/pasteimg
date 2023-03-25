using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when password provided for resource access is incorrect.
    /// </summary>
    public class WrongPasswordException : PasteImgException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongPasswordException"/> class with the specified parameters.
        /// </summary>
        /// <param name="id"> The Id of the entity the exception is related to.</param>
        /// <param name="remainingAttempt">The remaining number of attempts allowed to enter the correct password.</param>
        public WrongPasswordException(string id, int remainingAttempt) :
            base(typeof(Upload), id, $"Password incorrect! Remaining attempt: {remainingAttempt}")
        {
            RemainingAttempt = remainingAttempt;
        }

        /// <summary>
        /// Gets the remaining number of attempts allowed to enter the correct password.
        /// </summary>
        public int RemainingAttempt { get; }
    }
}