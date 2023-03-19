namespace Pasteimg.Server.Repository.FileStorages
{
    public interface IFileStorage
    {
        string Root { get; }
        int SubDirectoryDivision { get; }

        void ClearRoot();

        bool DeleteFile(string id);

        string? FindFile(string? id);

        void SaveFile(byte[] content, string id, string extension);
    }

    public class FileStorage : IFileStorage
    {

        public FileStorage(string root, int subDirectoryDivision)
        {
            Root = root;
            SubDirectoryDivision = subDirectoryDivision;
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

        public string? FindFile(string? id)
        {
            if (id is null) return null;

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

        public virtual void SaveFile(byte[] content, string id, string extension)
        {
            if (FindFile(id) is string path)
            {
                File.Delete(path);
            }

            Directory.CreateDirectory(GetDirectory(id));
            WriteFile(content, id, extension);
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

        protected void WriteFile(byte[] content, string id, string extension)
        {
            string path = GetPath(id, extension);
            File.WriteAllBytes(path, content);
        }
    }
}