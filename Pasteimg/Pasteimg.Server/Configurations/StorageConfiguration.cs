using System.Collections.ObjectModel;

namespace Pasteimg.Server.Configurations
{
    public class StorageConfiguration
    {
        public ReadOnlyCollection<string> Root { get; init; }
        public string SourceFileClass { get; init; }
        public int SubDirectoryDivision { get; init; }
        public string TempFileClass { get; init; }
        public string ThumbnailFileClass { get; init; }
    }
}