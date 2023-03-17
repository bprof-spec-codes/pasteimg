using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Models.Error
{
    public class LockoutException : PasteImgException
    {
        public LockoutException(string id, TimeSpan remainingTime) : 
            base(typeof(Upload), id,$"Too many attempts! Please try again in {Math.Ceiling(remainingTime.TotalMinutes)} minutes." )
        {
            RemainingTime = remainingTime;
        }

        public TimeSpan RemainingTime { get; }
    }
}