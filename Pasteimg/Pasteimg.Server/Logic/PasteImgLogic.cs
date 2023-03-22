using Pasteimg.Server.Configurations;
using Pasteimg.Server.ImageTransformers;
using Pasteimg.Server.Models;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Models.Error;
using Pasteimg.Server.Repository;
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

        Image EditImage(string id, string? description, bool nsfw);

        IEnumerable<Image> GetAllImage();

        IEnumerable<Upload> GetAllUpload();

        Image GetImage(string id);

        Image GetImageWithSourceFile(string id);

        Image GetImageWithThumbnailFile(string id);

        Upload GetUpload(string id);

        Upload GetUploadWithSourceFiles(string id);

        Upload GetUploadWithThumbnailFiles(string id);

        ValidationConfiguration GetValidationConfiguration();

        void Upload(Upload upload);
    }

    public class PasteImgLogic : IPasteImgLogic
    {
        protected readonly IFileStorage fileStorage;
        protected readonly IRepository<Image> imageRepository;
        protected readonly IImageTransformer sourceOptimizer;
        protected readonly IImageTransformer thumbnailCreator;
        protected readonly IRepository<Upload> uploadRepository;

        public PasteImgLogic(PasteImgConfiguration configuration,
                             IRepository<Upload> uploadRepository,
                             IRepository<Image> imageRepository,
                             IImageTransformerFactory transformerFactory,
                             IFileStorage fileStorage)

        {
            Configuration = configuration;
            Source = configuration.Storage.SourceFileClass;
            Thumbnail = configuration.Storage.ThumbnailFileClass;
            Temp = configuration.Storage.TempFileClass;

            this.imageRepository = imageRepository;
            this.uploadRepository = uploadRepository;
            this.fileStorage = fileStorage;
            thumbnailCreator = transformerFactory.CreateThumbnailCreator();
            sourceOptimizer = transformerFactory.CreateSourceOptimizer();
        }

        public PasteImgConfiguration Configuration { get; }
        private string ContentType { get; } = "image";
        private string Source { get; }
        private string Temp { get; }
        private string Thumbnail { get; }

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
            fileStorage.DeleteFileWithAllClass(id);
        }

        public virtual void DeleteUpload(string id)
        {
            if (uploadRepository.Delete(id) is Upload upload)
            {
                foreach (var item in upload.Images)
                {
                    imageRepository.Delete(item.Id);
                    fileStorage.DeleteFileWithAllClass(id);
                }
            }
            else
            {
                throw new NotFoundException(typeof(Upload), id);
            }
        }

        public Image EditImage(string id, string? description, bool nsfw)
        {
            if (description is not null && description.Length > Configuration.Validation.MaxImagePerUpload)
            {
                throw new InvalidEntityException(typeof(Image), nameof(Image.Description), description, id);
            }

            Image? image = imageRepository.Update((i) =>
            {
                i.Description = description;
                i.NSFW = nsfw;
            }, id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }
            return image;
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

        public Image GetImageWithSourceFile(string id) => GetImageWithFile(id, Source);

        public Image GetImageWithThumbnailFile(string id) => GetImageWithFile(id, Thumbnail);

        public Upload GetUpload(string id)
        {
            Upload? upload = uploadRepository.Read(id);
            if (upload is null)
            {
                throw new NotFoundException(typeof(Upload), id);
            }
            return upload;
        }

        public Upload GetUploadWithSourceFiles(string id) => GetUploadWithFiles(id, Source);

        public Upload GetUploadWithThumbnailFiles(string id) => GetUploadWithFiles(id, Thumbnail);

        public ValidationConfiguration GetValidationConfiguration()
        {
            return Configuration.Validation;
        }

        public void Upload(Upload upload)
        {
            try
            {
                ValidateUpload(upload);
                SetUpload(upload);
                SetImages(upload);
                uploadRepository.Create(upload);
            }
            catch (PasteImgException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SomethingWrongException(ex, null, null);
            }
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

        private async Task FileProcessing(string id, byte[] content)
        {
            thumbnailCreator.Transform(content, fileStorage.GetPath(id, Thumbnail));
            sourceOptimizer.Transform(content, fileStorage.GetPath(id, Source));
            await TryDeleteTempFile(id).WaitAsync(TimeSpan.FromSeconds(10));
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

        private IFormFile GetFile(string id, string fileClass)
        {
            if (fileStorage.FindFile(id, ContentType, fileClass) is IFormFile file)
            {
                return file;
            }
            else if (fileStorage.FindFile(id, ContentType, Temp) is IFormFile temp)
            {
                return temp;
            }
            else
            {
                return new FormFile(Stream.Null, 0, 0, "null", "null");
            }
        }

        private Image GetImageWithFile(string id, string fileClass)
        {
            Image image = GetImage(id);
            image.Content = GetFile(id, fileClass);
            return image;
        }

        private Upload GetUploadWithFiles(string id, string fileClass)
        {
            Upload upload = GetUpload(id);
            foreach (var item in upload.Images)
            {
                item.Content = GetFile(id, fileClass);
            }
            return upload;
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
                byte[] content = image.Content.ToArray();
                fileStorage.SaveFile(content, image.Id, Path.GetExtension(image.Content.FileName), Temp);
                _ = Task.Run(async () => await FileProcessing(image.Id, content));
            }
        }

        private void SetUpload(Upload upload)
        {
            upload.Id = GenerateId(uploadRepository.ReadAll().Select(i => i.Id));
            upload.Password = CreateHash(upload.Password);
            upload.TimeStamp = DateTime.Now;
        }

        private async Task TryDeleteTempFile(string id)
        {
            do
            {
                try
                {
                    fileStorage.DeleteFile(id, Temp);
                }
                catch (IOException)
                { }
                await Task.Delay(10);
            }
            while (fileStorage.FindPath(id, Temp) is not null);
        }
    }
}