using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Data.Configurations
{
    /// <summary>
    /// Configuration for the <see cref="Admin"/> entity.
    /// </summary>
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(a => a.Email);
        }
    }
}