using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models.Entity
{
    /// <summary>
    ///Kép feltöltésével kapcsolatos, alkalmazás szempontjából fontos adatokat tároló modell.
    /// </summary>
    public class Image : IEntity
    {
        /// <summary>
        /// Mind feltöltéskor, mind lekerdezéskor ebben tárolódik ideiglenes a kép tartalma.
        /// </summary>
        [NotMapped]
        public IFormFile? Content { get; set; }

        /// <summary>
        /// Feltöltéskor megadott leírás.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Kép egyedi azonosító kulcsa.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        /// <summary>
        /// Feltöltő felnőtt tartalomnak jelölte-e.
        /// </summary>
        public bool NSFW { get; set; }

        /// <summary>
        /// Képhez tartozó feltöltésnek a navigációs propertyje.
        /// </summary>
        [JsonIgnore]
        public virtual Upload Upload { get; set; }

        /// <summary>
        /// Feltöltés idegenkulcsa. Feltöltés egy a több kapcsolatban áll a képekkel.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string UploadID { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}