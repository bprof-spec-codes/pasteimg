using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Represents binary content such as image files.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Content"/> class.
        /// </summary>
        public Content()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Content"/> class with the specified path and content type.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <param name="contentType">The content type of the file.</param>
        public Content(string path, string contentType)
        {
            Data = File.ReadAllBytes(path);
            ContentType = contentType;
            FileName = Path.GetFileName(path);
        }
        /// <summary>
        /// Gets or sets the content type of the data.
        /// </summary>
        public string ContentType { get; init; }
        /// <summary>
        /// Gets or sets the binary data.
        /// </summary>
        public byte[] Data { get; init; }
        /// <summary>
        /// Gets or sets the file name of the data.
        /// </summary>
        public string FileName { get; init; }
    }

    /// <summary>
    /// Represents an image entity in the data model.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Gets or sets the image data.
        /// </summary>
        [NotMapped]
        public Content? Content { get; set; }

        /// <summary>
        /// Gets or sets the image description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the image.
        /// </summary>

        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        /// <summary>
        ///  Gets or sets a flag indicating whether the image contains adult content.
        /// </summary>
        public bool NSFW { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Models.Upload"/> that the image belongs to.
        /// </summary>
        [JsonIgnore]
        public virtual Upload? Upload { get; set; }

        /// <summary>
        ///  Gets or sets the identifier of the <see cref="Models.Upload"/> that the image belongs to.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string? UploadId { get; set; }
    }
}