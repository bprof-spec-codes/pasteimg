using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pasteimg.Server.Models;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Server.Repository
{
    public interface IImageFileStorage
    {
        int FolderDepth { get; }
        string? FindImage(string root, string key);
        string SaveImage(string root, ProcessorResult image, string? thumbnailRoot, ProcessorResult? thumbnail);
        bool DeleteImage(string root, string thumbnailRoot, string key);
    }


    public class ImageFileStorage : IImageFileStorage
    {
        public int FolderDepth { get; } = 1;

        public string? FindImage(string root, string key)
        {
            string[] files = Directory.GetFiles(GetDirectory(root, key), $"{key}.*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                return files[0];
            }
            else
            {
                if(files.Length>1)
                {
                    Debug.Write($"FindImage: {root} {key}, Files: {string.Join("\n", files)}");
                }
                return null;
            }
        }

        private string GetDirectory(string root, string key)
        {
            string[] parts = new string[FolderDepth + 1];
            parts[0] = root;

            for (int i = 1; i < FolderDepth + 1; i++)
            {
                parts[i] = key[i].ToString();
            }

            return Path.Combine(parts);
        }

        private string CreatePath(string root, string key, string extension)
        {
            return Path.Combine(GetDirectory(root, key), key + extension);
        }

        private string GenerateKey(string root, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string path;
            string key;
            using (SHA1 sha1 = SHA1.Create())
            {
                do
                {
                    key = string.Concat(sha1.ComputeHash(Encoding.UTF8.GetBytes(fileName)));
                    path = CreatePath(root, key, extension);
                } while (FindImage(root, key) is not null);
                return key;
            };
        }

        public bool DeleteImage(string root,string thumbnailRoot,string key)
        {
            FindImage(thumbnailRoot, key);
            if (FindImage(root, key) is string path)
            {
                File.Delete(path);
                return true;
            }
            else return false;
        }
        public string SaveImage(string root, ProcessorResult image, string? thumbnailRoot, ProcessorResult? thumbnail)
        {
            if (image.Content is null)
            {
                throw new ArgumentNullException(nameof(image.Content));
            }
            string extension = image.GetExtensionFromFormat();
            string fileName = $"{Path.GetFileNameWithoutExtension(image.Name)}.{extension}";
            string key = GenerateKey(root, fileName);
            string path = CreatePath(root, key, extension);
            using (FileStream fileStream = File.Create(path))
            {
                fileStream.Write(image.Content, 0, image.Content.Length);
            }

            if (thumbnailRoot != null && thumbnail != null)
            {
                if (thumbnail.Content is null)
                {
                    throw new ArgumentNullException(nameof(thumbnail));
                }
                path = CreatePath(thumbnailRoot, key, thumbnail.GetExtensionFromFormat());
                using (FileStream fileStream = File.Create(path))
                {
                    fileStream.Write(thumbnail.Content, 0, thumbnail.Content.Length);
                }
            }
            return key;
        }
    }
}
