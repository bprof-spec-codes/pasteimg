namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobódik, ha a kliens meg próbál hozzáférni egy védett tartalomhoz, de még nem adta meg a helyes jelszót.
    /// </summary>
    public class PasswordRequiredException : PasteImgException
    {
        public PasswordRequiredException(Type? entityType, string uploadId, string? imageId)
            : base(entityType, uploadId, $"Password required! Type: {entityType}, Id: {(imageId is null ? uploadId : imageId)}")
        {
            ImageId = imageId;
        }

        public string? ImageId { get; }
    }
}