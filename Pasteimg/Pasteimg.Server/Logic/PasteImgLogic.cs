using Pasteimg.Server.Configurations;
using Pasteimg.Server.Models;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Models.Error;
using Pasteimg.Server.Repository;
using Pasteimg.Server.Repository.FileStorages;
using Pasteimg.Server.Transformers;
using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Server.Logic
{
    public interface IPasteImgLogic
    {
        PasteImgConfiguration Configuration { get; }

        string? CreateHash(string? password);

        void DeleteImage(string id);

        void DeleteUpload(string id);

        IEnumerable<Image> GetAllImage();

        IEnumerable<Upload> GetAllUpload();

        Image GetImage(string id);

        Image GetImageWithSourceFile(string id);

        Image GetImageWithThumbnailFile(string id);

        Upload GetUpload(string id);

        Upload GetUploadWithSourceFiles(string id);

        Upload GetUploadWithThumbnailFiles(string id);

        ValidationConfiguration GetValidationConfiguration();

        void PostUpload(Upload upload);
    }

    public class PasteImgLogic : IPasteImgLogic
    {
        protected readonly IBackgroundTransformer backgroundTransformer;
        protected readonly IImageFileRepository imageRepository;
        protected readonly WebImageTransformer sourceOptimizer;
        protected readonly WebImageTransformer thumbnailCreator;
        protected readonly IRepository<Upload> uploadRepository;

        public PasteImgLogic(IRepository<Upload> uploadRepository,
                             IImageFileRepositoryFactory fileStorageFactory,
                             IWebImageTransformerFactory transformerFactory,
                             IBackgroundTransformer backgroundTransformer)

        {
            Configuration = ReadConfiguration();
            this.uploadRepository = uploadRepository;
            this.backgroundTransformer = backgroundTransformer;
            imageRepository = fileStorageFactory.CreateImageFileRepository(Configuration.Storage);
            thumbnailCreator = transformerFactory.CreateThumbnailCreator(Configuration.Transformation);
            sourceOptimizer = transformerFactory.CreateSourceOptimizer(Configuration.Transformation);
        }

        public PasteImgConfiguration Configuration { get; }

        public string? CreateHash(string? password)
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

        public virtual void DeleteImage(string id)
        {
            Image? image = imageRepository.Delete(id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }
        }

        public virtual void DeleteUpload(string id)
        {
            if (uploadRepository.Delete(id) is Upload upload)
            {
                foreach (var item in upload.Images)
                {
                    imageRepository.Delete(item.Id);
                }
            }
            else
            {
                throw new NotFoundException(typeof(Upload), id);
            }
        }

        public IEnumerable<Image> GetAllImage()
        {
            return imageRepository.ReadAll();
        }

        public IEnumerable<Upload> GetAllUpload()
        {
            return uploadRepository.ReadAll();
        }

        public Image GetImage(string id)
        {
            Image? image = imageRepository.Read(id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }

            return image;
        }

        public Image GetImageWithSourceFile(string id)
        {
            Image image = GetImage(id);
            image.Content = imageRepository.FindSourceFile(id);
            return image;
        }

        public Image GetImageWithThumbnailFile(string id)
        {
            Image image = GetImage(id);
            image.Content = imageRepository.FindThumbnailFile(id);
            return image;
        }

        public Upload GetUpload(string id)
        {
            Upload? upload = uploadRepository.Read(id);
            if (upload is null)
            {
                throw new NotFoundException(typeof(Upload), id);
            }
            return upload;
        }

        public Upload GetUploadWithSourceFiles(string id)
        {
            Upload upload = GetUpload(id);
            FillUploadWithFiles(upload, imageRepository.FindSourceFile);
            return upload;
        }

        public Upload GetUploadWithThumbnailFiles(string id)
        {
            Upload upload = GetUpload(id);
            FillUploadWithFiles(upload, imageRepository.FindThumbnailFile);
            return upload;
        }

        public ValidationConfiguration GetValidationConfiguration()
        {
            return Configuration.Validation;
        }

        public void PostUpload(Upload upload)
        {
            ValidateUpload(upload);
            SetUpload(upload);
            SetImages(upload);
            uploadRepository.Create(upload);
        }

        protected virtual void ValidateUpload(Upload upload)
        {
            if (upload.Images.Count == 0 || upload.Images.Count > Configuration.Validation.MaxImagePerUpload)
            {
                throw new InvalidEntityException(typeof(Upload), nameof(upload.Images.Count), upload.Images.Count);
            }

            foreach (var item in upload.Images)
            {
                if (item.Content is null)
                {
                    throw new InvalidEntityException(typeof(Image), nameof(item.Content), item.Content);
                }

                if (item.Content.Length > Configuration.Validation.MaxFileSize)
                {
                    throw new InvalidEntityException(typeof(Image), nameof(item.Content.Length), item.Content.Length);
                }

                if (!Configuration.Validation.SupportedFormats.Contains(Path.GetExtension(item.Content.FileName).TrimStart('.').ToLower()))
                {
                    throw new InvalidEntityException(typeof(Image), nameof(item.Content.ContentType), item.Content.ContentType);
                }

                if (item.Description != null && Configuration.Validation.DescriptionMaxLength < item.Description.Length)
                {
                    throw new InvalidEntityException(typeof(Image), nameof(item.Description), item.Description.Length);
                }
            }
        }

        private void FillUploadWithFiles(Upload? upload, Func<string, IFormFile?> find)
        {
            if (upload is not null)
            {
                foreach (var item in upload.Images)
                {
                    item.Content = find(item.Id);
                }
            }
        }

        private string GenerateId(IEnumerable<string> repositoryKeys)
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            } while (repositoryKeys.FirstOrDefault(key => key == id) != null);
            return id;
        }

        private string[] GenerateMultipleId(IEnumerable<string> repositoryKeys, int count)
        {
            string[] ids = new string[count];
            for (int i = 0; i < count; i++)
            {
                ids[i] = GenerateId(repositoryKeys);
            }
            return ids;
        }

        private PasteImgConfiguration ReadConfiguration()
        {
            string configPath = Path.Combine("Properties", "pasteImgLogic.json");
            try
            {
                return PasteImgConfiguration.ReadConfiguration(configPath);
            }
            catch
            {
                PasteImgConfiguration.WriteConfiguration(configPath, PasteImgConfiguration.Default);
                return PasteImgConfiguration.ReadConfiguration(configPath);
            }
        }

        private void SetImages(Upload upload)
        {
            int count = upload.Images.Count;
            string[] ids = GenerateMultipleId(GetAllImage().Select(i => i.Id), count);
            for (int i = 0; i < count; i++)
            {
                Image image = upload.Images[i];
                image.Id = ids[i];
                image.UploadID = upload.Id;
                imageRepository.Create(image, thumbnailCreator: thumbnailCreator);
                backgroundTransformer.EnqueueRequest
                    (new BackgroundTransformationRequest(imageRepository.GetSourcePath(image.Id), sourceOptimizer));
            }
        }

        private void SetUpload(Upload upload)
        {
            upload.Id = GenerateId(uploadRepository.ReadAll().Select(i => i.Id));
            upload.Password = CreateHash(upload.Password);
            upload.TimeStamp = DateTime.Now;
        }
    }
}