using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pasteimg.Server.Models;
using System.IO;
using System.Net;

namespace Pasteimg.Server.Repository
{
    public interface IImageFileStorage
    {
        int FolderDepth { get; }
        string? FindImage(string root, string key);
        string SaveImage(string root, ProcessorResult image, string? thumbnailRoot, ProcessorResult? thumbnail);
        bool DeleteImage(string root, string thumbnailRoot, string key);
    }
}
