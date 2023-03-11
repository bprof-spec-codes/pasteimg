using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteimg.Server.Models
{
    public class OptimizationResult:IEntity
    {
        [ValidateNever, StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public string SourceExtension { get; set; }
        public long SourceFileSize { get; set; }
        public int OptimizedWidth { get; set; }
        public int OptimizedHeight { get; set; }
        public string OptimizedExtension { get; set; }
        public TimeSpan Duration { get; set; }
        public long OptimizedFileSize { get; set; }
        public int UsedQuality { get; set; }
        [NotMapped]
        public float FileSizeDifferenceRatio => MathF.Round(OptimizedFileSize / (float)SourceFileSize, 4);
        [NotMapped,JsonIgnore]
        public byte[] Content { get; set; }
        public virtual Image? Image { get; set; }
        public object[] GetKey()
        {
            return new object[] { Id };
        }
    }

}