using ImageMagick;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using Tensorflow.Keras.Engine;

namespace Pasteimg.Server.Logic
{


    public interface IBinaryHandler
    {
        Size MaxSize { get; set; }
        Size MaxThumbnailSize { get; set; }
        Quality Quality { get; set; }

        byte[] Compress(byte[] binary);
        byte[] CreateThumbnail(byte[] binary);
    }


    public enum Quality
    {
        Good = 0, Normal = 1, Bad = 2, Perfect = 3
    }
    public class BinaryHandler : IBinaryHandler
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
            if (width > maxWidth || height > maxHeight)
            {
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
            }

            return new Size((int)width, (int)height);
        }
        public byte[] CreateThumbnail(byte[] binary)
        {
            using (MemoryStream originalStream = new MemoryStream(binary))
            {
                using(MemoryStream destination=new MemoryStream())
                {
                    using (Image image = Image.FromStream(originalStream))
                    {
                        Size newSize = ClampSize(image.Size, MaxThumbnailSize.Width, MaxThumbnailSize.Height);
                        if (newSize != image.Size)
                        {
                            using (Bitmap bitmap = new Bitmap(image,newSize))
                            {
                                bitmap.Save(destination,ImageFormat.Jpeg);
                            }
                        }
                        else
                        {
                            image.Save(destination, ImageFormat.Jpeg);
                        }
                        return destination.ToArray();
                    }
                }
            }
        }
        public byte[] Compress(byte[] binary)
        {
            using (MemoryStream originalStream = new MemoryStream(binary))
            {
                using(MemoryStream destination=new MemoryStream())
                {
                    using (Image image = Image.FromStream(originalStream))
                    {

                        if (image.FrameDimensionsList.Contains(FrameDimension.Time.Guid) && image.GetFrameCount(FrameDimension.Time) > 1)
                        {
                            image.Save(destination, image.RawFormat);
                            return destination.ToArray();
                        }
                        else
                        {
                            Size newSize = ClampSize(image.Size, MaxSize.Width, MaxSize.Height);
                            bool hasAlpha = false;
                            using (Bitmap bitmap = new Bitmap(image,newSize))
                            {
                                bitmap.Save(destination, ImageFormat.Jpeg);
                                hasAlpha = HasAlpha(bitmap);
                            }
                            if (!hasAlpha)
                            {

                                ImageCodecInfo? jpeg = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                EncoderParameters parameters = new EncoderParameters(1);
                                parameters.Param[0] = new EncoderParameter(Encoder.Quality, ConvertJpegQuality());
                                image.Save(destination, jpeg, parameters);
                                return destination.ToArray();
                            }
                            else
                            {
                                image.Save(destination, image.RawFormat);
                                return destination.ToArray();
                            }
                        }
                    }
                }
             
            }
        }
        private bool HasAlpha(Bitmap bitmap)
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

    }

}