using System.Collections.ObjectModel;

namespace Pasteimg.Server.Configurations
{
    public class ValidationConfiguration
    {
        public int DescriptionMaxLength { get; init; }
        public long MaxFileSize { get; init; }
        public int MaxImagePerUpload { get; init; }
        public ReadOnlyCollection<string> SupportedFormats { get; init; }
    }
}