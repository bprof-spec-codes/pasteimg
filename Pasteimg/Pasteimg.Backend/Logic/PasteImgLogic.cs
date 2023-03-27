using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Models.Error;
using Pasteimg.Backend.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Backend.Logic
{
    /// <summary>
    /// Interface for the Pasteimg logic that handles all functionality related to uploads and images.
    /// </summary>
    public interface IPasteImgLogic
    {
        /// <summary>
        /// Configuration object for the current Pasteimg instance.
        /// </summary>
        PasteImgConfiguration Configuration { get; }

        /// <summary>
        /// Creates a hash for the given password.
        /// </summary>
        /// <param name="password">The password to create a hash for.</param>
        /// <returns>The hash string.</returns>
        string? CreateHash(string? password);

        /// <summary>
        /// Deletes the image with the given ID.
        /// </summary>
        /// <param name="id">The ID of the image to delete.</param>
        void DeleteImage(string id);

        /// <summary>
        /// Deletes the upload  with the given ID.
        /// </summary>
        /// <param name="id">The ID of the upload to delete.</param>
        void DeleteUpload(string id);

        /// <summary>
        /// Updates the description and NSFW status of the image with the given ID.
        /// </summary>
        /// <param name="id">The ID of the image to edit.</param>
        /// <param name="description">The new description for the image.</param>
        /// <param name="nsfw">The new NSFW status for the image.</param>
        /// <returns>The edited image object.</returns>
        Image EditImage(string id, string? description, bool nsfw);

        /// <summary>
        /// Returns all images currently stored without files.
        /// </summary>
        IEnumerable<Image> GetAllImage();

        /// <summary>
        /// Returns all uploads currently stored without files.
        /// </summary>
        IEnumerable<Upload> GetAllUpload();

        /// <summary>
        /// Gets an image by ID without file.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>The image with the specified ID.</returns>
        Image GetImage(string id);

        /// <summary>
        /// Gets an image with its source file by ID.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>The image with the specified ID, including its source file.</returns>
        Image GetImageWithSourceFile(string id);

        /// <summary>
        /// Gets an image with its thumbnail file by ID.
        /// </summary>
        /// <param name="id">The ID of the image to retrieve.</param>
        /// <returns>The image with the specified ID, including its thumbnail file.</returns>
        Image GetImageWithThumbnailFile(string wid);

        /// <summary>
        /// Gets an upload by ID without files.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>The upload with the specified ID.</returns>
        Upload GetUpload(string id);

        /// <summary>
        /// Gets an upload with its source files by ID.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>The upload with the specified ID, including its source files.</returns>
        Upload GetUploadWithSourceFiles(string id);

        /// <summary>
        /// Gets an upload with its thumbnail files by ID.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>The upload with the specified ID, including its thumbnail files.</returns>
        Upload GetUploadWithThumbnailFiles(string id);

        /// <summary>
        /// Stores the uploaded images, if modelstate is valid.
        /// </summary>
        /// <param name="upload">The object containing the images to be uploaded.</param>
        void PostUpload(Upload upload);
    }

    /// <summary>
    /// Logic implementation that handles all functionality related to uploads and <see cref="Models.Entity.Image"/>s.
    /// </summary>
    public class PasteImgLogic : IPasteImgLogic
    {
        protected readonly IFileStorage fileStorage;
        protected readonly IRepository<Image> imageRepository;
        protected readonly ImageTransformer sourceOptimizer;
        protected readonly ImageTransformer thumbnailCreator;
        protected readonly IRepository<Upload> uploadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasteImgLogic"/> class with the specified dependencies.
        /// </summary>
        /// <param name="configuration">The configuration settings for the logic.</param>
        /// <param name="uploadRepository">The repository for managing uploads.</param>
        /// <param name="imageRepository">The repository for managing images.</param>
        /// <param name="transformerFactory">The factory for creating image transformers.</param>
        /// <param name="fileStorage">The storage provider for saving image files.</param>
        public PasteImgLogic(PasteImgConfiguration configuration,
                             IRepository<Upload> uploadRepository,
                             IRepository<Image> imageRepository,
                             IImageTransformerFactory transformerFactory,
                             IFileStorage fileStorage)

        {
            ValidateConfiguration(configuration);
            Configuration = configuration;
            this.imageRepository = imageRepository;
            this.uploadRepository = uploadRepository;
            this.fileStorage = fileStorage;
            thumbnailCreator = transformerFactory.CreateThumbnailCreator(Configuration.Thumbnail);
            sourceOptimizer = transformerFactory.CreateSourceOptimizer(Configuration.Source);
        }

        /// <summary>
        /// Gets the configuration settings.
        /// </summary>
        public PasteImgConfiguration Configuration { get; }

        /// <summary>
        /// Gets the file class of the source files.
        /// </summary>
        private string SourceFileClass { get; } = "src";

        /// <summary>
        /// Gets the file class of the temp files.
        /// </summary>
        private string TempFileClass { get; } = "tmp";

        /// <summary>
        /// Gets the file class of the thumbnail files.
        /// </summary>
        private string ThumbnailFileClass { get; } = "thb";

        /// <summary>
        /// Creates a hash for a given password using the SHA256 algorithm.
        /// </summary>
        /// <param name="password">The password to create a hash for.</param>
        /// <returns>The hashed password.</returns>
        public string? CreateHash(string? password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                using (SHA256 hash = SHA256.Create())
                {
                    int randomSeed = 0;
                    unchecked
                    {
                        foreach (var chr in password)
                        {
                            randomSeed += ~chr;
                        }
                    }

                    Random random = new Random(randomSeed);
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

        /// <summary>
        /// Deletes an image with the specified ID from the image repository and all associated files from the file storage.
        /// </summary>
        /// <param name="id">The ID of the image to delete.</param>
        /// <exception cref="NotFoundException"/>
        public virtual void DeleteImage(string id)
        {
            Image? image = imageRepository.Delete(id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }
            _ = Task.Run(async () =>
            {
                await TryDeleteFile(id, SourceFileClass,10).WaitAsync(TimeSpan.FromSeconds(5));
                await TryDeleteFile(id, ThumbnailFileClass,10).WaitAsync(TimeSpan.FromSeconds(5));
            });
        }

        /// <summary>
        /// Deletes an upload with the specified ID from the upload repository and all associated images and files from the file storage.
        /// </summary>
        /// <param name="id">The ID of the upload to delete.</param>
        /// <exception cref="NotFoundException"/>
        public virtual void DeleteUpload(string id)
        {
            if (uploadRepository.Delete(id) is Upload upload)
            {
                foreach (var item in upload.Images)
                {
                    imageRepository.Delete(item.Id);
                    _ = Task.Run(async () =>
                    {
                        await TryDeleteFile(item.Id, SourceFileClass, 10).WaitAsync(TimeSpan.FromSeconds(5));
                        await TryDeleteFile(item.Id, ThumbnailFileClass, 10).WaitAsync(TimeSpan.FromSeconds(5));
                    });
                }
            }
            else
            {
                throw new NotFoundException(typeof(Upload), id);
            }
        }

        /// <summary>
        /// Edits an image with the given ID, updating its description and NSFW flag. Throws an exception if the description is too long or the image with the given ID is not found in the image repository.
        /// </summary>
        /// <param name="id">The ID of the image to edit.</param>
        /// <param name="description">The new description of the image. Can be null or empty.</param>
        /// <param name="nsfw">The new NSFW flag of the image.</param>
        /// <returns>The updated image.</returns>
        /// <exception cref="InvalidEntityException"/>
        /// <exception cref="NotFoundException"/>
        public Image EditImage(string id, string? description, bool nsfw)
        {
            if (description is not null && description.Length > Configuration.Validation.MaxImagePerUpload)
            {
                throw new InvalidEntityException(typeof(Image), nameof(Image.Description), description, id);
            }

            Image? image = imageRepository.Update((img) =>
            {
                img.Description = description;
                img.NSFW = nsfw;
            }, id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }
            return image;
        }

        /// <summary>
        /// Returns all the images in the repository.
        /// </summary>
        /// <returns> A collection of all the images.</returns>
        public IEnumerable<Image> GetAllImage()
        {
            return imageRepository.ReadAll();
        }

        /// <summary>
        /// Returns all the uploads in the repository
        /// </summary>
        /// <returns> A collection of all the uploads</returns>
        public IEnumerable<Upload> GetAllUpload()
        {
            return uploadRepository.ReadAll();
        }

        /// <summary>
        /// Returns the image with the specified id without file.
        /// </summary>
        /// <param name="id"> The ID of the image to retrieve.</param>
        /// <returns> The image with the specified id.</returns>
        /// <exception cref="NotFoundException"/>
        public Image GetImage(string id)
        {
            Image? image = imageRepository.Read(id);
            if (image is null)
            {
                throw new NotFoundException(typeof(Image), id);
            }

            return image;
        }

        /// <summary>
        /// Returns the image with the specified id along with its source file.
        /// </summary>
        /// <param name="id"> The ID of the image to retrieve.</param>
        /// <returns> The image with the specified id along with its source file.</returns>
        /// <exception cref="NotFoundException"/>
        public Image GetImageWithSourceFile(string id) => GetImageWithFile(id, SourceFileClass);

        /// <summary>
        /// Returns the image with the specified id along with its thumbnail file.
        /// </summary>
        /// <param name="id"> The ID of the image to retrieve.</param>
        /// <returns> The image with the specified id along with its thumbnail file.</returns>
        /// <exception cref="NotFoundException"/>
        public Image GetImageWithThumbnailFile(string id) => GetImageWithFile(id, ThumbnailFileClass);

        /// <summary>
        /// Returns the upload with the specified id without files.
        /// </summary>
        /// <param name="id"> The ID of the upload to retrieve.</param>
        /// <returns> The upload with the specified id.</returns>
        /// <exception cref="NotFoundException"/>
        public Upload GetUpload(string id)
        {
            Upload? upload = uploadRepository.Read(id);
            if (upload is null)
            {
                throw new NotFoundException(typeof(Upload), id);
            }
            return upload;
        }

        /// <summary>
        ///Returns the upload with the specified id, including all source files.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>The upload with the specified id and all source files.</returns>
        /// <exception cref="NotFoundException"/>
        public Upload GetUploadWithSourceFiles(string id) => GetUploadWithFiles(id, SourceFileClass);

        /// <summary>
        ///Returns the upload with the specified id, including all thumbnail files.
        /// </summary>
        /// <param name="id">The ID of the upload to retrieve.</param>
        /// <returns>The upload with the specified id and all thumbnail files.</returns>
        /// <exception cref="NotFoundException"/>
        public Upload GetUploadWithThumbnailFiles(string id) => GetUploadWithFiles(id, ThumbnailFileClass);

        /// <summary>
        ///Processes an upload by validating the provided data, setting the upload and image properties, storing the images on the file storage, creating the corresponding thumbnails and optimized sources, and adding the upload to the repository.
        /// </summary>
        /// <param name="upload">The upload to process.</param>
        /// <exception cref="InvalidEntityException"/>
        /// <exception cref="SomethingWrongException"/>
        public void PostUpload(Upload upload)
        {
            try
            {
                ValidateUpload(upload);
                SetUpload(upload);
                SetImages(upload);
                uploadRepository.Create(upload);
            }
            catch (InvalidEntityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SomethingWrongException(ex, null, null);
            }
        }

        /// <summary>
        ///Validates the given upload by checking that it meets certain criteria, such as the number of images, the file size, the file format, and the description length.
        /// </summary>
        /// <param name="upload">The upload to validate.</param>
        /// <exception cref="InvalidEntityException"></exception>
        protected virtual void ValidateUpload(Upload upload)
        {
            if (upload.Images.Count == 0 || upload.Images.Count > Configuration.Validation.MaxImagePerUpload)
            {
                throw new InvalidEntityException(typeof(Upload), nameof(upload.Images.Count), upload.Images.Count);
            }
            if (!string.IsNullOrEmpty(upload.Password) && upload.Password.Length > Configuration.Validation.PasswordMaxLength)
            {
                throw new InvalidEntityException(typeof(Upload), nameof(upload.Password), upload.Password);
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

        /// <summary>
        /// Asynchronously creates optimized versions of an image and deletes its temporary file.
        /// </summary>
        /// <param name="id">The unique identifier for the image being processed.</param>
        /// <param name="content">The byte array containing the image content to be processed.</param>
        private async Task FileProcessing(string id, byte[] content)
        {
            thumbnailCreator.Transform(content, fileStorage.GetPath(id, fileClass: ThumbnailFileClass));
            sourceOptimizer.Transform(content, fileStorage.GetPath(id, fileClass: SourceFileClass));
            await TryDeleteFile(id,TempFileClass,10).WaitAsync(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        ///Generates a unique identifier by creating a new GUID and replacing dashes and converting it to lowercase, and ensures that the identifier does not exist in the repository keys.
        /// </summary>
        /// <param name="repositoryKeys">The collection of existing keys to compare the generated identifier against.</param>
        /// <returns>A string representation of a unique identifier.</returns>
        private string GenerateID(IEnumerable<string> repositoryKeys)
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            } while (repositoryKeys.FirstOrDefault(key => key == id) != null);
            return id;
        }

        /// <summary>
        ///Generates an array of unique identifiers by calling the GenerateID method multiple times and returning an array of results.
        /// </summary>
        /// <param name="repositoryKeys">The collection of existing keys to compare the generated identifiers against.</param>
        /// <param name="count">The number of identifiers to generate.</param>
        /// <returns>An array of string representations of unique identifiers.</returns>
        private string[] GenerateMultipleID(IEnumerable<string> repositoryKeys, int count)
        {
            string[] ids = new string[count];
            for (int i = 0; i < count; i++)
            {
                ids[i] = GenerateID(repositoryKeys);
            }
            return ids;
        }

        /// <summary>
        /// Retrieves an image file with the given identifier and file class from the file storage service.
        /// If the file cannot be found, a default "unavailable" image file is returned.
        /// </summary>
        /// <param name="id">The unique identifier for the image file to be retrieved.</param>
        /// <param name="fileClass">The file class of the image file to be retrieved.</param>
        /// <returns>An <see cref="IFormFile"/> containing the image data.</returns>
        private IFormFile GetFile(string id, string fileClass)
        {
            string path;
            string unavaliable= "unavaliable.webp";

            if (fileStorage.FindPath(id, fileClass) is string filePath)
            {
                path = filePath;
            }
            else if (fileStorage.FindPath(id, TempFileClass) is string tempPath)
            {
                path = tempPath;
            }
            else
            {
                path = unavaliable;
            }

            MemoryStream stream;
            try
            {
                stream = new MemoryStream(File.ReadAllBytes(path));
            }
            catch(IOException)
            {
                stream = new MemoryStream(File.ReadAllBytes(unavaliable));
            }


            return new FormFile(stream, 0, stream.Length, Path.GetFileNameWithoutExtension(path), Path.GetFileName(path))
            {
                Headers = new HeaderDictionary(),
                ContentType = $"image/{Path.GetExtension(path).TrimStart('.')}"
            };
        }

        /// <summary>
        /// Retrieves an image with the given identifier and file class from the database and file storage service,
        /// attaching the image file to the image object. If the file cannot be found, a default "unavailable" image file is attached.
        /// </summary>
        /// <param name="id">The unique identifier for the image to be retrieved.</param>
        /// <param name="fileClass">The file class of the image file to be retrieved.</param>
        /// <returns>The image object with the specified identifier and file class.</returns>
        private Image GetImageWithFile(string id, string fileClass)
        {
            Image image = GetImage(id);
            image.Content = GetFile(id, fileClass);
            return image;
        }

        /// <summary>
        /// Retrieves an upload with the given identifier and file class from the database and file storage service,
        /// attaching the image files to the image objects in the upload. If a file cannot be found for an image,
        /// a default "unavailable" image file is attached to the image object.
        /// </summary>
        /// <param name="id">The unique identifier for the upload to be retrieved.</param>
        /// <param name="fileClass">The file class of the image files to be retrieved.</param>
        /// <returns>The upload object with the specified identifier and file class.</returns>
        private Upload GetUploadWithFiles(string id, string fileClass)
        {
            Upload upload = GetUpload(id);
            foreach (var item in upload.Images)
            {
                item.Content = GetFile(id, fileClass);
            }
            return upload;
        }

        /// <summary>
        /// Sets the unique identifiers for the images in the given upload, saves the images
        /// and triggers asynchronous processing to create optimized versions of the images and delete the temporary files.
        /// </summary>
        /// <param name="upload">The upload containing the images to be processed.</param>
        private void SetImages(Upload upload)
        {
            int count = upload.Images.Count;
            string[] ids = GenerateMultipleID(GetAllImage().Select(i => i.Id), count);
            for (int i = 0; i < count; i++)
            {
                Image image = upload.Images[i];
                image.Id = ids[i];
                image.UploadID = upload.Id;
                byte[] content = image.Content.ToArray();
                fileStorage.SaveFile(content, image.Id, Path.GetExtension(image.Content.FileName), TempFileClass);
                _ = Task.Run(async () => await FileProcessing(image.Id, content));
            }
        }

        /// <summary>
        ///Sets the unique identifier, password hash, and timestamp of the upload.
        /// </summary>
        /// <param name="upload">The upload to set.</param>
        private void SetUpload(Upload upload)
        {
            upload.Id = GenerateID(uploadRepository.ReadAll().Select(i => i.Id));
            upload.Password = CreateHash(upload.Password);
            upload.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Asynchronously attempts to delete file for an image until the file is deleted or a timeout occurs.
        /// </summary>
        /// <param name="id">The unique identifier for the image whose file should be deleted.</param>
        /// <param name="fileClass">The class or type of the file to be deleted.</param>
        /// <param name="polling">The time interval (in milliseconds) at which to check if the file has been deleted.</param>
        private async Task TryDeleteFile(string id,string fileClass,int polling)
        {
            do
            {
                try
                {
                    fileStorage.DeleteFile(id, fileClass);
                }
                catch (IOException)
                { }
                await Task.Delay(polling);
            }
            while (fileStorage.FindPath(id, fileClass) is not null);
        }

        /// <summary>
        /// Validates the given PasteImgConfiguration against certain conditions.
        /// </summary>
        /// <param name="configuration">The configuration to validate.</param>
        private void ValidateConfiguration(PasteImgConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Visitor is null)
            {
                throw new ArgumentNullException(nameof(configuration.Visitor));
            }
            if (configuration.Visitor.LockoutTresholdInMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.Visitor.LockoutTresholdInMinutes));
            }
            if (configuration.Visitor.LockoutTresholdInMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.Visitor.MaxFailedAttempt));
            }
            if (configuration.Validation is null)
            {
                throw new ArgumentNullException(nameof(configuration.Validation));
            }
            if (configuration.Validation.MaxFileSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.Validation.MaxFileSize));
            }
            if (configuration.Validation.MaxImagePerUpload <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.Validation.MaxImagePerUpload));
            }
            if (configuration.Validation.DescriptionMaxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration.Validation.DescriptionMaxLength));
            }
            if (configuration.Validation.SupportedFormats is null || configuration.Validation.SupportedFormats.Count == 0)
            {
                throw new ArgumentNullException(nameof(configuration.Validation.SupportedFormats));
            }
            if (configuration.Validation.PasswordMaxLength <= 0)
            {
                throw new ArgumentNullException(nameof(configuration.Validation.PasswordMaxLength));
            }
        }
    }
}