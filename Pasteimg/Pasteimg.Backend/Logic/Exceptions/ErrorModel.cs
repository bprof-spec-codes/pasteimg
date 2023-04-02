using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Represents an error model that can be used to return detailed error information to the client.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Gets or sets the dictionary of key-value pairs that provide additional details about the error.
        /// </summary>
        public Dictionary<string, string>? Details { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier of the error.
        /// </summary>
        public int ErrorId { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Gets or sets the name of the error.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the HTTP status code associated with the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}