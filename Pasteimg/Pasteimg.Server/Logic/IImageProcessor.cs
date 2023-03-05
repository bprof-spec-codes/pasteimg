using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using Pasteimg.Server.Models;
using WindowsImage = System.Drawing.Image;
using System.Diagnostics;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Query;

namespace Pasteimg.Server.Logic
{
    public interface IImageProcessor
    {
        Size MaxSize { get; set; }
        Size MaxThumbnailSize { get; set; }
        Quality Quality { get; set; }

        CompressionResult Compress(ImageModel imageModel);
        ProcessorResult CreateThumbnail(ImageModel imageModel);
    }

    public enum Quality
    {
        Good = 0, Normal = 1, Bad = 2, Perfect = 3
    }
    public class ImageProcessor : IImageProcessor
    {
        public Size MaxSize { get; set; } = new Size(2048, 2048);
        public Size MaxThumbnailSize { get; set; } = new Size(200, 200);
        public Quality Quality { get; set; } = Quality.Normal;
        private int ConvertJpegQuality()
        {
            switch (Quality)
            {
                case Quality.Bad: return 25;
                case Quality.Normal: return 50;
                case Quality.Perfect: return 100;
                default: return 75;
            }
        }

        private Size ClampSize(Size imageSize, int maxWidth, int maxHeight)
        {
            double width = imageSize.Width;
            double height = imageSize.Height;

            if (width > maxWidth)
            {
                double wratio = maxWidth / (double)imageSize.Width;
                width *= wratio;
                height *= wratio;
            }

            if (height > maxHeight)
            {
                double hratio = maxHeight / (double)imageSize.Height;
                width *= hratio;
                height *= hratio;
            }

            return new Size((int)width, (int)height);
        }
        public ProcessorResult? CreateThumbnail(ImageModel imageModel)
        {
            if (imageModel.Content is null)
            {
                throw new ArgumentNullException(nameof(imageModel.Content));
            }

            ProcessorResult result = new ProcessorResult()
            {
                Name =imageModel.Content.Name
            };

            try
            {
                using (Stream originalStream = imageModel.Content.OpenReadStream())
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        using (WindowsImage image = WindowsImage.FromStream(originalStream))
                        {
                            result.Format = image.RawFormat;
                            if (image.Size.Width > MaxThumbnailSize.Width || image.Size.Height > MaxThumbnailSize.Height)
                            {
                                Size newSize = ClampSize(image.Size, MaxThumbnailSize.Width, MaxThumbnailSize.Height);
                                result.Size = newSize;
                                if (newSize != image.Size)
                                {
                                    using (Bitmap bitmap = new Bitmap(image, newSize))
                                    {
                                        bitmap.Save(destination, ImageFormat.Jpeg);
                                    }
                                }
                                else
                                {
                                    image.Save(destination, ImageFormat.Jpeg);
                                }
                                result.Format = ImageFormat.Jpeg;
                                result.Content=destination.ToArray();
                            }
                            else
                            {
                                return null;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            return result;

        }
        public CompressionResult Compress(ImageModel imageModel)
        {
            if(imageModel.Content is null)
            {
                throw new ArgumentNullException(nameof(imageModel.Content));
            }

            CompressionResult result=new CompressionResult() { Name=imageModel.Content?.Name};
            try
            {
                using (Stream original = imageModel.Content.OpenReadStream())
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        using (WindowsImage image = WindowsImage.FromStream(original))
                        {
                            CompressLogic(result,original, destination, image);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Content=imageModel.Content.ToArray();
            }
            return result;

            bool HasAlpha(Bitmap bitmap)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        if (bitmap.GetPixel(x, y).A != 255)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            void CompressLogic(CompressionResult result,Stream original, MemoryStream destination, WindowsImage image)
            {
                result.OriginalSize = image.Size;
                result.Size = image.Size;
                result.OriginalFormat = image.RawFormat;
                result.Format = image.RawFormat;
                result.OriginalLength = imageModel.Content.Length;

                if (!(image.FrameDimensionsList.Contains(FrameDimension.Time.Guid) && image.GetFrameCount(FrameDimension.Time) > 1))
                {

                    Size newSize = ClampSize(image.Size, MaxSize.Width, MaxSize.Height);
                    result.Size = newSize;

                    bool hasAlpha = false;
                    using (Bitmap bitmap = new Bitmap(image, newSize))
                    {
                        if (image.Size.Width > MaxSize.Width || image.Size.Height > MaxSize.Height)
                        {
                            bitmap.Save(destination, image.RawFormat);
                        }
                        hasAlpha = HasAlpha(bitmap);
                    }

                    if (!hasAlpha)
                    {

                        ImageCodecInfo? jpeg = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        EncoderParameters parameters = new EncoderParameters(1);
                        parameters.Param[0] = new EncoderParameter(Encoder.Quality, ConvertJpegQuality());
                        image.Save(destination, jpeg, parameters);

                        result.Format = ImageFormat.Jpeg;
                    }
                    else
                    {

                        image.Save(destination, image.RawFormat);
                    }
                }
                result.Content=destination.ToArray();
            }

        }
    }

}