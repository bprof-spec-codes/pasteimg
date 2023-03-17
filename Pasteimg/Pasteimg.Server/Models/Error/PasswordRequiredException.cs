namespace Pasteimg.Server.Models.Error
{
    public class PasswordRequiredException : PasteImgException
    {
        public PasswordRequiredException(Type? entityType, string uploadId, string? imageId) 
            : base(entityType, uploadId, $"Password required! Type: {entityType}, Id: {(imageId is null?uploadId:imageId)}")
        {
            ImageId = imageId;
        }

        public string? ImageId { get; }
    }
}