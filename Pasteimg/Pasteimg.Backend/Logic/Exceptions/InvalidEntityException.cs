using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when posted content is invalid.
    /// </summary>
    [HttpError(HttpStatusCode.BadRequest, 2)]
    public class InvalidEntityException : PasteImgResourceException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidEntityException class with a specified property name, property value, and reason for invalidity.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="propertyValue">The value of the invalid property.</param>
        /// <param name="reason">The reason why the content is invalid.</param>
        public InvalidEntityException(string propertyName, object? propertyValue, string reason) :
            base($"Posted content is invalid! Reason: {reason}.")
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue?.ToString();
            Reason = reason;
        }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        [HttpErrorDetail]
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value of the invalid property.
        /// </summary>
        [HttpErrorDetail]
        public string? PropertyValue { get; }
        /// <summary>
        /// Gets the reason why the content is invalid.
        /// </summary>

        [HttpErrorDetail]
        public string Reason { get; }
    }
}