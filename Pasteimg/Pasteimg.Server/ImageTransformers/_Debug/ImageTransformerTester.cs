using Microsoft.AspNetCore.Routing.Constraints;
using Pasteimg.Server.Configurations;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Pasteimg.Server.ImageTransformers._Debug
{

    public class ImageTransformerTester
    {
        static string InputDirectory { get; } = Path.Combine("wwwroot", "_debug", "test_input");
        static string SourceDirectory { get; } = Path.Combine("wwwroot", "_debug", "source_output");
        static string ThumbnailDirectory { get; } = Path.Combine("wwwroot", "_debug", "thumbnail_output");
        private readonly IImageTransformer sourceOptimizer;
        private readonly IImageTransformer thumbnailCreator;
        private readonly List<ImageTransformerTestResult> results;
        public ReadOnlyCollection<ImageTransformerTestResult> Results { get; }
        public ImageTransformerTester(IImageTransformerFactory factory)
        {
            sourceOptimizer = factory.CreateSourceOptimizer();
            thumbnailCreator = factory.CreateThumbnailCreator();
            results = new List<ImageTransformerTestResult>();
            Results = new ReadOnlyCollection<ImageTransformerTestResult>(results);
            TestOutputClear();
            ReadFiles();
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
        private void AddResult(string item)
        {
            TransformationResult source = ProcessFile(item, SourceDirectory, sourceOptimizer);
            TransformationResult thumbnail = ProcessFile(item, ThumbnailDirectory, thumbnailCreator);
            lock (resultsLock)
            {
                results.Add(new ImageTransformerTestResult(source, thumbnail));
            }
        }
        private TransformationResult ProcessFile(string path, string outputDir, IImageTransformer transformer)
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
        object resultsLock = new object();
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
    }
}