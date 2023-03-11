using Pasteimg.Server.Models;

namespace Pasteimg.Server.Repository.IFileStorage
{
    public interface IImageFileStorage:IFileStorage
    {
        int MaxHeight { get; }
        int MaxWidth { get; }
        int Quality { get; set; }

        OptimizationResult OptimizeImage(byte[] content, string id);
        Task<OptimizationResult> OptimizeImageAsync(byte[] content, string id);
        Task<OptimizationResult[]> OptimizeImagesAsync(IList<FileArgument> files);
        void SaveFiles(IEnumerable<OptimizationResult> results);
    }

    public class ImageFileStorage : FileStorage, IImageFileStorage
    {
        private IImageOptimizer optimizer;
        public ImageFileStorage(IWebHostEnvironment environment,
                                string root,
                                int subDirectoryDivision,
                                int maxWidth,
                                int maxHeight,
                                IImageOptimizer optimizer,
                                int quality) : base(environment, root, subDirectoryDivision)
        {
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            this.optimizer = optimizer;
            Quality = quality;
        }


        public int MaxHeight { get; }
        public int MaxWidth { get; }
        public int Quality { get; set; }


        public OptimizationResult OptimizeImage(byte[] content, string id)
        {
            return optimizer.Optimize(id, content, MaxWidth, MaxHeight, Quality);
        }

        public async Task<OptimizationResult> OptimizeImageAsync(byte[] content, string id)
        {
            return await Task.Run(() => optimizer.Optimize(id, content, MaxWidth, MaxHeight, Quality));

        }

        public async Task<OptimizationResult[]> OptimizeImagesAsync(IList<FileArgument> files)
        {
            Task<OptimizationResult>[] tasks = new Task<OptimizationResult>[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                FileArgument file = files[i];
                tasks[i] = OptimizeImageAsync(file.Content, file.Id);
            }
            return await Task.WhenAll(tasks);
        }
     
        public void SaveFiles(IEnumerable<OptimizationResult> results)
        {
            foreach (var item in results)
            {
                SaveFile(item.Content, item.Id, item.OptimizedExtension);
            }
        }


    }
}