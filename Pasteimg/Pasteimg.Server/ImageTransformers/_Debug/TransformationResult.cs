namespace Pasteimg.Server.ImageTransformers._Debug
{
    public record TransformationResult(ImageInfo Original, ImageInfo Transformed, TimeSpan Duration)
    {
        public float FileSizeDifferenceRatio => MathF.Round(Transformed.FileSize / (float)Original.FileSize, 3);
    }
}