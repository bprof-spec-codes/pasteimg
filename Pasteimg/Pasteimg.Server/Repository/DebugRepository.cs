using Pasteimg.Server.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Pasteimg.Server.Repository
{
    public class DebugRepository<TEntity> : IRepository<TEntity> where TEntity : class,IEntity
    {
        class EntityCollection : KeyedCollection<object[], TEntity>
        {

            public EntityCollection() : base()
            {

            }
            protected override object[] GetKeyForItem(TEntity item)
            {
                return item.GetKey();
            }
        }

        class EqualityComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[]? x, object[]? y)
            {
                if (x != null && y != null)
                {
                    return x.SequenceEqual(y);
                }
                else if (x == null && y == null)
                {
                    return true;
                }
                else return false;
            }

            public int GetHashCode([DisallowNull] object[] obj)
            {
                int hash = 0;
                if (obj != null)
                {
                        for (int i = 0; i < obj.Length; i++)
                        {
                        hash ^= obj[i].GetHashCode();
                        }
                }
                return hash;
            }
        }

        EntityCollection repository;
        public DebugRepository()
        {
            repository = new EntityCollection();
        }

        public void Create(TEntity item)
        {
            repository.Add(item);
        }

        public TEntity? Delete(params object[] id)
        {
            if(repository.TryGetValue(id,out TEntity? entity))
            {
                repository.Remove(entity);
            }
            return entity;
        }

        public TEntity? Read(params object[] id)
        {
            repository.TryGetValue(id, out TEntity? entity);
            return entity;
        }

        public IQueryable<TEntity> ReadAll()
        {
            return repository.AsQueryable();
        }

        public void Update(Action<TEntity> updateAction, params object[] id)
        {
           if(repository.TryGetValue(id,out TEntity? item))
           {
                updateAction(item);
           }
        }
    }
}