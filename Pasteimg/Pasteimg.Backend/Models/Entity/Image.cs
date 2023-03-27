using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Pasteimg.Backend.Models.Entity
{
    /// <summary>
    /// Represents an image entity in the data model.
    /// </summary>
    public class Image : IEntity
    {
        /// <summary>
        /// Gets or sets the image data.
        /// </summary>
        [NotMapped]
        public IFormFile? Content { get; set; }

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
        /// Gets or sets the <see cref="Entity.Upload"/> that the image belongs to.
        /// </summary>
        [JsonIgnore]
        public virtual Upload Upload { get; set; }

        /// <summary>
        ///  Gets or sets the identifier of the <see cref="Entity.Upload"/> that the image belongs to.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string UploadID { get; set; }

        /// <summary>
        ///  Gets the unique key of the image entity.
        /// </summary>
        /// <returns> An array of objects containing the image <see cref="Image.Id"/></returns>
        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}