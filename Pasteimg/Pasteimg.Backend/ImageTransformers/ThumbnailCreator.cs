using ImageMagick;

namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// A transformer that creates a thumbnail image of a given maximum size.
    /// </summary>
    public class ThumbnailCreator : ImageTransformer
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ThumbnailCreator"/> class with specified settings.
        /// </summary>
        /// <param name="settings">Settings of the transformer</param>
        public ThumbnailCreator(TransformationSettings settings) : base(settings)
        { }

        /// <summary>
        /// Transforms an animated image to thumbnail by resizing it, stripping metadata, reducing its color depth and optimizing it.
        /// </summary>
        /// <param name="maxSize">The maximum size of the thumbnail.</param>
        /// <param name="frames">The frames of the animated image.</param>
        protected override void TransformAnimated((int width, int height) maxSize, MagickImageCollection frames)
        {
            var quantizeSettings = new QuantizeSettings() { Colors = 255 };
            foreach (var frame in frames)
            {
                frame.Strip();
                frame.Quality = Quality;
                frame.Sample(maxSize.width, maxSize.height);
                frame.ColorFuzz = new Percentage(30);
            }
            frames.Optimize();
        }

        /// <summary>
        /// Transforms a static image to thumbnail by resizing it, stripping metadata, reducing its color depth, converting it to the WebP format, and optimizing it.
        /// </summary>
        /// <param name="newSize">The new size of the transformed image.</param>
        /// <param name="frames">The collection of frames in the static image.</param>
        protected override void TransformStatic((int width, int height) maxSize, MagickImageCollection frames)
        {
            var firstFrame = frames[0];
            firstFrame.Strip();
            firstFrame.Quality = Quality;
            firstFrame.Format = MagickFormat.WebP;
            firstFrame.Sample(maxSize.width, maxSize.height);
            firstFrame.ColorFuzz = new Percentage(30);
            frames.Optimize();
        }
    }
}