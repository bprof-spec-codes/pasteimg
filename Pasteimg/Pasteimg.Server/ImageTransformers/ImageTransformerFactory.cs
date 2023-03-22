using Pasteimg.Server.Configurations;

namespace Pasteimg.Server.ImageTransformers
{
    public interface IImageTransformerFactory
    {
        IImageTransformer CreateSourceOptimizer();
        IImageTransformer CreateThumbnailCreator();
    }

    public class ImageTransformerFactory : IImageTransformerFactory
    {
        TransformationConfiguration config;
        public ImageTransformerFactory(PasteImgConfiguration configuration)
        {
            this.config =configuration.Transformation;
        }
        public IImageTransformer CreateSourceOptimizer()
        {
            if (config.SourceOptimizerMaxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.SourceOptimizerMaxWidth));
            }
            if (config.SourceOptimizerMaxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.SourceOptimizerMaxHeight));
            }
            if (config.SourceQuality < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.SourceQuality));
            }
            return new SourceOptimizer(config.SourceOptimizerMaxWidth,
                                       config.SourceOptimizerMaxHeight,
                                       config.SourceQuality);
        }

        public IImageTransformer CreateThumbnailCreator()
        {
            if (config.ThumbnailMaxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.ThumbnailMaxWidth));
            }
            if (config.ThumbnailMaxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.ThumbnailMaxHeight));
            }
            if (config.ThumbnailQuality < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.ThumbnailQuality));
            }
            return new ThumbnailCreator(config.ThumbnailMaxWidth,
                                        config.ThumbnailMaxHeight,
                                        config.ThumbnailQuality);
        }
    }
}