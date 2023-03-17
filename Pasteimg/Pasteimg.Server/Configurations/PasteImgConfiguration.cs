﻿using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Pasteimg.Server.Configurations
{
    public class PasteImgConfiguration
    {
        public static PasteImgConfiguration Default { get; } = new PasteImgConfiguration()
        {
            Visitor = new VisitorConfiguration()
            {
                LockoutTimeInMinutes = 10,
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
                })
            },
            Storage = new StorageConfiguration()
            {
                SubDirectoryDivision = 4,
                ThumbnailRoot = Path.Combine("images", "thumbnail"),
                SourceRoot = Path.Combine("images", "source"),
            },
            Transformation = new TransformationConfiguration()
            {
                Quality = 75,
                SourceOptimizerMaxWidth = 2000,
                SourceOptimizerMaxHeight = 2000,
                ThumbnailMaxWidth = 300,
                ThumbnailMaxHeight = 300,
            }
        };

        public StorageConfiguration Storage { get; init; }

        public TransformationConfiguration Transformation { get; init; }

        public ValidationConfiguration Validation { get; init; }

        public VisitorConfiguration Visitor { get; init; }

        public static PasteImgConfiguration ReadConfiguration(string path)
        {
            PasteImgConfiguration? config = JsonConvert.DeserializeObject<PasteImgConfiguration>(File.ReadAllText(path));
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (config.Visitor is null)
            {
                throw new ArgumentNullException(nameof(config.Visitor));
            }
            if (config.Validation.SupportedFormats is null)
            {
                throw new ArgumentNullException(nameof(config.Validation.SupportedFormats));
            }
            if (config.Validation.MaxFileSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.Validation.MaxFileSize));
            }
            if (config.Validation.MaxImagePerUpload <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.Validation.MaxImagePerUpload));
            }
            return config;
        }

        public static void WriteConfiguration(string path, PasteImgConfiguration config)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}