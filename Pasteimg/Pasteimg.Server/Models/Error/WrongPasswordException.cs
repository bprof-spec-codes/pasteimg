using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobódik, ha a kliens rossz jelszót adott meg védett tartalomhoz.
    /// </summary>
    public class WrongPasswordException : PasteImgException
    {
        public WrongPasswordException(string id, int remainingAttempt) : 
            base(typeof(Upload), id, $"Password incorrect! Remaining attempt: {remainingAttempt}")
        {
            RemainingAttempt = remainingAttempt;
        }
        /// <summary>
        /// Hátralévő próbálkozások száma.
        /// </summary>
        public int RemainingAttempt { get; }
    }
}