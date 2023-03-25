namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when an internal error, such as an IO error, occurs.
    /// </summary>
    public class SomethingWrongException : PasteImgException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SomethingWrongException"/> class with the specified parameters.
        /// </summary>
        /// <param name="entityType">The type of entity the exception is related to.</param>
        /// <param name="id"> The Id of the entity the exception is related to.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public SomethingWrongException(Exception innerException, string? id = null, Type? entityType = null) : base(entityType, id, innerException.Message, innerException)
        {
        }
    }
}