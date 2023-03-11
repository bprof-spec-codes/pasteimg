using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Server.Models;

namespace Pasteimg.Server.Data.Configurations
{
    public class UploadConfiguration : IEntityTypeConfiguration<Upload>
    {
        public void Configure(EntityTypeBuilder<Upload> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasMany(u => u.Images)
                   .WithOne(i => i.Upload)
                   .HasForeignKey(i => i.UploadID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(u => u.TimeStamp);
        }
    }
}