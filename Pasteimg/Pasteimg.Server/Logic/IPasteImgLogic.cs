using Microsoft.EntityFrameworkCore.Update.Internal;
using Pasteimg.Server.Models;
using System.Diagnostics;

namespace Pasteimg.Server.Logic
{

    public interface IPasteImgLogic
    {
        long? MaxFileSize { get; set; }
        int? MaxImagePerUpload { get; set; }
        IImageProcessor Processor { get; }

        void DeleteImage(string id);
        void DeleteUpload(string id);
        string? FindImageFile(string id);
        string? FindThumbnailFile(string id);
        IEnumerable<Image> ReadAllImage();
        IEnumerable<Upload> ReadAllUpload();
        Image? ReadImage(string id);
        Upload? ReadUpload(string id);
        void UploadImages(UploadModel uploadModel);
    }
}