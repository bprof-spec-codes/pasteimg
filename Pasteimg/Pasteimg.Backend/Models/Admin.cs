using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Represents an admin user of the system.
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// The email address of the admin. This field is required, must be a valid email address, and cannot be longer than 320 characters.
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailAddress, MaxLength(320)]
        public string Email { get; set; }

        /// <summary>
        /// The hashed password of the admin. This field is required and cannot be longer than 256 characters.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [PasswordPropertyText(true)]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}