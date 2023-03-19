using ImageMagick;

namespace Pasteimg.Server.Transformers
{
    public class SourceOptimizer : WebImageTransformer
    {
        public SourceOptimizer(int maxWidth, int maxHeight, int quality) : base(maxWidth, maxHeight, quality)
        {
        }

        public override string Transform(string path)
        {
            using MagickImageCollection frames = new MagickImageCollection(path);
            string inputFormat = Path.GetExtension(path).TrimStart('.').ToLower();

            TransformMethod(frames);
            MagickFormat outputFormat = frames[0].Format;
            path = Path.ChangeExtension(path, outputFormat.ToString().ToLower());
            frames.Write(path);

            if (inputFormat != outputFormat.ToString().ToLower())
            {
                File.Delete(path);
            }
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
            var firstFrame = frames[0];
            var newSize = ClampSize(firstFrame.Width, firstFrame.Height);
            foreach (var frame in frames)
            {
                frame.Quality = Quality;
                frame.AdaptiveResize(newSize.width, newSize.height);
                frame.ColorFuzz = new Percentage(20);
                frame.Strip();
            }
            frames.Optimize();
        }
    }
}