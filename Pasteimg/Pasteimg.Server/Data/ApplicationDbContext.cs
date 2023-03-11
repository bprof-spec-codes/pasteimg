using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Server.Models;
using System.Runtime.CompilerServices;

namespace Pasteimg.Server.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetAssembly(typeof(ApplicationDbContext)));
        }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Upload> Uploads { get; set; }
        public virtual DbSet<OptimizationResult> OptimizationResults { get; set; }
    }
}