namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when password is required for resource access.
    /// </summary>
    public class PasswordRequiredException : PasteImgException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordRequiredException"/> class with the specified parameters.
        /// </summary>
        /// <param name="entityType">The type of the entity for which the password is required.</param>
        /// <param name="uploadId">The Id of the upload that requires a password.</param>
        /// <param name="imageId">The Id of the image that requires a password. Optional.</param>
        public PasswordRequiredException(Type? entityType, string uploadId, string? imageId)
            : base(entityType, uploadId, $"Password required! Type: {entityType}, Id: {(imageId is null ? uploadId : imageId)}")
        {
            ImageId = imageId;
        }

        /// <summary>
        /// Gets the Id of the image that requires a password.
        /// </summary>
        public string? ImageId { get; }
    }
}