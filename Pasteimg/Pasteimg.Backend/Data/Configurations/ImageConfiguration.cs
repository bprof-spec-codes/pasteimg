using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Data.Configurations
{
    /// <summary>
    /// Configuration for the <see cref="Image"/> entity.
    /// </summary>
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);
        }
    }
}