namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when model state is invalid.
    /// </summary>
    public class InvalidEntityException : PasteImgException
    {
 
        public InvalidEntityException(string propertyName, object? propertyValue,string reason) :
            base($"Posted content is invalid! Reason: {reason}.")
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue?.ToString();
            Reason = reason;
        }

        public string Reason { get; }
        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value of the invalid property.
        /// </summary>
        public string? PropertyValue { get; }
        protected override void SetErrorDetails(ErrorDetails details)
        {
            base.SetErrorDetails(details);
            AddValueIfNotNull(details,nameof(PropertyName), PropertyName);
            AddValueIfNotNull(details, nameof(PropertyValue), PropertyValue);
            AddValueIfNotNull(details, nameof(Reason), Reason);
        }
        protected override PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.InvalidEntity;
        }
    }
}