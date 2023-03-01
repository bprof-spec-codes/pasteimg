using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pasteimg.Server.Models
{
    [Index(nameof(GroupKey), IsUnique = false)]
    [Index(nameof(Timestamp), IsUnique = false)]
    public class BinaryImage
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }
        [Required]
        public byte[] Content { get; set; }
        public byte[]? Thumbnail { get; set; }
        public bool IsNsfw { get; set; }
        public string? Password { get; set; }
        [StringLength(120)]
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime LastAccess { get; set; }
        public string? GroupKey { get; set; }
    }
}