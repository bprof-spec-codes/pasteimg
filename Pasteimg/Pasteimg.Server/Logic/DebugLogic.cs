using Pasteimg.Server.Models;
using Pasteimg.Server.Repository;
using Pasteimg.Server.Repository.IFileStorage;

namespace Pasteimg.Server.Logic
{
    public class DebugLogic:PasteImgLogic
    {

        public DebugLogic(IRepository<Image> imageRepository,
                             IRepository<Upload> uploadRepository,
                             IRepository<OptimizationResult> optimizationRepository,
                             IImageFileStorageFactory fileStorageFactory) :
            base(imageRepository, uploadRepository, optimizationRepository,
                fileStorageFactory.CreateImagesStorage(Path.Combine("deb","images"), 1500, 1500),
                fileStorageFactory.CreateThumbnailsStorage(Path.Combine("deb","images_thumbnails"), 300, 300))
        {
            if (imageRepository.ReadAll().Count() == 0)
            {
                images.ClearRoot();
                thumbnails.ClearRoot();
                AddSampleImagesAsync().Wait();
            }
        }

        private async Task AddSampleImagesAsync()
        {
            Upload upload = new Upload();
            var files = Directory.GetFiles(Path.Combine("wwwRoot", "deb", "optimizer_input"));
            upload.Password = "debug";
            Random rnd = new Random();
            foreach (var item in files)
            {
                MemoryStream stream = new MemoryStream(File.ReadAllBytes(item));
                string fileName = Path.GetFileName(item);
                upload.Images.Add(new Image()
                {
                    Description = fileName,
                    Content = new FormFile(stream, 0, stream.Length, fileName, fileName),
                    NSFW = rnd.Next(0, 2) == 1
                });
            }
            await UploadAndStoreResultsAsync(upload);
        }

    }
}