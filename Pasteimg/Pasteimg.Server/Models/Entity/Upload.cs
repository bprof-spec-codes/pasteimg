using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Server.Models.Entity
{
    /// <summary>
    /// Képfeltöltést reprezentáló modell.
    /// </summary>
    [ModelBinder(typeof(UploadModelBinder))]
    public class Upload : IEntity
    {
        public Upload()
        {
            Images = new List<Image>();
        }

        /// <summary>
        /// Feltöltés egyedi azonosító kulcsa.
        /// </summary>
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        /// <summary>
        /// Feltöltéshez tartozó képeknek navigációs propertyje.
        /// </summary>
        public virtual IList<Image> Images { get; set; }

        /// <summary>
        /// Feltöltő által megadott, hashelt jelszó.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Feltöltés időpontja.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }
}