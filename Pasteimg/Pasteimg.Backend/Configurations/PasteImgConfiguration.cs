using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Logic;
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
        /// <list type="bullet">
        /// <item>
        /// <see cref="Visitor"/>
        /// <list type="bullet">
        /// <item>
        ///     LockoutTresholdInMinutes: 10
        /// </item>
        /// <item>
        ///     MaxFailedAttempt: 3
        /// </item>
        /// </list>
        /// </item>
        /// <item>
        /// <see cref="Validation"/>
        /// <list type="bullet">
        /// <item>
        ///    DescriptionMaxLength: 150
        /// </item>
        /// <item>
        ///     MaxFileSize: 10000000 
        /// </item>
        /// <item>
        ///    MaxImagePerUpload: 20 
        /// </item>
        /// <item>
        ///     SupportedFormats: jpg, jpeg, jpe, jif, jfif, jfi, gif, png, apng, webp, bmp
        /// </item>
        /// <item>
        ///     PasswordMaxLength: 12
        /// </item>
        /// </list>
        /// </item>
        /// <item>
        /// <see cref="Storage"/>
        /// <list type="bullet">
        /// <item>
        ///     SubDirectoryDivision: 4
        /// </item>
        /// <item>
        ///     Root: "_wwwimages"
        /// </item>
        /// </list>
        /// </item>
        /// <item>
        /// <see cref="Source"/>
        /// <list type="bullet">
        /// <item>
        ///   MaxHeight: 2000
        /// </item>
        /// <item> 
        ///     MaxWidth: 2000
        /// </item>
        /// <item>
        ///     Quality: 75
        /// </item>
        /// </list>
        /// </item>
        /// <item>
        /// <see cref="Thumbnail"/>
        /// <list type="bullet">
        /// <item>
        ///     MaxHeight: 300
        /// </item>
        /// <item> 
        ///     MaxWidth: 300
        /// </item>
        /// <item>
        ///     Quality: 90
        /// </item>
        /// </list>
        /// </item>
        /// </list>
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
            },
            Session=new SessionSettings()
            {
                IdleTimeout=TimeSpan.FromMinutes(60),
                IOTimeout=TimeSpan.FromSeconds(30)
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

        public SessionSettings Session { get; init; }
    }
}