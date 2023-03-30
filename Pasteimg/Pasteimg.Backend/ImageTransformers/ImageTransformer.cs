using ImageMagick;

namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// Defines the interface for an image transformer.
    /// </summary>
    public interface IImageTransformer
    {
        /// <summary>
        /// Gets the maximum height of the image.
        /// </summary>
        int MaxHeight { get; }

        /// <summary>
        /// Gets the maximum width of the image.
        /// </summary>
        int MaxWidth { get; }

        /// <summary>
        /// Gets the quality of the image.
        /// </summary>
        int Quality { get; }

        /// <summary>
        /// Gets information about an image from its byte content.
        /// </summary>
        /// <param name="content">The byte content of the image.</param>
        /// <returns>An <see cref="ImageInfo"/> object containing information about the image.</returns>
        ImageInfo GetImageInfo(byte[] content);

        /// <summary>
        /// Gets information about an image from its file path.
        /// </summary>
        /// <param name="path">The file path of the image.</param>
        /// <returns>An <see cref="ImageInfo"/> object containing information about the image.</returns>
        ImageInfo GetImageInfo(string path);

        /// <summary>
        /// Transforms an image from its byte content.
        /// </summary>
        /// <param name="content">The byte content of the image.</param>
        /// <returns>The transformed image as a byte array.</returns>
        byte[] Transform(byte[] content);
        /// <summary>
        /// Transforms an image from its byte content and saves it to a file.
        /// </summary>
        /// <param name="content">The byte content of the image.</param>
        /// <param name="outputPath">The file path to save the transformed image to.</param>
        /// <returns>The file path of the saved image.</returns>
        string Transform(byte[] content, string outputPath);
    }

    /// <summary>
    /// Defines the abstract base class for image transforming.
    /// </summary>
    public abstract class ImageTransformer : IImageTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTransformer"/> class with the specified settings.
        /// </summary>
        /// <param name="settings">The settings of the transformer</param>
        public ImageTransformer(TransformationSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.MaxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.MaxWidth));
            }

            if (settings.MaxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.MaxHeight));
            }

            if (settings.Quality < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.Quality));
            }
            MaxWidth = settings.MaxWidth;
            MaxHeight = settings.MaxHeight;
            Quality = settings.Quality;
        }

        /// <inheritdoc/>
        public int MaxHeight { get; }

        /// <inheritdoc/>
        public int MaxWidth { get; }

        /// <inheritdoc/>
        public int Quality { get; }

        /// <inheritdoc/>
        public virtual ImageInfo GetImageInfo(byte[] content)
        {
                var magickInfo = MagickImageInfo.ReadCollection(content).First();
                return new ImageInfo(magickInfo.Width, magickInfo.Height, content.LongLength, magickInfo.Format.ToString().ToLower(), null);
        }

        /// <inheritdoc/>
        public virtual ImageInfo GetImageInfo(string path)
        {
            var fileInfo = new FileInfo(path);
            var magickInfo = MagickImageInfo.ReadCollection(fileInfo).First();
            return new ImageInfo(magickInfo.Width, magickInfo.Height, fileInfo.Length, magickInfo.Format.ToString().ToLower(), path);
        }

        /// <inheritdoc/>
        public byte[] Transform(byte[] content)
        {
            using MagickImageCollection frames = new MagickImageCollection(content);
            TransformMethod(frames);
            return frames.ToByteArray();
        }
        /// <inheritdoc/>

        public string Transform(byte[] content, string outputPath)
        {
            using MagickImageCollection frames = new MagickImageCollection(content);
            TransformMethod(frames);
            string outputFormat = frames[0].Format.ToString().ToLower();
            string path = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + "." + outputFormat);
            frames.Write(path);
            return path;
        }
        /// <summary>
        /// Clamps the size of an image to the maximum dimensions specified when the image transformer was instantiated.
        /// </summary>
        /// <param name="width">The original width of the image.</param>
        /// <param name="height">The original height of the image.</param>
        /// <returns>A tuple containing the clamped width and height.</returns>
        protected virtual (int width, int height) ClampSize(float width, float height)
        {
            if (width > MaxWidth)
            {
                float wratio = MaxWidth / width;
                width *= wratio;
                height *= wratio;
            }

            if (height > MaxHeight)
            {
                float hratio = MaxHeight / height;
                width *= hratio;
                height *= hratio;
            }

            return new((int)Math.Round(width, 0), (int)Math.Round(height, 0));
        }

        /// <summary>
        /// Transforms an animated image.
        /// </summary>
        /// <param name="maxSize">The maximum size to transform the image into.</param>
        /// <param name="frames">The MagickImageCollection object containing the image to transform.</param>
        protected abstract void TransformAnimated((int width, int height) maxSize, MagickImageCollection frames);

        /// <summary>
        /// Transforms the image using the specified transformation method. If the image is animated, applies the transformation to each frame of the animation.
        /// </summary>
        /// <param name="frames">The MagickImageCollection object containing the image to transform.</param>
        protected virtual void TransformMethod(MagickImageCollection frames)
        {
            var firstFrame = frames[0];
            var maxSize = ClampSize(firstFrame.Width, firstFrame.Height);
            if (frames.Count == 1)
            {
                TransformStatic(maxSize, frames);
            }
            else
            {
                TransformAnimated(maxSize, frames);
            }
        }

        /// <summary>
        /// Transforms a static image.
        /// </summary>
        /// <param name="maxSize">The maximum size to transform the image into.</param>
        /// <param name="frames">The MagickImageCollection object containing the image to transform.</param>
        protected abstract void TransformStatic((int width, int height) maxSize, MagickImageCollection frames);
    }
}