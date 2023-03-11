using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Server.Models;

namespace Pasteimg.Server.Data.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);
            builder.HasOne(i => i.OptimizationResult)
                   .WithOne(o => o.Image)
                   .HasForeignKey(typeof(OptimizationResult), nameof(OptimizationResult.Id))
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}