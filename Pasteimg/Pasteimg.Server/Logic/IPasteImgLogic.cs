using Pasteimg.Server.Models;
using Pasteimg.Server.Repository;

namespace Pasteimg.Server.Logic
{
    public interface IPasteImgLogic
    {
        IBinaryHandler BinaryHandler { get; }
        int? MaxImageGroupSize { get; set; }
        INsfwRecognizer NsfwRecognizer { get; }

        void Create(byte[][] binaries, string? description = null, string? password = null);
        void Delete(string key);
        IEnumerable<BinaryImage> GetImageGroup(string groupKey);
        IEnumerable<BinaryImage> GetImageGroupFromUri(string uri);
        BinaryImage? Read(string key);
        IEnumerable<BinaryImage> ReadAll();
        void Update(string key, Action<BinaryImage> updateAction);
    }

    public class PasteImgLogic : IPasteImgLogic
    {
        private readonly IRepository<BinaryImage> repository;
        private readonly IPasswordHasher passwordHasher;
        private readonly IKeyGenerator keyGenerator;
        public int? MaxImageGroupSize { get; set; } = 50;
        public INsfwRecognizer NsfwRecognizer { get; }
        public IBinaryHandler BinaryHandler { get; }
        public PasteImgLogic(IRepository<BinaryImage> repository,
                             IPasswordHasher passwordHasher,
                             IKeyGenerator keyGenerator,
                             INsfwRecognizer nsfwRecognizer,
                             IBinaryHandler binaryHandler)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
            this.keyGenerator = keyGenerator;
            NsfwRecognizer = nsfwRecognizer;
            BinaryHandler = binaryHandler;

        }

        public void Create(byte[][] binaries, string? description = null, string? password = null)
        {
            DateTime timestamp = DateTime.Now;

            string? groupKey = binaries.Length > 1 ?
                keyGenerator.GenerateKey(ReadAll().Where(i => i.GroupKey != null)
                         .Select(i => i.GroupKey)
                         .Distinct()) : null;

            foreach (var binary in binaries)
            {
                BinaryImage image = new BinaryImage()
                {
                    Key = keyGenerator.GenerateKey(ReadAll().Select(i => i.Key)),
                    Content = BinaryHandler.Compress(binary),
                    Thumbnail = BinaryHandler.CreateThumbnail(binary),
                    GroupKey = groupKey,
                    Password = passwordHasher.CreateHash(password),
                    Description = description,
                    Timestamp = timestamp,
                    LastAccess = timestamp
                };
                repository.Create(image);
            }
        }
        public void Delete(string key)
        {
            repository.Delete(key);
        }

        public IEnumerable<BinaryImage> GetImageGroup(string groupKey)
        {
            return repository.ReadAll().Where(i => groupKey == i.GroupKey);
        }
        public IEnumerable<BinaryImage> GetImageGroupFromUri(string uri)
        {
            return GetImageGroup(uri.Split(Path.DirectorySeparatorChar)[^-1]);
        }

        public BinaryImage? Read(string key)
        {
            return repository.Read(key);
        }
        public void Update(string key, Action<BinaryImage> updateAction)
        {
            repository.Update(updateAction, key);
        }
        public IEnumerable<BinaryImage> ReadAll()
        {
            return repository.ReadAll();
        }

    }
}