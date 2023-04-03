using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Logic
{
    ///<summary>
    ///Interface for managing Pasteimg session logic.
    ///</summary>
    public interface IPasteImgSessionLogic
    {
        ///<summary>
        ///Creates a new session.
        ///</summary>
        ///<returns>A string representing the newly created session.</returns>
        string CreateSession();
        /// <summary>
        /// Retrieves an image with the specified ID and session key.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested image.</returns>
        Image GetImage(string id, string? sessionKey);

        /// <summary>
        /// Retrieves an image with the specified ID and session key, including its source file.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested image with its source file.</returns>
        Image GetImageWithSourceFile(string id, string? sessionKey);

        /// <summary>
        /// Retrieves an image with the specified ID and session key, including its thumbnail file.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested image with its thumbnail file.</returns>
        Image GetImageWithThumbnailFile(string id, string? sessionKey);

        /// <summary>
        /// Retrieves an upload with the specified ID and session key.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested upload.</returns>
        Upload GetUpload(string id, string? sessionKey);

        /// <summary>
        /// Retrieves an upload with the specified ID and session key, including its source files.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested upload with its source files.</returns>
        Upload GetUploadWithSourceFiles(string id, string? sessionKey);

        /// <summary>
        /// Retrieves an upload with the specified ID and session key, including its thumbnail files.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <param name="sessionKey">The session key for the current session.</param>
        /// <returns>The requested upload with its thumbnail files.</returns>
        Upload GetUploadWithThumbnailFiles(string id, string? sessionKey);
    }
}