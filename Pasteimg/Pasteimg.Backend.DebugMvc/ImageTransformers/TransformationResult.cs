using Pasteimg.Backend.ImageTransformers;

namespace Pasteimg.Backend.DebugMvc.ImageTransformers
{
    public record TransformationResult(ImageInfo Original, ImageInfo Transformed, TimeSpan Duration)
    {
        public float FileSizeDifferenceRatio => MathF.Round(Transformed.FileSize / (float)Original.FileSize, 3);
    }
}