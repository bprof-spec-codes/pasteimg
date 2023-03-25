using System.Collections.ObjectModel;

namespace Pasteimg.Backend.Configurations
{
    /// <summary>
    /// Represents the validation configuration settings.
    /// </summary>
    public class ValidationConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum length of the description for an uploaded image.
        /// </summary>
        public int DescriptionMaxLength { get; init; }

        /// <summary>
        /// Gets or sets the maximum file size, in bytes, for an uploaded image.
        /// </summary>
        public long MaxFileSize { get; init; }

        /// <summary>
        /// Gets or sets the maximum number of images allowed per upload.
        /// </summary>
        public int MaxImagePerUpload { get; init; }

        /// <summary>
        /// Gets or sets the maximum length of the password.
        /// </summary>
        public int PasswordMaxLength { get; init; }

        /// <summary>
        /// Gets or sets a collection of supported image formats.
        /// </summary>
        public ReadOnlyCollection<string> SupportedFormats { get; init; }
    }
}