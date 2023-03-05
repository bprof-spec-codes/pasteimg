using Newtonsoft.Json;
using Pasteimg.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{

    public class Image:IEntity
    {
        [Key]
        public string? Id { get; set; }

        [ForeignKey(nameof(Upload))]
        public string? UploadID { get; set; }
        [NotMapped, JsonIgnore]
        public Upload? Upload { get; set; }
        
        [MaxLength(120)]
        public string Description { get; set; }
        
        public bool NSFW { get; set; }
        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }

    public class ImageModel:Image
    {
        public IFormFile? Content { get; set; }
    }
  
   
}
