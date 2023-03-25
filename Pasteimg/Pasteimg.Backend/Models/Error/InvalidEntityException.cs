namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when model state is invalid.
    /// </summary>
    public class InvalidEntityException : PasteImgException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="value">The value of the invalid property.</param>
        /// <param name="id">The ID of the entity, if any.</param>
        public InvalidEntityException(Type entityType, string propertyName, object value, string? id = null) :
            base(entityType, id, $"Model state is invalid! PropertyName: {propertyName}, Value: {value}")
        {
            PropertyName = propertyName;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value of the invalid property.
        /// </summary>
        public object Value { get; }
    }
}