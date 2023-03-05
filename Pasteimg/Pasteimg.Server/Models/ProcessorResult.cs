using System.Drawing;
using System.Drawing.Imaging;

namespace Pasteimg.Server.Models
{


    public class ProcessorResult
    {
        public string? Name { get; set; }
        public Size Size { get; set; }
        public byte[]? Content { get; set; }
        public Exception? Error { get; set; }
        public ImageFormat? Format { get; set; }
        public string GetExtensionFromFormat()
        {
            string? extension=ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == Format?.Guid)?.FilenameExtension;
            if (extension == null)
            {
                throw new InvalidDataException(Format?.ToString());
            }
            return extension;
        }
    }
    public class CompressionResult : ProcessorResult
    {
        public Size OriginalSize { get; set; }
        public long OriginalLength { get; set; }
        public long CompressedLength => Content?.Length ?? OriginalLength;
        public float CompressionRatio => MathF.Round(CompressedLength / (float)OriginalLength, 3);
        public ImageFormat? OriginalFormat { get; set; }

    };
}