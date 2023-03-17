using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models.Entity
{
    public class Image : IEntity
    {
        [NotMapped]
        public IFormFile? Content { get; set; }

        public string? Description { get; set; }

        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        public bool NSFW { get; set; }

        [NotMapped, JsonIgnore]
        public virtual Upload Upload { get; set; }

        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string UploadID { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}