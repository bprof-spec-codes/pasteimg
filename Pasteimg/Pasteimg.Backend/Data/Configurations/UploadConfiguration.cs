using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Data.Configurations
{

    /// <summary>
    /// Configuration for the <see cref="Upload"/> entity.
    /// </summary>
    public class UploadConfiguration : IEntityTypeConfiguration<Upload>
    {
        /// <inheritdoc/>
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