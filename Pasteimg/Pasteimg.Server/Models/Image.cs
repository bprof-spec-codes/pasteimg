using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{
    public class Image:IEntity
    {
        [MaxLength(120)]
        public string? Description { get; set; }

        [ValidateNever,StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        public bool NSFW { get; set; }

        [NotMapped, JsonIgnore]
        public virtual Upload Upload { get; set; }

        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string UploadID { get; set; }
        
        [NotMapped]
        public IFormFile? Content { get; set; }
        public virtual OptimizationResult? OptimizationResult { get; set; } 
        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }

}