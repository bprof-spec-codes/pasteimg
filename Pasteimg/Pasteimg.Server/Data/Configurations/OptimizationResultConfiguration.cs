using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pasteimg.Server.Models;

namespace Pasteimg.Server.Data.Configurations
{
    public class OptimizationResultConfiguration : IEntityTypeConfiguration<OptimizationResult>
    {
        public void Configure(EntityTypeBuilder<OptimizationResult> builder)
        {
            builder.HasKey(o => o.Id);
        }
    }
}