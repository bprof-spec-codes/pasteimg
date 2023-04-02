using Microsoft.EntityFrameworkCore;
using Pasteimg.Backend.Models;

namespace Pasteimg.Backend.Data
{
    /// <summary>
    /// Represents a DbContext for the PasteImg application that provides access to the database tables for <see cref="Admin"/>, <see cref="Image"/>, and <see cref="Upload"/>.
    /// </summary>
    public class PasteImgDbContext : DbContext
    {
        private IPasswordHasher hasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasteImgDbContext"/> class with the specified password hasher and DbContext options.
        /// </summary>
        /// <param name="hasher">The password hasher implementation to use for hashing admin passwords.</param>
        /// <param name="options">The DbContext options to use for configuring the database connection.</param>

        public PasteImgDbContext(IPasswordHasher hasher, DbContextOptions<PasteImgDbContext> options)
            : base(options)
        {
            this.hasher = hasher;
        }
        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Admin"/> entity in the database.
        /// </summary>
        public virtual DbSet<Admin> Admins { get; set; }
        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Image"/> entity in the database.
        /// </summary>
        public virtual DbSet<Image> Images { get; set; }
        /// <summary>
        /// Gets or sets the DbSet for the <see cref="Upload"/> entity in the database.
        /// </summary>
        public virtual DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetAssembly(typeof(PasteImgDbContext)));
            builder.Entity<Admin>().HasData(new Admin
            {
                Email = "admin@admin.com",
                Password = hasher.CreateHash("123Admin456")
            });

            base.OnModelCreating(builder);
        }
    }
}