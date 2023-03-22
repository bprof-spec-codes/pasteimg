using Pasteimg.Server.Configurations;
using Pasteimg.Server.Logic;
using Pasteimg.Server.Models.Entity;
using Pasteimg.Server.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteimg.Tests
{

    public class FakeRepository<TEntity> : KeyedCollection<object[], TEntity>,IRepository<TEntity>
        where TEntity:class,IEntity
    {
        class EqualityComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[]? x, object[]? y)
            {
                if(x is null && y is null)
                {
                    return true;
                }
                else if(x is not null && y is not null)
                {
                    return x.SequenceEqual(y);
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode([DisallowNull] object[] objs)
            {
                if(objs.Length==0)
                {
                    return 0.GetHashCode();
                }
                else
                {
                    int hash = objs[0].GetHashCode();
                    for (int i = 1; i < objs.Length; i++)
                    {
                        hash = HashCode.Combine(hash, objs[i].GetHashCode());
                    }
                    return hash;
                }
            }
        }
        public FakeRepository():base(new EqualityComparer())
        {

        }
        public void Create(TEntity item)
        {
            throw new NotImplementedException();
        }

        public TEntity? Delete(params object[] id)
        {
            throw new NotImplementedException();
        }

        public TEntity? Read(params object[] id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> ReadAll()
        {
            throw new NotImplementedException();
        }

     
        protected override object[] GetKeyForItem(TEntity item)
        {
            return item.GetKey();
        }

        public TEntity? Update(Action<TEntity> updateAction, params object[] id)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    internal class PasteImgLogicTest
    {
        PasteImgLogic logic;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DeleteImageTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void DeleteUploadTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllImageTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetAllUploadTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetImageTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetImageWithSourceFileTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetImageWithThumbnailFileTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetUploadTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetUploadWithSourceFilesTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetUploadWithThumbnailFilesTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void GetValidationConfigurationTest()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void PostUpload()
        {
            throw new NotImplementedException();
        }



    }
}
