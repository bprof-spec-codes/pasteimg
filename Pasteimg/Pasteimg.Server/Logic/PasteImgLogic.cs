using Pasteimg.Server.Models;
using Pasteimg.Server.Repository;

namespace Pasteimg.Server.Logic
{
    public class PasteImgLogic : IPasteImgLogic
    {
        protected static string Root { get; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot");
        protected static string ImageRoot { get; } = Path.Combine(Root, "images");
        protected static string ThumbnailRoot { get; } = Path.Combine(Root, "thumbnails");


        protected readonly IRepository<Image> imageRepository;
        protected readonly IRepository<Upload> uploadRepository;
        protected readonly IPasswordHasher passwordHasher;
        protected readonly IImageFileStorage fileStorage;
        public IImageProcessor Processor { get; }
        public int? MaxImagePerUpload { get; set; } = 50;
        public long? MaxFileSize { get; set; } = 10 * MegaByte;

        protected const long MegaByte = 1_000_000;

        public PasteImgLogic(IRepository<Image> imageRepository,
                             IRepository<Upload> uploadRepository,
                             IPasswordHasher passwordHasher,
                             IImageProcessor processor,
                             IImageFileStorage fileStorage)
        {
            this.imageRepository = imageRepository;
            this.uploadRepository = uploadRepository;
            this.passwordHasher = passwordHasher;
            this.fileStorage = fileStorage;
            Processor = processor;
        }

        public virtual IEnumerable<Upload> ReadAllUpload()
        {
            return uploadRepository.ReadAll();
        }
        public virtual IEnumerable<Image> ReadAllImage()
        {
            return imageRepository.ReadAll();
        }

        protected virtual void ValidateUploadModel(UploadModel uploadModel)
        {

        }

        private string GenerateUploadId()
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            } while (uploadRepository.ReadAll().FirstOrDefault(u => u.Id == id) != null);
            return id;
        }
        protected virtual void UploadImages(UploadModel uploadModel, Action<CompressionResult, ProcessorResult, string>? resultAction)
        {
            ValidateUploadModel(uploadModel);
            Upload upload = new Upload()
            {
                Id = GenerateUploadId(),
                TimeStamp = DateTime.Now
            };

            foreach (var item in uploadModel.Images)
            {
                CompressionResult imgResult = Processor.Compress(item);
                ProcessorResult thumbResult = Processor.CreateThumbnail(item);
                string key = fileStorage.SaveImage(ImageRoot, imgResult, ThumbnailRoot, thumbResult);
                item.Id = key;
                resultAction?.Invoke(imgResult, thumbResult, key);
                item.UploadID = upload.Id;
            }

            uploadRepository.Create(upload);
        }
        public virtual void UploadImages(UploadModel uploadModel)
        {
            UploadImages(uploadModel, null);
        }

        public virtual string? FindImageFile(string id)
        {
            return fileStorage.FindImage(ImageRoot, id);
        }

        public virtual string? FindThumbnailFile(string id)
        {
            if (fileStorage.FindImage(ThumbnailRoot, id) is string thumbnail)
            {
                return thumbnail;
            }
            else if (fileStorage.FindImage(Root, id) is string image)
            {
                return image;
            }
            else return null;
        }

        public virtual void DeleteImage(string id)
        {
            if (imageRepository.Delete(id) is not null)
            {
                fileStorage.DeleteImage(ImageRoot, ThumbnailRoot, id);
            };
        }
        public virtual void DeleteUpload(string id)
        {
            if (uploadRepository.Delete(id) is Upload upload)
            {
                foreach (var item in upload.Images)
                {
                    DeleteImage(item.Id);
                }
            };
        }
        public virtual Upload? ReadUpload(string id)
        {
            return uploadRepository.Read(id);
        }
        public virtual Image? ReadImage(string id)
        {
            return imageRepository.Read(id);
        }

    }
}