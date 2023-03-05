using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Server.Models;

namespace Pasteimg.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public virtual DbSet<Upload> Uploads { get; set; }
        public virtual DbSet<Image> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}