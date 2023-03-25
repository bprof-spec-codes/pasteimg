namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// An exception class that represents an error that occurred within the Pasteimg server.
    /// </summary>
    public class PasteImgException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteImgException"/> class with the specified parameters.
        /// </summary>
        /// <param name="entityType">The type of entity the exception is related to.</param>
        /// <param name="id"> The Id of the entity the exception is related to.</param>
        /// <param name="message">The error message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public PasteImgException(Type? entityType = null, string? id = null, string? message = null, Exception? innerException = null) : base(message, innerException)
        {
            Id = id;
            EntityType = entityType;
        }

        /// <summary>
        /// Gets the type of entity the exception is related to.
        /// </summary>
        public Type? EntityType { get; }

        /// <summary>
        /// Gets the Id of the entity the exception is related to.
        /// </summary>
        public string? Id { get; }
    }
}