using ImageMagick;

namespace Pasteimg.Server.Transformers
{
    public class ThumbnailCreator : WebImageTransformer
    {
        public ThumbnailCreator(int maxWidth, int maxHeight, int quality) : base(maxWidth, maxHeight, quality)
        { }

        public override string Transform(string path)
        {
            using MagickImageCollection frames = new MagickImageCollection(path);
            TransformMethod(frames);
            frames.Write(path);
            return path;
        }

        public override byte[] Transform(byte[] content)
        {
            using MagickImageCollection frames = new MagickImageCollection(content);
            TransformMethod(frames);
            return frames.ToByteArray();
        }

        private void TransformMethod(MagickImageCollection frames)
        {
            Parallel.ForEach(frames, frame =>
            {
                frame.Quality = Quality;
                frame.Thumbnail(MaxWidth, MaxHeight);
            });
        }
    }
}