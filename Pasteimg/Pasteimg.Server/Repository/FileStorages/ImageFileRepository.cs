using Pasteimg.Server.Models;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Transformers;

namespace Pasteimg.Server.Repository.FileStorages
{
    public interface IImageFileRepository
    {
        void Create(Image item, WebImageTransformer? imageOptimizer = null, WebImageTransformer? thumbnailCreator = null);

        Image? Delete(string id);

        IFormFile? FindSourceFile(string id);

        IFormFile? FindThumbnailFile(string id);

        string? GetSourcePath(string id);

        string? GetThumbnailPath(string id);

        Image? Read(string id);

        IQueryable<Image> ReadAll();

        void Update(string id, Action<Image> updateAction);
    }

    public class ImageFileRepository : IImageFileRepository
    {
        private IRepository<Image> repository;
        private IFileStorage sourceStorage;
        private IFileStorage thumbnailStorage;

        public ImageFileRepository(IRepository<Image> repository, IFileStorage sources, IFileStorage thumbnails)
        {
            this.repository = repository;
            this.sourceStorage = sources;
            this.thumbnailStorage = thumbnails;
        }

        public void Create(Image item, WebImageTransformer? sourceOptimizer, WebImageTransformer? thumbnailCreator)
        {
            repository.Create(item);
            byte[] content = item.Content.ToArray();
            sourceStorage.SaveFile(sourceOptimizer?.Transform(content) ?? content, item.Id, Path.GetExtension(item.Content.FileName));
            thumbnailStorage.SaveFile(thumbnailCreator?.Transform(content) ?? content, item.Id, Path.GetExtension(item.Content.FileName));
        }

        public Image? Delete(string id)
        {
            if (repository.Delete(id) is Image image)
            {
                sourceStorage.DeleteFile(id);
                thumbnailStorage.DeleteFile(id);
                return image;
            }
            else return null;
        }

        public IFormFile? FindSourceFile(string id)
        {
            if (sourceStorage.FindFile(id) is string path)
            {
                return GetFormFile(id, path);
            }
            else return null;
        }

        public IFormFile? FindThumbnailFile(string id)
        {
            if (thumbnailStorage.FindFile(id) is string path)
            {
                return GetFormFile(id, path);
            }
            else return null;
        }

        public string? GetSourcePath(string id)
        {
            return sourceStorage.FindFile(id);
        }

        public string? GetThumbnailPath(string id)
        {
            return thumbnailStorage.FindFile(id);
        }

        public Image? Read(string id)
        {
            return repository.Read(id);
        }

        public IQueryable<Image> ReadAll()
        {
            return repository.ReadAll();
        }

        public void Update(string id, Action<Image> updateAction)
        {
            repository.Update(updateAction, id);
        }

        private IFormFile? GetFormFile(string id, string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new FormFile(stream, 0, stream.Length, id, Path.GetFileName(path))
            {
                Headers = new HeaderDictionary(),
                ContentType = $"image/{Path.GetExtension(path).TrimStart('.')}"
            };
        }
    }
}