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
        /// <param name="id"> The Id of the entity the exception is related to.</param>
        public LockoutException(string id) :
            base(typeof(Upload), id, $"Resource access is locked out. Id: {id}")
        {
        }
    }
}