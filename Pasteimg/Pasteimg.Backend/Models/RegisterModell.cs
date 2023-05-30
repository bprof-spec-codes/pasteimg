using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Represents an admin user of the system.
    /// </summary>
    public class RegisterModell
    {
        /// <summary>
        /// New admin's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// New admin's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The one time use register key
        /// </summary>
        public int Key { get; set; }
    }
}
