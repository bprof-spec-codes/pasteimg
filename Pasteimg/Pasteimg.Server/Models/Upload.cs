using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{
    [ModelBinder(typeof(UploadModelBinder))]
    public class Upload :IEntity
    {
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }
        public string? Password { get; set; }
        public DateTime TimeStamp { get; set; }

        public Upload()
        {
            Images = new List<Image>();
        }

        [NotMapped]
        public virtual IList<Image> Images { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
  
}