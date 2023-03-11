using Microsoft.AspNetCore.Http;
using Pasteimg.Server.Models;
using Pasteimg.Server.Repository;
using Pasteimg.Server.Repository.IFileStorage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Server.Logic
{
    public interface IPasteImgLogic
    {
        long? MaxFileSize { get; set; }
        int? MaxImagePerUpload { get; set; }

        void DeleteImage(string id);
        void DeleteOptimizationResult(string id);
        void DeleteUpload(string id);
        string? GetImagePath(Image image);
        string? GetImagePath(string id);
        string? GetThumbnailPath(Image image);
        string? GetThumbnailPath(string id);
        IEnumerable<Image> ReadAllImage();
        IEnumerable<OptimizationResult> ReadAllOptimizationResult();
        IEnumerable<Upload> ReadAllUpload();
        Image? ReadImage(string id);
        OptimizationResult? ReadOptimizationResult(string id);
        Upload? ReadUpload(string id);
        void Upload(Upload upload);
        void UploadAndStoreResults(Upload upload);
        Task UploadAndStoreResultsAsync(Upload upload);
    }
    public class PasteImgLogic : IPasteImgLogic
    {
        protected const long MegaByte = 1_000_000;
        protected readonly IRepository<Image> imageRepository;
        protected readonly IRepository<Upload> uploadRepository;
        protected readonly IRepository<OptimizationResult> optimizationRepository;
        protected readonly IImageFileStorage images;
        protected readonly IImageFileStorage thumbnails;

        public PasteImgLogic(IRepository<Image> imageRepository,
                             IRepository<Upload> uploadRepository,
                             IRepository<OptimizationResult> optimizationRepository,
                             IImageFileStorageFactory fileStorageFactory) :
            this(imageRepository, uploadRepository, optimizationRepository,
                fileStorageFactory.CreateImagesStorage("images",1500, 1500), 
                fileStorageFactory.CreateThumbnailsStorage("images_thumbnails",300, 300))
        {
        }

        protected PasteImgLogic(IRepository<Image> imageRepository,
                            IRepository<Upload> uploadRepository,
                            IRepository<OptimizationResult> optimizationRepository,
                            IImageFileStorage images,
                            IImageFileStorage thumbnails)
        {
            this.imageRepository = imageRepository;
            this.uploadRepository = uploadRepository;
            this.optimizationRepository = optimizationRepository;
            this.images = images;
            this.thumbnails = thumbnails;
        }

        public long? MaxFileSize { get; set; } = 10 * MegaByte;
        public int? MaxImagePerUpload { get; set; } = 50;


        public virtual void DeleteOptimizationResult(string id)
        {
            optimizationRepository.Delete(id);
        }
        public virtual void DeleteImage(string id)
        {

            if (imageRepository.Delete(id) is Image image)
            {
                images.DeleteFile(image.Id);
                thumbnails.DeleteFile(image.Id);
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

        public string? GetImagePath(Image image)
        {
            return GetImagePath(image.Id);
        }

        public string? GetImagePath(string id)
        {
            return images.FindWebFile(id);
        }


        public string? GetThumbnailPath(Image image)
        {
            return GetThumbnailPath(image.Id);
        }

        public string? GetThumbnailPath(string id)
        {
            return thumbnails.FindWebFile(id);
        }

        public IEnumerable<Image> ReadAllImage()
        {
            return imageRepository.ReadAll();
        }

        public IEnumerable<Upload> ReadAllUpload()
        {
            return uploadRepository.ReadAll();
        }
        public IEnumerable<OptimizationResult> ReadAllOptimizationResult()
        {
            return optimizationRepository.ReadAll();
        }
        public Image? ReadImage(string id)
        {
            return imageRepository.Read(id);
        }

        public Upload? ReadUpload(string id)
        {
            return uploadRepository.Read(id);
        }
        public OptimizationResult? ReadOptimizationResult(string id)
        {
            return optimizationRepository.Read(id);
        }
      
        public void Upload(Upload upload)
        {
            ValidateUpload(upload);
            SetUpload(upload);
            SetImages(upload, out FileArgument[] files);
            var thumbResults = files.Select(f => thumbnails.OptimizeImage(f.Content, f.Id)).ToArray();
            thumbnails.SaveFiles(thumbResults);
            var results = files.Select(f => images.OptimizeImage(f.Content, f.Id)).ToArray();
            images.SaveFiles(results);
            uploadRepository.Create(upload);

            CleanContents(upload.Images, thumbResults, results);
        }

        public async Task UploadAsync(Upload upload)
        {
            ValidateUpload(upload);
            SetUpload(upload);
            SetImages(upload, out FileArgument[] files);
            var thumbResults = await thumbnails.OptimizeImagesAsync(files);
            thumbnails.SaveFiles(thumbResults);
            images.SaveFiles(files);
            var results = await images.OptimizeImagesAsync(files);
            images.SaveFiles(results);
            uploadRepository.Create(upload);

            CleanContents(upload.Images, thumbResults, results);
        }
        public void UploadAndStoreResults(Upload upload)
        {
            ValidateUpload(upload);
            SetUpload(upload);
            SetImages(upload, out FileArgument[] files);
            var thumbResults = files.Select(f => thumbnails.OptimizeImage(f.Content, f.Id)).ToArray();
            thumbnails.SaveFiles(thumbResults);
            var results = files.Select(f => images.OptimizeImage(f.Content, f.Id)).ToArray();
            images.SaveFiles(results);
            uploadRepository.Create(upload);

            foreach (var item in results)
            {
                optimizationRepository.Create(item);
            }
            CleanContents(upload.Images, thumbResults, results);
        }
        public async Task UploadAndStoreResultsAsync(Upload upload)
        {
            ValidateUpload(upload);
            SetUpload(upload);
            SetImages(upload, out FileArgument[] files);
            var thumbResults = await thumbnails.OptimizeImagesAsync(files);
            thumbnails.SaveFiles(thumbResults);
            images.SaveFiles(files);
            var results=await images.OptimizeImagesAsync(files);
            images.SaveFiles(results);
            uploadRepository.Create(upload);

            foreach (var item in results)
            {
                optimizationRepository.Create(item);
            }

            CleanContents(upload.Images, thumbResults, results);
        }
        private void CleanContents(IList<Image> images,OptimizationResult[] thumbnailResults, OptimizationResult[] imagesResults)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].Content = null;
                thumbnailResults[i].Content = null;
                imagesResults[i].Content = null;
            }
        }

  
     
        protected virtual void ValidateUpload(Upload upload)
        {
        }

        private void SetUpload(Upload upload)
        {
            upload.Id = GenerateId(uploadRepository.ReadAll().Select(i => i.Id));
            upload.Password = CreateHash(upload.Password);
            upload.TimeStamp = DateTime.Now;
        }
        private void SetImages(Upload upload, out FileArgument[] files)
        {
            int count = upload.Images.Count;
            files = new FileArgument[count];
            string[] ids = GenerateMultipleId(ReadAllImage().Select(i => i.Id), count);
            for (int i = 0; i < count; i++)
            {
                Image image = upload.Images[i];
                image.Id = ids[i];
                image.UploadID = upload.Id;
                files[i] = new FileArgument(image.Content.ToArray(),
                                            ids[i],
                                            Path.GetFileNameWithoutExtension(image.Content.FileName));
            }
        }
        private string? CreateHash(string? password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                using (SHA256 hash = SHA256.Create())
                {
                    Random random = new Random(password.Sum(c => ~c));
                    byte[] salt = new byte[10];
                    random.NextBytes(salt);
                    password += new string(salt.Select(b => (char)b).ToArray());
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    for (int i = bytes.Length - 1; i > 1; i--)
                    {
                        int j = random.Next(0, i + 1);
                        if (i != j)
                        {
                            byte temp = bytes[i];
                            bytes[i] = bytes[j];
                            bytes[j] = temp;
                        }
                    }

                    return BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }
            }
            else return null;
        }

        private string GenerateId(IEnumerable<string> collection)
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            } while (collection.FirstOrDefault(key => key == id) != null);
            return id;
        }

        private string[] GenerateMultipleId(IEnumerable<string> collection, int count)
        {
            string[] ids = new string[count];
            for (int i = 0; i < count; i++)
            {
                ids[i] = GenerateId(collection);
            }
            return ids;
        }
   

    }

    public class PasteImgLogicConfiguration
    {
        public int CompressionQuality { get; }
        public string ImageRoot { get; }
        public long? MaxFileSize { get; }
        public int? MaxImagePerUpload { get; }
        public (int width, int height) MaxSize { get; }
        public (int width, int height) MaxThumbnailSize { get; }
        public ReadOnlyCollection<string> SupportedFormats { get; }
        public string ThumbnailRoot { get; }
    }
}