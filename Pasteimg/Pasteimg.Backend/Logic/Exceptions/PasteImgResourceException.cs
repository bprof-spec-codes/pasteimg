namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// An abstract base class for exceptions related to resources in PasteImg application.
    /// </summary>
    public abstract class PasteImgResourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteImgResourceException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public PasteImgResourceException(string message) : base(message)
        { }
        /// <summary>
        /// Gets or sets the ID of the image associated with the exception, if any.
        /// </summary>
        [HttpErrorDetail]
        public string? ImageId { get; init; }
        /// <summary>
        /// Gets or sets the ID of the upload associated with the exception, if any.
        /// </summary>
        [HttpErrorDetail]
        public string? UploadId { get; init; }
    }
}