namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// An exception class that represents an error that occurred within the Pasteimg server.
    /// </summary>
    public class PasteImgException : Exception
    {
      
        public PasteImgException(string? message = null, Exception? innerException = null) : base(message, innerException)
        { }

        public string? UploadId { get; init; }
        public string? ImageId { get; init; }
        protected virtual PasteImgErrorStatusCode GetStatusCode()
        {
            return PasteImgErrorStatusCode.SomethingWrong;
        }
        protected virtual void SetErrorDetails(ErrorDetails details)
        {
            
           AddValueIfNotNull(details,nameof(UploadId), UploadId);
           AddValueIfNotNull(details,nameof(ImageId), ImageId);
        }

        public PasteImgErrorResult GetErrorResult()
        {
            ErrorDetails details = new ErrorDetails();
            details.Message = Message;
            details.StatusCode = GetStatusCode();
            details.KeyValues = new Dictionary<string, string>();
            SetErrorDetails(details);
            PasteImgErrorResult error = new PasteImgErrorResult(details);
            return error;
        }
        protected void AddValueIfNotNull(ErrorDetails details,string key, object? value)
        {
            if(value is not null)
            {
                details.KeyValues.Add(key, value.ToString());
            }
        }
    }
}