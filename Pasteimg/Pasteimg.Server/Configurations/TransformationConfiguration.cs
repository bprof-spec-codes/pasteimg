namespace Pasteimg.Server.Configurations
{
    public class TransformationConfiguration
    {
        public int Quality { get; init; }
        public int SourceOptimizerMaxHeight { get; init; }
        public int SourceOptimizerMaxWidth { get; init; }
        public int ThumbnailMaxHeight { get; init; }
        public int ThumbnailMaxWidth { get; init; }
    }
}