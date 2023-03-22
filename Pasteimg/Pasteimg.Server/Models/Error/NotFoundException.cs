namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobódik, ha a keresett tartalom nem található.
    /// </summary>
    public class NotFoundException : PasteImgException
    {
        public NotFoundException(Type? entityType, string id)
            : base(entityType, id, $"The request resource is not found. Type: {entityType}, Id: {id}")
        {
        }
    }
}