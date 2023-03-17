namespace Pasteimg.Server.Models.Error
{
    public class NotFoundException : PasteImgException
    {
        public NotFoundException(Type? entityType, string id, string? message = null) : base(entityType, id, message)
        {
        }
    }
}