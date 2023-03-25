using System.Collections.ObjectModel;

namespace Pasteimg.Backend.Repository
{
    /// <summary>
    /// Represents settings for file storage.
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        /// Gets the components of root directory path for storing data.
        /// </summary>
        public ReadOnlyCollection<string> Root { get; init; }

        /// <summary>
        /// Gets the number of characters to be used to divide the files into subdirectories.
        /// </summary>
        public int SubDirectoryDivision { get; init; }
    }
}