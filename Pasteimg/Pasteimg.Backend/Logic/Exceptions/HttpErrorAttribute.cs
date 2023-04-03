using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Defines an attribute to associate an HTTP status code and an error ID with an exception type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class HttpErrorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpErrorAttribute"/> class with the specified HTTP status code and error ID.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to associate with the exception.</param>
        /// <param name="errorId">The unique error ID to associate with the exception.</param>
        public HttpErrorAttribute(HttpStatusCode statusCode, int errorId)
        {
            StatusCode = statusCode;
            ErrorId = errorId;
        }
        /// <summary>
        /// Gets the unique error ID associated with the exception.
        /// </summary>
        public int ErrorId { get; }
        /// <summary>
        /// Gets or sets a custom HTTP message to associate with the exception.
        /// </summary>
        public string? CustomHttpMessage { get; init; }
        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class HttpErrorDetailAttribute : Attribute
    {
        public string? CustomName { get; init; }
    }
}