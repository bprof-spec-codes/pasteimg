using System.Text;

namespace Pasteimg.Server.Repository.IFileStorage
{
    public interface IImageFileStorageFactory
    {
        IImageFileStorage CreateImagesStorage(string root,int maxWidth, int maxHeight);
        IImageFileStorage CreateThumbnailsStorage(string root,int maxWidth, int maxHeight);
    }

    public class ImageFileStorageFactory : IImageFileStorageFactory
    {
        const int SubDirectoryDivision = 2;
        protected readonly IWebHostEnvironment environment;
        public ImageFileStorageFactory(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public IImageFileStorage CreateThumbnailsStorage(string root,int maxWidth, int maxHeight)
        {
            return new ImageFileStorage(environment,root, SubDirectoryDivision, maxWidth, maxHeight, new ThumbnailCreator(), 75);
        }

        public IImageFileStorage CreateImagesStorage(string root,int maxWidth,int maxHeight)
        {
            return new ImageFileStorage(environment,root, SubDirectoryDivision, maxWidth, maxHeight, new ImageCompressorAndResizer(), 75);
        }
    }
}