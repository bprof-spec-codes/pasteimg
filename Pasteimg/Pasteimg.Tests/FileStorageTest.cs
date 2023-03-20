using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pasteimg.Server.Repository.FileStorages;

namespace Pasteimg.Tests
{
    [TestFixture]
    public class FileStorageTest
    {

        const string RootDirectory = "TestRoot";

        [TearDown]
        public void TearDown()
        {
            if(Directory.Exists(RootDirectory))
            {
                Directory.Delete(RootDirectory, true);
            }
        }

        [Test]
        public void SaveFileTest()
        {
            string id = "test";
            string ext = "test";
            byte[] content = new byte[40];
            string path = Path.Combine(RootDirectory, id + "." + ext);
            new Random().NextBytes(content);
            
            FileStorage fileStorage = new FileStorage(RootDirectory, 0);
            fileStorage.SaveFile(content,id,ext);
            Assert.That(File.Exists(path),"save");

            byte[] readedContent=File.ReadAllBytes(path);
            Assert.That(content.SequenceEqual(readedContent), "writed-readed contents equal");
        }
        [Test]
        public void ClearRootTest()
        {

            FileStorage fileStorage = new FileStorage(RootDirectory,0);
            for (int i = 0; i < 10; i++)
            {
                fileStorage.SaveFile(new byte[] { }, i.ToString(), "test");
            }
            fileStorage.ClearRoot();


            Assert.That(Directory.GetFiles(RootDirectory).Length == 0, "clear");

        }
        
        [Test]
        public void UniqueIdTest()
        {
            string id = "test";
            string ext1 = "test1";
            string ext2 = "test2";
            byte[] content = new byte[] {};

            FileStorage fileStorage = new FileStorage(RootDirectory, 0);
            fileStorage.SaveFile(content, id, ext1);
            fileStorage.SaveFile(content, id, ext2);
            Assert.That(!File.Exists(Path.Combine(RootDirectory, id + "." + ext1)),ext1);
            Assert.That(File.Exists(Path.Combine(RootDirectory, id + "." + ext2)),ext2);
        }


        [Test]
        public void FindFileTest()
        {
            string existId = "exist";
            string nonExistentId = "nonExist";
            string ext = "test";
            FileStorage fileStorage = new FileStorage(RootDirectory, 0);
            fileStorage.SaveFile(new byte[] { }, existId, ext);
            Assert.That(fileStorage.FindFile(existId) is not null, existId);
            Assert.That(fileStorage.FindFile(nonExistentId) is null, nonExistentId);
        }
        [Test]
        public void DeleteFileTest()
        {
            string id = "test";
            string ext = "test";
            FileStorage fileStorage = new FileStorage(RootDirectory, 0);
            fileStorage.SaveFile(new byte[] { }, id, ext);
            Assert.That(fileStorage.DeleteFile(id), "delete");
            Assert.That(!File.Exists(Path.Combine(RootDirectory, id + "." + ext)), "deleted file exists");
            Assert.That(!fileStorage.DeleteFile(id), "delete deleted");
        }


        [Test]
        public void SubDirectoryDivisionTest()
        {
            string id = "test123test";
            string ext = "test";
            byte[] content = new byte[] { };
            for (int i = 1; i <=id.Length; i++)
            {
                string subDir = id.Substring(0, i);
                string path = Path.Combine(RootDirectory, subDir, id + '.' + ext);

                FileStorage fileStorage = new FileStorage(RootDirectory, i);
                fileStorage.SaveFile(content, id, ext);
                Assert.That(File.Exists(path), $"save {i}");
                Assert.That(fileStorage.FindFile(id) is not null, $"find {i}");
                Assert.That(fileStorage.DeleteFile(id), $"delete {i}");
                Assert.That(!File.Exists(path), $"deleted file exists {i}");
                Assert.That(fileStorage.FindFile(id) is null, $"find deleted {i}");
                Assert.That(!fileStorage.DeleteFile(id), $"delete deleted {i}");
                fileStorage.ClearRoot();
                Assert.That(Directory.GetDirectories(RootDirectory).Length == 0, $"clear {i}");
            }
        }



    }
}