namespace Pasteimg.Server.Models.Error
{
    public class PasswordRequiredException : PasteImgException
    {
        public PasswordRequiredException(Type? entityType, string uploadId, string? imageId, string? message = null) : base(entityType, uploadId, message)
        {
            ImageId = imageId;
        }

        public string? ImageId { get; }
    }
}