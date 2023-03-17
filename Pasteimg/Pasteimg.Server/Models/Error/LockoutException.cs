using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Models.Error
{
    public class LockoutException : PasteImgException
    {
        public LockoutException(string id, TimeSpan remainingTime, string? message = null) : base(typeof(Upload), id, message)
        {
            RemainingTime = remainingTime;
        }

        public TimeSpan RemainingTime { get; }
    }
}