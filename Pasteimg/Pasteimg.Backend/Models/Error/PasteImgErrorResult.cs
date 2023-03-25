using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Pasteimg.Backend.Models.Error
{
    /// <summary>
    /// Represents a custom error result that wraps a <see cref="PasteImgException"/> as a HttpResponse.
    /// </summary>
    public class PasteImgErrorResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteImgErrorResult"/> class with the specified exception.
        /// </summary>
        /// <param name="ex">The PasteImgException instance to wrap.</param>
        public PasteImgErrorResult(PasteImgException ex) : base(ex)
        {
            Exception = ex;
            StatusCode = GetStatusCode(ex);
        }

        /// <summary>
        /// Gets the wrapped PasteImgException instance.
        /// </summary>
        public PasteImgException Exception { get; }

        /// <summary>
        /// Gets the HTTP status code based on the type of PasteImgException thrown.
        /// </summary>
        /// <param name="ex">The PasteImgException to determine the status code for.</param>
        /// <returns>The HTTP status code.</returns>
        private int GetStatusCode(PasteImgException ex)
        {
            if (ex is NotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }
            else if (ex is SomethingWrongException)
            {
                return (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                return (int)HttpStatusCode.BadRequest;
            }
        }
    }
}