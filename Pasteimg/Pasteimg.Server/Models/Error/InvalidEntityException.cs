namespace Pasteimg.Server.Models.Error
{
    public class InvalidEntityException : PasteImgException
    {
        public InvalidEntityException(Type entityType, string name, object value, string? message = null) : base(entityType, null, message)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}