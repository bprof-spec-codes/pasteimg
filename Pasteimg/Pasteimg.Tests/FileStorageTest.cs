using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pasteimg.Backend.Repository;

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
         
            throw new NotImplementedException();
        }
        [Test]
        public void ClearRootTest()
        {
            throw new NotImplementedException();


        }

        [Test]
        public void FindFileTest()
        {
            throw new NotImplementedException();

        }
        [Test]
        public void DeleteFileTest()
        {
            throw new NotImplementedException();

        }


        [Test]
        public void SubDirectoryDivisionTest()
        {
         
            throw new NotImplementedException();
        }
    }
}