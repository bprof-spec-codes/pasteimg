using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models.Entity
{
    [ModelBinder(typeof(UploadModelBinder))]
    public class Upload : IEntity
    {
        public Upload()
        {
            Images = new List<Image>();
        }

        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        [NotMapped]
        public virtual IList<Image> Images { get; set; }

        public string? Password { get; set; }
        public DateTime TimeStamp { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}