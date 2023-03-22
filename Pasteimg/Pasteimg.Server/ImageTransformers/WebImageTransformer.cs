using ImageMagick;

namespace Pasteimg.Server.ImageTransformers
{
    public abstract class WebImageTransformer : IImageTransformer
    {
        public WebImageTransformer(int maxWidth, int maxHeight, int quality)
        {
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Quality = quality;
        }

        public int MaxHeight { get; }
        public int MaxWidth { get; }
        public int Quality { get; }

        public virtual ImageInfo GetImageInfo(byte[] content)
        {
            var magickInfo = MagickImageInfo.ReadCollection(content).First();
            return new ImageInfo(magickInfo.Width, magickInfo.Height, content.LongLength, magickInfo.Format.ToString().ToLower(), null);
        }

        public virtual ImageInfo GetImageInfo(string path)
        {
            var fileInfo = new FileInfo(path);
            var magickInfo = MagickImageInfo.ReadCollection(fileInfo).First();
            return new ImageInfo(magickInfo.Width, magickInfo.Height, fileInfo.Length, magickInfo.Format.ToString().ToLower(), path);
        }

        public byte[] Transform(byte[] content)
        {
            using MagickImageCollection frames = new MagickImageCollection(content);
            TransformMethod(frames);
            return frames.ToByteArray();
        }

        public string Transform(byte[] content, string outputPath)
        {
            using MagickImageCollection frames = new MagickImageCollection(content);
            TransformMethod(frames);
            string outputFormat = frames[0].Format.ToString().ToLower();
            string path = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + "." + outputFormat);
            frames.Write(path);
            return path;
        }

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

        protected abstract void TransformAnimated((int width, int height) newSize, MagickImageCollection frames);

        protected virtual void TransformMethod(MagickImageCollection frames)
        {
            var firstFrame = frames[0];
            var newSize = ClampSize(firstFrame.Width, firstFrame.Height);
            if (frames.Count == 1)
            {
                TransformStatic(newSize, frames);
            }
            else
            {
                TransformAnimated(newSize, frames);
            }
        }

        protected abstract void TransformStatic((int width, int height) newSize, MagickImageCollection frames);
    }
}