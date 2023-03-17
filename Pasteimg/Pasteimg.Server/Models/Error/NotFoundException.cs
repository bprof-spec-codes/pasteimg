namespace Pasteimg.Server.Models.Error
{
    public class NotFoundException : PasteImgException
    {
        public NotFoundException(Type? entityType, string id) 
            : base(entityType, id, $"The request resource is not found. Type: {entityType}, Id: {id}")
        {
        }
    }
}