using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Models.Error
{
    public class WrongPasswordException : PasteImgException
    {
        public WrongPasswordException(string id, int remainingAttempt, string? message = null) : base(typeof(Upload), id, message)
        {
            RemainingAttempt = remainingAttempt;
        }

        public int RemainingAttempt { get; }
    }
}