using Pasteimg.Server.Configurations;
using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Repository.FileStorages
{
    public interface IImageFileRepositoryFactory
    {
        IImageFileRepository CreateImageFileRepository(StorageConfiguration storageConfig);

        IFileStorage CreateSourceStorage(StorageConfiguration storageConfig);

        IFileStorage CreateThumbnailStorage(StorageConfiguration storageConfig);
    }

    public class ImageFileRepositoryFactory : IImageFileRepositoryFactory
    {
        private readonly IRepository<Image> repository;

        public ImageFileRepositoryFactory(IRepository<Image> repository)
        {
            this.repository = repository;
        }

        public IImageFileRepository CreateImageFileRepository(StorageConfiguration storageConfig)
        {
            return new ImageFileRepository(repository, CreateSourceStorage(storageConfig), CreateThumbnailStorage(storageConfig));
        }

        public virtual IFileStorage CreateSourceStorage(StorageConfiguration storageConfig)
        {
            if (storageConfig.ThumbnailRoot is null)
            {
                throw new ArgumentNullException(nameof(storageConfig.ThumbnailRoot));
            }

            return new FileStorage(storageConfig.SourceRoot, storageConfig.SubDirectoryDivision);
        }

        public virtual IFileStorage CreateThumbnailStorage(StorageConfiguration storageConfig)
        {
            if (storageConfig.SourceRoot is null)
            {
                throw new ArgumentNullException(nameof(storageConfig.SourceRoot));
            }

            return new FileStorage(storageConfig.ThumbnailRoot, storageConfig.SubDirectoryDivision);
        }
    }
}