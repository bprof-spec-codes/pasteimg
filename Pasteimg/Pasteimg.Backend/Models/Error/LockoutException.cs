using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when resource access is locked out.
    /// </summary>
    public class LockoutException : PasteImgException
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="LockoutException"/> class with the specified parameters.
        /// </summary>
        /// <param name="uploadId"> The Id of the upload the exception is related to.</param>
        public LockoutException() :
            base(message:$"Resource access is locked out.")
        {
        }
        protected override PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.Lockout;
        }
    }
}