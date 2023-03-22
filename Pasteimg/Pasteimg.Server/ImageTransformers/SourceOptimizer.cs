using ImageMagick;

namespace Pasteimg.Server.ImageTransformers
{
    public class SourceOptimizer : WebImageTransformer
    {
        public SourceOptimizer(int maxWidth, int maxHeight, int quality) : base(maxWidth, maxHeight, quality)
        {
        }


        protected override void TransformAnimated((int width, int height) newSize, MagickImageCollection frames)
        {
            foreach (var frame in frames)
            {
                frame.Strip();
                frame.Quality = Quality;
                frame.Sample(newSize.width, newSize.height);
                frame.ColorFuzz = new Percentage(15);
            }
            frames.OptimizePlus();
        }

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