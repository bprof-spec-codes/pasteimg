namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// Alkalmazás alapkivétel osztálya.
    /// </summary>
    public class PasteImgException : Exception
    {
        public PasteImgException(Type? entityType = null, string? id = null, string? message = null, Exception? innerException = null) : base(message, innerException)
        {
            Id = id;
            EntityType = entityType;
        }
        /// <summary>
        /// Kapcsolodó tartalom tipusa. Opcionális
        /// </summary>
        public Type? EntityType { get; }
        /// <summary>
        /// Kapcsolodó tartalom kulcsa. Opcionális.
        /// </summary>
        public string? Id { get; }
    }
}