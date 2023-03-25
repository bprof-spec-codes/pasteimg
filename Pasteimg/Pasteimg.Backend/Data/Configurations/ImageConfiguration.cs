using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Data.Configurations
{
    /// <summary>
    /// Represents the configuration for the Image entity in the Pasteimg application's database context.
    /// </summary>
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);
        }
    }
}