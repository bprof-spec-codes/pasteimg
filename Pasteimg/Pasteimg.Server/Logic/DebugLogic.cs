using Pasteimg.Server.Models;
using Pasteimg.Server.Repository;

namespace Pasteimg.Server.Logic
{
    public class DebugLogic : PasteImgLogic
    {
        public DebugLogic(IPasswordHasher passwordHasher,
                          IImageProcessor processor,
                          IImageFileStorage fileStorage) : base(new DebugRepository<Image>(), new DebugRepository<Upload>(), passwordHasher, processor, fileStorage)
        {
            imageResults = new Dictionary<string, CompressionResult>();
            thumbnailResults = new Dictionary<string, ProcessorResult>();
        }
        Dictionary<string, CompressionResult> imageResults;
        Dictionary<string, ProcessorResult> thumbnailResults;
        
        public override void UploadImages(UploadModel upload)
        {
            base.UploadImages(upload, (img, thumb,key) =>
            {
                imageResults.Add(key,img);
                thumbnailResults.Add(key,thumb);
            });
        }
    }
}