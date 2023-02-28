using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{
    public class Upload
    {
        public Upload()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        [NotMapped]
        public virtual List<Image> Images { get; set; }
    }
}
