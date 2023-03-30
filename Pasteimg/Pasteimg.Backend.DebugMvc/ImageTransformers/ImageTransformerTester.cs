using Pasteimg.Backend.Configurations;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Pasteimg.Backend.ImageTransformers;

namespace Pasteimg.Backend.DebugMvc.ImageTransformers
{
    public class ImageTransformerTester
    {
        private readonly List<ImageTransformerTestResult> results;
        private readonly ImageTransformer sourceOptimizer;
        private readonly ImageTransformer thumbnailCreator;
        private object resultsLock = new object();

        public ImageTransformerTester(PasteImgConfiguration config, IImageTransformerFactory factory)
        {
            sourceOptimizer = factory.CreateSourceOptimizer(config.Source);
            thumbnailCreator = factory.CreateThumbnailCreator(config.Thumbnail);
            results = new List<ImageTransformerTestResult>();
            Results = new ReadOnlyCollection<ImageTransformerTestResult>(results);
            TestOutputClear();
            ReadFiles();
        }

        public ReadOnlyCollection<ImageTransformerTestResult> Results { get; }
        private static string InputDirectory { get; } = Path.Combine("wwwroot", "test_input");
        private static string SourceDirectory { get; } = Path.Combine("wwwroot","source_output");
        private static string ThumbnailDirectory { get; } = Path.Combine("wwwroot", "thumbnail_output");

        private void AddResult(string item)
        {
            TransformationResult source = ProcessFile(item, SourceDirectory, sourceOptimizer);
            TransformationResult thumbnail = ProcessFile(item, ThumbnailDirectory, thumbnailCreator);
            lock (resultsLock)
            {
                results.Add(new ImageTransformerTestResult(source, thumbnail));
            }
        }

        private TransformationResult ProcessFile(string path, string outputDir, ImageTransformer transformer)
        {
            string newPath = Path.Combine(outputDir, Path.GetFileName(path));
            Stopwatch sw = new Stopwatch();

            var original = transformer.GetImageInfo(path);
            original = original with { Path = path.Replace("wwwroot", "") };
            sw.Start();
            newPath = transformer.Transform(File.ReadAllBytes(path), newPath);
            sw.Stop();
            var transformed = transformer.GetImageInfo(newPath);
            transformed = transformed with { Path = transformed.Path.Replace("wwwroot", "") };
            return new TransformationResult(original, transformed, sw.Elapsed);
        }

        private void ReadFiles()
        {
            string[] files = Directory.GetFiles(InputDirectory);

            Directory.CreateDirectory(SourceDirectory);
            Directory.CreateDirectory(ThumbnailDirectory);
            foreach (var item in files)
            {
                _ = Task.Run(() => AddResult(item));
            }
        }

        private void TestOutputClear()
        {
            if (Directory.Exists(SourceDirectory))
            {
                foreach (var item in Directory.GetFiles(SourceDirectory))
                {
                    File.Delete(item);
                }
            }
            if (Directory.Exists(ThumbnailDirectory))
            {
                foreach (var item in Directory.GetFiles(ThumbnailDirectory))
                {
                    File.Delete(item);
                }
            }
        }
    }
}