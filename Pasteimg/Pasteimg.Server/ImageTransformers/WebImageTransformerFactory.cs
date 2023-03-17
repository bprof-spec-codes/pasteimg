using Pasteimg.Server.Configurations;

namespace Pasteimg.Server.Transformers
{
    public interface IWebImageTransformerFactory
    {
        WebImageTransformer CreateSourceOptimizer(TransformationConfiguration transformationConfig);

        WebImageTransformer CreateThumbnailCreator(TransformationConfiguration transformationConfig);
    }

    public class WebImageTransformerFactory : IWebImageTransformerFactory
    {
        public WebImageTransformer CreateSourceOptimizer(TransformationConfiguration transformationConfig)
        {
            if (transformationConfig.SourceOptimizerMaxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.SourceOptimizerMaxWidth));
            }
            if (transformationConfig.SourceOptimizerMaxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.SourceOptimizerMaxHeight));
            }
            if (transformationConfig.Quality < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.Quality));
            }
            return new SourceOptimizer(transformationConfig.SourceOptimizerMaxWidth,
                                       transformationConfig.SourceOptimizerMaxHeight,
                                       transformationConfig.Quality);
        }

        public WebImageTransformer CreateThumbnailCreator(TransformationConfiguration transformationConfig)
        {
            if (transformationConfig.SourceOptimizerMaxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.ThumbnailMaxWidth));
            }
            if (transformationConfig.SourceOptimizerMaxHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.ThumbnailMaxHeight));
            }
            if (transformationConfig.Quality < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transformationConfig.Quality));
            }
            return new ThumbnailCreator(transformationConfig.ThumbnailMaxWidth,
                                        transformationConfig.ThumbnailMaxHeight,
                                        transformationConfig.Quality);
        }
    }
}