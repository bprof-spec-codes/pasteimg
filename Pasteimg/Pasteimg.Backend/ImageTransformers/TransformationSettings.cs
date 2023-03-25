namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// Represents settings for image transformation.
    /// </summary>
    public class TransformationSettings
    {
        /// <summary>
        /// Gets or sets the maximum height for image transformation.
        /// </summary>
        public int MaxHeight { get; init; }

        /// <summary>
        /// Gets or sets the maximum width for image transformation.
        /// </summary>
        public int MaxWidth { get; init; }

        /// <summary>
        /// Gets or sets the quality for image transformation.
        /// </summary>
        public int Quality { get; init; }
    }
}