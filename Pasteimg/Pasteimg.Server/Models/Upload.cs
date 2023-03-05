using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteimg.Server.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Pasteimg.Server.Models
{
    public class UploadBase
    {
        public virtual string? Id { get; set; }
        public string? Password { get; set; }
        public DateTime TimeStamp { get; set; }
    }


    public class Upload : UploadBase, IEntity
    {
        public Upload()
        {
            Images = new List<Image>();
        }

        [Key]
        public override string? Id { get; set; }

        [NotMapped]
        public virtual List<Image> Images { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}
