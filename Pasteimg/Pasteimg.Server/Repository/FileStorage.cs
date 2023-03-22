using Pasteimg.Server.Configurations;

namespace Pasteimg.Server.Repository
{
    public interface IFileStorage
    {
        string Root { get; }
        int SubDirectoryDivision { get; }

        void ClearRoot();

        bool DeleteFile(string id, string? fileClass = null);

        bool DeleteFileWithAllClass(string id);

        IFormFile? FindFile(string id, string contentType, string? fileClass = null);

        string? FindPath(string? id, string? fileClass = null);

        string[]? FindPathWithAllClass(string? id);

        string GetDirectory(string id);

        string GetFileName(string id, string? fileClass = null);

        string GetPath(string id, string? fileClass = null, string? extension = null);

        void SaveFile(byte[] content, string id, string extension, string? fileClass = null);
    }

    public class FileStorage : IFileStorage
    {
        public FileStorage(PasteImgConfiguration configuration)
        {
            Root = Path.Combine(configuration.Storage.Root.ToArray());
            SubDirectoryDivision = configuration.Storage.SubDirectoryDivision;
        }

        public string Root { get; }
        public int SubDirectoryDivision { get; }

        public virtual void ClearRoot()
        {
            if (Directory.Exists(Root))
            {
                if (SubDirectoryDivision > 0)
                {
                    foreach (var item in Directory.GetDirectories(Root))
                    {
                        Directory.Delete(item, true);
                    }
                }
                else
                {
                    foreach (var item in Directory.GetFiles(Root))
                    {
                        File.Delete(item);
                    }
                }
            }
        }

        public bool DeleteFile(string id, string? fileClass = null)
        {
            if (FindPath(id, fileClass) is string path)
            {
                File.Delete(path);
                if (SubDirectoryDivision > 0)
                {
                    DirectoryInfo dir = Directory.GetParent(path);
                    if (dir.GetFiles().Length == 0)
                    {
                        dir.Delete();
                    }
                }

                return true;
            }
            else
            {
                return false;
            };
        }

        public bool DeleteFileWithAllClass(string id)
        {
            if (FindPathWithAllClass(id) is string[] paths)
            {
                foreach (var path in paths)
                {
                    File.Delete(path);
                }
                if (SubDirectoryDivision > 0)
                {
                    DirectoryInfo dir = Directory.GetParent(paths[0]);
                    if (dir.GetFiles().Length == 0)
                    {
                        dir.Delete();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public IFormFile? FindFile(string id, string contentType, string? fileClass = null)
        {
            if (FindPath(id, fileClass) is string path)
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new FormFile(stream, 0, stream.Length, id, Path.GetFileName(path))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = $"{contentType}/{Path.GetExtension(path).TrimStart('.')}"
                };
            }
            else return null;
        }

        public string? FindPath(string? id, string? fileClass = null)
        {
            if (id is null) return null;

            string directory = GetDirectory(id);
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(GetDirectory(id), GetFileName(id, fileClass) + ".*");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }
            return null;
        }

        public string[]? FindPathWithAllClass(string? id)
        {
            if (id is null) return null;

            string directory = GetDirectory(id);
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(GetDirectory(id), id + "*");
                if (files.Length > 0)
                {
                    return files;
                }
            }
            return null;
        }

        public string GetDirectory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (SubDirectoryDivision > 0)
            {
                return Path.Combine(Root, id[..SubDirectoryDivision]);
            }
            else return Root;
        }

        public string GetFileName(string id, string? fileClass = null)
        {
            if (fileClass is null)
            {
                return id;
            }
            else return id + "_" + fileClass;
        }

        public string GetPath(string id, string? fileClass = null, string? extension = null)
        {
            return Path.Combine(GetDirectory(id), GetFileName(id, fileClass) + '.' + extension?.TrimStart('.') ?? "");
        }

        public void SaveFile(byte[] content, string id, string extension, string? fileClass = null)
        {
            if (FindPath(id, fileClass) is string path)
            {
                File.Delete(path);
            }

            Directory.CreateDirectory(GetDirectory(id));
            WriteFile(content, id, extension, fileClass);
        }

        protected void WriteFile(byte[] content, string id, string extension, string? fileClass)
        {
            string path = GetPath(id, extension, fileClass);
            File.WriteAllBytes(path, content);
        }
    }
}