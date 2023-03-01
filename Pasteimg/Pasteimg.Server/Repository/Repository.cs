using Microsoft.EntityFrameworkCore;
using Pasteimg.Server.Data;

namespace Pasteimg.Server.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext context;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public void Create(TEntity item)
        {
            context.Set<TEntity>().Add(item);
            context.SaveChanges();
        }

        public TEntity? Delete(params object[] id)
        {
            TEntity? entity = Read(id);
            if (entity != null)
            {
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }
            return entity;
        }

        public void Update(Action<TEntity> updateAction, params object[] id)
        {
            TEntity? entity = Read(id);
            if (entity != null)
            {
                updateAction(entity);
                context.SaveChanges();
            }
        }
        public TEntity? Read(params object[] id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public IQueryable<TEntity> ReadAll()
        {
            return context.Set<TEntity>();
        }


    }
}