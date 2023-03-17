namespace Pasteimg.Server.Models.Error
{
    public class InvalidEntityException : PasteImgException
    {
        public InvalidEntityException(Type entityType, string propertyName, object value) : 
            base(entityType, null, $"Model state is invalid! PropertyName: {propertyName}, Value: {value}")
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; }
        public object Value { get; }
    }
}