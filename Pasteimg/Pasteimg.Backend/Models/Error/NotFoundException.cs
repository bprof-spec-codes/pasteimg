namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when requested resource is not found.
    /// </summary>
    public class NotFoundException : PasteImgException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> with the specified parameters.
        /// </summary>
        /// <param name="entityType">The type of entity the exception is related to.</param>
        /// <param name="id"> The Id of the entity the exception is related to.</param>
        public NotFoundException(Type? entityType, string id)
            : base(entityType, id, $"The request resource is not found. Type: {entityType}, Id: {id}")
        {
        }
    }
}