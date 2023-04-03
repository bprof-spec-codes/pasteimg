using System.Reflection;

namespace Pasteimg.Backend.Repository
{
    /// <summary>
    /// Interface for a file storage implementation.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Gets the root directory of the file storage.
        /// </summary>
        string Root { get; }

        /// <summary>
        /// Gets the number of characters to be used to divide the files into subdirectories.
        /// </summary>
        int SubDirectoryDivision { get; }

        /// <summary>
        /// Deletes all files and subdirectories in the root directory.
        /// </summary>
        void ClearRoot();

        /// <summary>
        /// Deletes the file with the given ID and optional file class.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <param name="fileClass">The class of the file (optional).</param>
        /// <returns>True if the file was deleted, otherwise false.</returns>
        bool DeleteFile(string id, string? fileClass = null);

        /// <summary>
        /// Deletes all files with the given ID, regardless of their file class.
        /// </summary>
        /// <param name="id">The ID of the files to delete.</param>
        /// <returns>True if the files were deleted, otherwise false.</returns>
        bool DeleteFileWithAllClass(string id);

        /// <summary>
        /// Finds the path of a file with the given ID and optional file class.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <param name="fileClass">The class of the file (optional).</param>
        /// <returns>The path of the file, or null if the file was not found.</returns>
        string? FindPath(string? id, string? fileClass = null);

        /// <summary>
        /// Finds the paths of all files with the given ID, regardless of their file class.
        /// </summary>
        /// <param name="id">The ID of the files.</param>
        /// <returns>An array of file paths, or null if no files were found.</returns>
        string[]? FindPathWithAllClass(string? id);

        /// <summary>
        /// Gets the directory path of the file with the given ID.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <returns>The directory path of the file.</returns>
        string GetDirectory(string id);

        /// <summary>
        /// Returns the file name for the specified id and file class.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        string GetFileName(string id, string? fileClass = null);

        /// <summary>
        /// Returns the file path for the specified id, extension, and file class.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="extension">The file extension of the file. Optional.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        string GetPath(string id, string? extension = null, string? fileClass = null);

