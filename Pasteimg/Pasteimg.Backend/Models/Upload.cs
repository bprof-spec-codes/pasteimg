using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

//using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Represents an upload entity in the data model.
    /// </summary>
    public class Upload
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
    }
}