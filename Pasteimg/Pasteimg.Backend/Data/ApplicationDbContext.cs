using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Data
{
    /// <summary>
    /// Represents the database context for the Pasteimg application, which extends the IdentityDbContext class provided by ASP.NET Core Identity.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext class with the specified options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Gets or sets the set of Image entities in the database.
        /// </summary>
        public virtual DbSet<Image> Images { get; set; }

        /// <summary>
        /// Gets or sets the set of Upload entities in the database.
        /// </summary>
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