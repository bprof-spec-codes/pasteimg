namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobóbik, ha valamilyen belsőhiba (IO- vagy képátalakítások) történt a feltöltés feldolgozásakor.
    /// Becsomagolja az eredeti kivételt.
    /// </summary>
    public class SomethingWrongException : PasteImgException
    {
        public SomethingWrongException(Exception innerException, string? id = null, Type? entityType = null) : base(entityType, id, innerException.Message, innerException)
        {
        }
    }
}