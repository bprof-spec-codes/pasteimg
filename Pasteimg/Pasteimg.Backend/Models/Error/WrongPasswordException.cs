using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when password provided for resource access is incorrect.
    /// </summary>
    public class WrongPasswordException : PasteImgException
    {

        public WrongPasswordException(int remainingAttempt) :
            base($"Password incorrect! Remaining attempt: {remainingAttempt}")
        {
            RemainingAttempt = remainingAttempt;
        }

        /// <summary>
        /// Gets the remaining number of attempts allowed to enter the correct password.
        /// </summary>
        public int RemainingAttempt { get; }
        protected override PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.WrongPassword;
        }
        protected override void SetErrorDetails(ErrorDetails details)
        {
            base.SetErrorDetails(details);
            AddValueIfNotNull(details,nameof(RemainingAttempt), RemainingAttempt);
        }
    }
}