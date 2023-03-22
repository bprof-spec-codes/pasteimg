namespace Pasteimg.Server.Configurations
{
    public class TransformationConfiguration
    {
        public int SourceQuality { get; init; }
        public int SourceOptimizerMaxHeight { get; init; }
        public int SourceOptimizerMaxWidth { get; init; }
        public int ThumbnailQuality { get; init; }
        public int ThumbnailMaxHeight { get; init; }
        public int ThumbnailMaxWidth { get; init; }
    }
}