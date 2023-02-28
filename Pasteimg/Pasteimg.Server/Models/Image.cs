using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{
    public class Image
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        public bool NSFW { get; set; }

        [MaxLength(120)]
        public string Description { get; set; }

        [NotMapped]
        public virtual Upload Upload { get; set; }

        [ForeignKey(nameof(Upload))]
        public string UploadID { get; set; }
    }
}
