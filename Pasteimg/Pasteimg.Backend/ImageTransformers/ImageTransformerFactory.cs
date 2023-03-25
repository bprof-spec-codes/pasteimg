namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// Interface for creating <see cref="ImageTransformer"/>s.
    /// </summary>
    public interface IImageTransformerFactory
    {
        /// <summary>
        /// Creates a source optimizer <see cref="ImageTransformer"/>.
        /// </summary>
        /// <param name="settings">The settings of the transformer</param>
        /// <returns>A new source optimizer  <see cref="ImageTransformer"/></returns>
        ImageTransformer CreateSourceOptimizer(TransformationSettings settings);

        /// <summary>
        /// Creates a thumbnail creator <see cref="ImageTransformer"/>.
        /// </summary>
        /// <param name="settings">The settings of the transformer</param>
        /// <returns>A new thumbnail creator <see cref="ImageTransformer"/></returns>
        ImageTransformer CreateThumbnailCreator(TransformationSettings settings);
    }

    /// <summary>
    /// Factory class for creating <see cref="ImageTransformer"/>s.
    /// </summary>
    public class ImageTransformerFactory : IImageTransformerFactory
    {
        private TransformationSettings config;

        /// <summary>
        /// Creates a new <see cref="SourceOptimizer"/> <see cref="ImageTransformer"/> with the specified settings.
        /// </summary>
        /// <param name="settings">Settings of the transformer</param>
        /// <returns>A new <see cref="SourceOptimizer"/> <see cref="ImageTransformer"/>.</returns>
        public ImageTransformer CreateSourceOptimizer(TransformationSettings settings)
        {
            return new SourceOptimizer(settings);
        }

        /// <summary>
        /// Creates a new <see cref="ThumbnailCreator"/>  <see cref="ImageTransformer"/>  with the specified settings.
        /// </summary>
        /// <param name="settings">Settings of the transformer</param>
        /// <returns>A new <see cref="ThumbnailCreator"/>  <see cref="ImageTransformer"/>.</returns>
        public ImageTransformer CreateThumbnailCreator(TransformationSettings settings)
        {
            return new ThumbnailCreator(settings);
        }
    }
}