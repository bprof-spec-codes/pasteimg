using ImageMagick;
using ImageMagick.ImageOptimizers;
using Pasteimg.Server.Models;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Pasteimg.Server.Repository.IFileStorage
{
    public interface IImageOptimizer
    {
        OptimizationResult Optimize(string id,byte[] content, int maxWidth, int maxHeight, int quality);
    }
    public class ImageCompressorAndResizer : ImageOptimizer
    {
        public override OptimizationResult Optimize(string id,byte[] content, int maxWidth, int maxHeight, int quality)
        {

            return Transfrom(id,content, (frames) =>
            {

                var firstFrame = frames[0];
                var newSize = ClampSize(firstFrame.Width, firstFrame.Height, maxWidth, maxHeight);
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].Quality = quality;
                    frames[i].AdaptiveResize(newSize.width, newSize.height);
                    frames[i].ColorFuzz = new Percentage(30);
                }
                frames.Optimize();

            });
        }
    }

    public class ThumbnailCreator : ImageOptimizer
    {
        public override OptimizationResult Optimize(string id,byte[] content, int maxWidth, int maxHeight, int quality)
        {
            return Transfrom(id,content, (frames) =>
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].Quality = quality;
                    frames[i].Thumbnail(maxWidth, maxHeight);

                }
            });
        }
    }


    public abstract class ImageOptimizer : IImageOptimizer
    {

        public abstract OptimizationResult Optimize(string id,byte[] content, int maxWidth, int maxHeight, int quality);
   
        protected (int width, int height) ClampSize(float width, float height, int maxWidth, int maxHeight)
        {
            if (width > maxWidth)
            {
                float wratio = maxWidth / width;
                width *= wratio;
                height *= wratio;
            }

            if (height > maxHeight)
            {
                float hratio = maxHeight / height;
                width *= hratio;
                height *= hratio;
            }

            return new((int)Math.Round(width, 0), (int)Math.Round(height, 0));
        }
     
        protected void SetOriginal(OptimizationResult result,string id, byte[] content, MagickImageCollection frames)
        {
            result.Id = id;
            result.SourceWidth = frames[0].Width;
            result.SourceHeight = frames[0].Height;
            result.SourceExtension = frames[0].Format.ToString().ToLower();
            result.SourceFileSize = content.Length;
        }
        protected void SetOriginalToOptimized(OptimizationResult result, byte[] content,TimeSpan duration)
        {
            result.OptimizedWidth = result.SourceWidth;
            result.OptimizedHeight = result.SourceHeight;
            result.OptimizedExtension = result.SourceExtension;
            result.OptimizedFileSize = result.SourceFileSize;
            result.Content = content;
            result.Duration = duration;
            result.UsedQuality = -1;
        }
        protected void SetOptimized(OptimizationResult result,byte[] content,MagickImageCollection frames,TimeSpan duration)
        {

            result.OptimizedWidth = frames[0].Width;
            result.OptimizedHeight = frames[0].Height;
            result.OptimizedExtension = frames[0].Format.ToString().ToLower();
            result.Content = content;
            result.OptimizedFileSize = result.Content.Length;
            result.Duration = duration;
            result.UsedQuality = frames[0].Quality;
        }
     
        protected OptimizationResult Transfrom(string id,byte[] content, Action<MagickImageCollection> transform)
        {
            OptimizationResult result = new OptimizationResult();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using MagickImageCollection frames = new MagickImageCollection(content);
            SetOriginal(result,id, content, frames);
            transform(frames);
            sw.Stop();
            byte[] optimized = frames.ToByteArray();
            if(optimized.Length<=content.Length)
            {
               SetOptimized(result,optimized, frames, sw.Elapsed);
            }
            else
            {
                SetOriginalToOptimized(result, content, sw.Elapsed);
            }
            return result;
        }

    


    
    }
}