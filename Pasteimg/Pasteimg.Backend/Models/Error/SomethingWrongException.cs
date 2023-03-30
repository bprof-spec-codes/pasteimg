namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Exception thrown when an internal error, such as an IO error, occurs.
    /// </summary>
    public class SomethingWrongException : PasteImgException
    {

        public SomethingWrongException(Exception innerException,string? message=null) : base("Something wrong happened! "+message, innerException)
        {
        }
    }
}