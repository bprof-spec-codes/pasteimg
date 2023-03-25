using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Repository;
using System.Collections.ObjectModel;

namespace Pasteimg.Backend.Configurations
{
    /// <summary>
    /// Represents the configuration settings for the Pasteimg server application.
    /// </summary>
    public class PasteImgConfiguration
    {
        /// <summary>
        /// Gets the default configuration for the Pasteimg server application.
        /// </summary>
        public static PasteImgConfiguration Default { get; } = new PasteImgConfiguration()
        {
            Visitor = new VisitorConfiguration()
            {
                LockoutTresholdInMinutes = 10,
                MaxFailedAttempt = 3
            },
            Validation = new ValidationConfiguration()
            {
                DescriptionMaxLength = 120,
                MaxFileSize = 10_000_000,
                MaxImagePerUpload = 20,
                SupportedFormats = new ReadOnlyCollection<string>(new string[]
                {
                "jpg","jpeg","jpe","jif","jfif","jfi",
                "gif","png","apng","webp","bmp"
                }),
                PasswordMaxLength = 12
            },
            Storage = new StorageSettings()
            {
                SubDirectoryDivision = 4,
                Root = new ReadOnlyCollection<string>(new string[] { "_wwwimages" }),
            },
            Source = new TransformationSettings()
            {
                MaxHeight = 2000,
                MaxWidth = 2000,
                Quality = 75
            },
            Thumbnail = new TransformationSettings()
            {
                MaxHeight = 300,
                MaxWidth = 300,
                Quality = 90
            }
        };

        /// <summary>
        /// Gets or sets the transformation settings for source image transformer.
        /// </summary>
        public TransformationSettings Source { get; init; }

        /// <summary>
        /// Gets or sets the filestorage settings.
        /// </summary>
        public StorageSettings Storage { get; init; }

        /// <summary>
        /// Gets or sets the transformation settings for thumbnail image transformer.
        /// </summary>
        public TransformationSettings Thumbnail { get; init; }

        /// <summary>
        /// Gets or sets the validation configuration.
        /// </summary>
        public ValidationConfiguration Validation { get; init; }

        /// <summary>
        /// Gets or sets the visitor configuration.
        /// </summary>
        public VisitorConfiguration Visitor { get; init; }
    }
}