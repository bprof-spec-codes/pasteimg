using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
               new IdentityRole()
               {
                   Id = "1",
                   Name = "Admin",
                   NormalizedName = "ADMIN"
               });

            PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
            var admin = new IdentityUser
            {
                Id = "1",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
            };

            admin.PasswordHash = hasher.HashPassword(admin, "123Admin456");
            builder.Entity<IdentityUser>().HasData(admin);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            {
                RoleId = "1",
                UserId = "1"
            });
            builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetAssembly(typeof(ApplicationDbContext)));
            base.OnModelCreating(builder);
        }
    }
}