        /// <summary>
        /// Saves the specified file content to the file storage with the given id, extension, and file class.
        /// If a file with the same id and file class already exists, it will be overwritten.
        /// </summary>
        /// <param name="content">The content of the file to be saved.</param>
        /// <param name="id">The id of the file.</param>
        /// <param name="extension">The file extension of the file.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        void SaveFile(byte[] content, string id, string extension, string? fileClass = null);
    }

    /// <summary>
    /// Represents a file storage class that implements <see cref="IFileStorage"/> interface.
    ///     <example>
    ///          <para>
    ///             <strong>FileStorage routing examples: </strong>
    ///         </para>
    ///         <para>
    ///             <strong>GetDirectory: </strong>
    ///         </para>
    ///         <para><code><see cref="Root"/>: "root" </code></para>
    ///         <para><code><see cref="SubDirectoryDivision"/>: 3 </code></para>
    ///         <para><code><paramref name="id"/>: "abcdef" </code></para>
    ///         <para><strong>Output: </strong>  <code>"/root/abc/"</code></para>
    ///         <para>
    ///             <strong>GetPath: </strong>
    ///         </para>
    ///         <para><code><see cref="Root"/>: "root" </code></para>
    ///         <para><code><see cref="SubDirectoryDivision"/>: 5 </code></para>
    ///         <para><code><paramref name="id"/>: "testtest" </code></para>
    ///         <para><code><paramref name="extension"/>: "exe" </code></para>
    ///         <para><code><paramref name="fileClass"/>: "main" </code></para>
    ///         <para><strong>Output: </strong><code>"/root/testt/testtest_main.exe"</code></para>
    ///         <para>
    ///             <strong>GetPath</strong>
    ///         </para>
    ///         <para><code><see cref="Root"/>: "base" </code></para>
    ///         <para><code><see cref="SubDirectoryDivision"/>: 2 </code></para>
    ///         <para><code><paramref name="id"/>: "example" </code></para>
    ///         <para><code><paramref name="extension"/>: null </code></para>
    ///         <para><code><paramref name="fileClass"/>: null </code></para>
    ///         <para><strong>Output: </strong><code>"/base/ex/example"</code></para>
    ///     </example>
    /// </summary>

    public class FileStorage : IFileStorage
    {
        private static readonly string Bin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Initializes a new instance of the FileStorage class with the specified settings.
        /// </summary>
        /// <param name="settings">Settings of the file storage.</param>
        public FileStorage(StorageSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.Root is null)
            {
                throw new ArgumentNullException(nameof(settings.Root));
            }

            Root = Path.Combine(Bin, Path.Combine(settings.Root.ToArray()));
            SubDirectoryDivision = settings.SubDirectoryDivision;
        }

        public string Root { get; }
        public int SubDirectoryDivision { get; }

        /// <summary>
        /// Deletes all files and subdirectories in the root directory.
        /// </summary>
        public void ClearRoot()
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

        /// <summary>
        /// Deletes the file with the given ID and optional file class.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <param name="fileClass">The class of the file (optional).</param>
        /// <returns>
        /// True if the file was deleted, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Deletes all files with the given ID, regardless of their file class.
        /// </summary>
        /// <param name="id">The ID of the files to delete.</param>
        /// <returns>
        /// True if the files were deleted, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Finds the path of a file with the given ID and optional file class.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <param name="fileClass">The class of the file (optional).</param>
        /// <returns>
        /// The path of the file, or null if the file was not found.
        /// </returns>
        public string? FindPath(string? id, string? fileClass = null)
        {
            if (id is null) return null;

            string directory = GetDirectory(id);
            var t = Path.GetFullPath(Root);
            var d = Path.GetFullPath(directory);
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, GetFileName(id, fileClass) + ".*");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the paths of all files with the given ID, regardless of their file class.
        /// </summary>
        /// <param name="id">The ID of the files.</param>
        /// <returns>
        /// An array of file paths, or null if no files were found.
        /// </returns>
        public string[]? FindPathWithAllClass(string? id)
        {
            if (id is null) return null;

            string directory = GetDirectory(id);
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, id + "*");
                if (files.Length > 0)
                {
                    return files;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the directory path of the file with the given ID.
        /// </summary>
        /// <param name="id">The ID of the file.</param>
        /// <returns>
        /// The directory path of the file.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
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

        /// <summary>
        /// Returns the file name for the specified id and file class.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        /// <returns></returns>
        public string GetFileName(string id, string? fileClass = null)
        {
            if (fileClass is null)
            {
                return id;
            }
            else return id + "_" + fileClass;
        }

        /// <summary>
        /// Returns the file path for the specified id, extension, and file class.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="extension">The file extension of the file. Optional.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        /// <returns></returns>
        public string GetPath(string id, string? extension = null, string? fileClass = null)
        {
            return Path.Combine(GetDirectory(id), GetFileName(id, fileClass) + '.' + extension?.TrimStart('.') ?? "");
        }

        /// <summary>
        /// Saves the specified file content to the file storage with the given id, extension, and file class.
        /// If a file with the same id and file class already exists, it will be overwritten.
        /// </summary>
        /// <param name="content">The content of the file to be saved.</param>
        /// <param name="id">The id of the file.</param>
        /// <param name="extension">The file extension of the file.</param>
        /// <param name="fileClass">The file class of the file. Optional.</param>
        public void SaveFile(byte[] content, string id, string extension, string? fileClass = null)
        {
            if (FindPath(id, fileClass) is string path)
            {
                File.Delete(path);
            }

            Directory.CreateDirectory(GetDirectory(id));
            WriteFile(content, id, extension, fileClass);
        }

        /// <summary>
        /// Writes the specified byte array to a file with the given ID, extension and file class (if any) in the file storage.
        /// </summary>
        /// <param name="content">The byte array to be written to the file.</param>
        /// <param name="id">The ID of the file.</param>
        /// <param name="extension">The extension of the file.</param>
        /// <param name="fileClass">The optional file class of the file.</param>
        protected void WriteFile(byte[] content, string id, string extension, string? fileClass)
        {
            string path = GetPath(id, extension, fileClass);
            File.WriteAllBytes(path, content);
        }
    }
}