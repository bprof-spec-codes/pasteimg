namespace Pasteimg.Server.Models.Error
{
    public class PasteImgException : Exception
    {
        public PasteImgException(Type? entityType = null, string? id = null, string? message = null, Exception? innerException = null) : base(message, innerException)
        {
            Id = id;
            EntityType = entityType;
        }

        public Type? EntityType { get; }
        public string? Id { get; }
    }
}