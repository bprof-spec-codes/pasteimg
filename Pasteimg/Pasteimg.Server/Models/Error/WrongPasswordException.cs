using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Models.Error
{
    public class WrongPasswordException : PasteImgException
    {
        public WrongPasswordException(string id, int remainingAttempt) : 
            base(typeof(Upload), id, $"Password incorrect! Remaining attempt: {remainingAttempt}")
        {
            RemainingAttempt = remainingAttempt;
        }

        public int RemainingAttempt { get; }
    }
}