using ImageMagick;

namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// An image transformer that optimizes the source image for web use.
    /// </summary>
    public class SourceOptimizer : ImageTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceOptimizer"/> class with  specified settings.
        /// </summary>
        /// <param name="settings">Settings of the transformer</param>
        public SourceOptimizer(TransformationSettings settings) : base(settings)
        {
        }

        /// <summary>
        /// Transforms an animated image by resizing it, stripping metadata, reducing its color depth, and optimizing it for web use.
        /// </summary>
        /// <param name="maxSize">The maximum size of the transformed image.</param>
        /// <param name="frames">The collection of frames in the animated image.</param>
        protected override void TransformAnimated((int width, int height) maxSize, MagickImageCollection frames)
        {
            foreach (var frame in frames)
            {
                frame.Strip();
                frame.Quality = Quality;
                frame.Sample(maxSize.width, maxSize.height);
                frame.ColorFuzz = new Percentage(15);
            }
            frames.OptimizePlus();
        }

        /// <summary>
        /// Transforms a static image by resizing it, stripping metadata, reducing its color depth, converting it to the WebP format, and optimizing it for web use.
        /// </summary>
        /// <param name="newSize">The maximum size of the transformed image.</param>
        /// <param name="frames">The collection of frames in the static image.</param>
        protected override void TransformStatic((int width, int height) newSize, MagickImageCollection frames)
        {
            var firstFrame = frames[0];
            firstFrame.Strip();
            firstFrame.Quality = Quality;
            firstFrame.Format = MagickFormat.WebP;
            firstFrame.Sample(newSize.width, newSize.height);
            firstFrame.ColorFuzz = new Percentage(15);
            frames.Optimize();
        }
    }
}