using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Data.Configurations
{
    /// <summary>
    /// Represents the configuration for the Upload entity in the Pasteimg application's database context.
    /// </summary>
    public class UploadConfiguration : IEntityTypeConfiguration<Upload>
    {
        public void Configure(EntityTypeBuilder<Upload> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasMany(u => u.Images)
                   .WithOne(i => i.Upload)
                   .HasForeignKey(i => i.UploadId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(u => u.TimeStamp);
        }
    }
}