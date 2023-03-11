namespace Pasteimg.Server.Repository.IFileStorage
{
    public class FileArgument
    {
        public byte[] Content { get; set; }
        public string Id { get; set; }
        public string Extension { get; set; }
        public FileArgument(byte[] content, string id, string extension)
        {
            Content = content;
            Id = id;
            Extension = extension;
        }
    }

    public interface IFileStorage
    {
        string Root { get; }
        int SubDirectoryDivision { get; }
        IReadOnlySet<string> SupportedFiles { get; }

        void ClearRoot();
        bool DeleteFile(string id);
        string? FindWebFile(string id);
        void SaveFile(byte[] content, string id, string extension);
        void SaveFiles(IEnumerable<FileArgument> files);
    }

    public class FileStorage : IFileStorage
    {
        protected readonly HashSet<string> _supportedFiles;
        public IReadOnlySet<string> SupportedFiles => _supportedFiles;
        protected string WebRoot { get; }
        public FileStorage(IWebHostEnvironment environment, string root, int subDirectoryDivision)
        {
            Root = root;
            SubDirectoryDivision = subDirectoryDivision;
            _supportedFiles = new HashSet<string>();
            WebRoot = environment.WebRootPath.Split(Path.DirectorySeparatorChar)[^1];
            Root = Path.Combine(WebRoot, Root);
            Directory.CreateDirectory(Root);
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

        public virtual bool DeleteFile(string id)
        {
            if (FindFile(id) is string path)
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            };
        }

        public string? FindWebFile(string id)
        {
            if (FindFile(id) is string path)
            {
                return path.Replace(WebRoot, "").Replace(Path.DirectorySeparatorChar, '/');
            }
            else return null;
        }

        protected string? FindFile(string id)
        {
            string directory = GetDirectory(id);
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(GetDirectory(id), $"{id}.*");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }
            return null;

        }

        protected string GetDirectory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (SubDirectoryDivision > 0)
            {
                return Path.Combine(Root, id[..SubDirectoryDivision]);
            }
            else return Path.Combine(Root);
        }

        protected string GetPath(string id, string extension)
        {
            return Path.Combine(GetDirectory(id), $"{id}.{extension.TrimStart('.')}");
        }


        public virtual void SaveFile(byte[] content, string id, string extension)
        {
            if (FindFile(id) is string path)
            {
                File.Delete(path);
            }

            Directory.CreateDirectory(GetDirectory(id));
            WriteFile(content, id, extension);
        }

      
        public void SaveFiles(IEnumerable<FileArgument> files)
        {
            foreach (var item in files)
            {
                SaveFile(item.Content, item.Id, item.Extension);
            }
        }

    
        protected void WriteFile(byte[] content, string id, string extension)
        {
            string path = GetPath(id, extension);
            File.WriteAllBytes(path, content);
        }

      

    }
}