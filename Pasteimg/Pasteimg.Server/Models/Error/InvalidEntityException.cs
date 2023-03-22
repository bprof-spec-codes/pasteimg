namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobódik, ha a feltöltés valamilyen szempontból nem megfelelő.
    /// </summary>
    public class InvalidEntityException : PasteImgException
    {
        public InvalidEntityException(Type entityType, string propertyName, object value, string? id = null) :
            base(entityType, id, $"Model state is invalid! PropertyName: {propertyName}, Value: {value}")
        {
            PropertyName = propertyName;
            Value = value;
        }

        /// <summary>
        /// A kifogásolt tulajdonság neve.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Kifogásolt tulajdonság értéke.
        /// </summary>
        public object Value { get; }
    }
}