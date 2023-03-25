using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Backend.Models.Entity
{
    /// <summary>
    /// Represents an upload entity in the data model.
    /// </summary>
    [ModelBinder(typeof(UploadModelBinder))]
    public class Upload : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Upload"/> class.
        /// </summary>
        public Upload()
        {
            Images = new List<Image>();
        }

        /// <summary>
        /// Gets or sets the unique identifier of the upload.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        /// <summary>
        ///  Gets or sets the <see cref="Image"/>s  that belong to the upload.
        /// </summary>
        public virtual IList<Image> Images { get; set; }

        /// <summary>
        /// Gets or sets the password that protects the upload.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the upload creation.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets the unique key of the upload entity.
        /// </summary>
        /// <returns> An array of objects containing the <see cref="Upload.Id"/></returns>
        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}