
using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// Exception that represents the requested resource is not found.
    /// </summary>
    [HttpError(HttpStatusCode.NotFound, 4)]
    public class NotFoundException : PasteImgResourceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        public NotFoundException() : base($"The requested resource is not found.")
        { }
    }
}