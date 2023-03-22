using Microsoft.EntityFrameworkCore;
using Pasteimg.Server.Data;
using Pasteimg.Server.Models.Entity;

namespace Pasteimg.Server.Repository
{
    /// <summary>
    ///Perzisztens adattárolásért felelős interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Elmenti az elemet.
        /// </summary>
        void Create(TEntity item);
        /// <summary>
        /// Törli az elemet azonosító kulcs alapján, ha létezik és visszatér a törölt elemmel.
        /// </summary>
        TEntity? Delete(params object[] id);
        /// <summary>
        /// Megkeresi az elemet azonosító kulcs alapján.
        /// </summary>
        TEntity? Read(params object[] id);
        /// <summary>
        /// Lekérdezi az eltárolt elemeket.
        /// </summary>
        IQueryable<TEntity> ReadAll();
        /// <summary>
        /// Frissíti az elem állapotát kulcs alapján, ha létezik és visszatér a frissített elemmel.
        /// </summary>
        /// <param name="entity"></param>
        TEntity? Update(Action<TEntity> updateAction, params object[] id);
    }
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
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

        public TEntity? Read(params object[] id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public IQueryable<TEntity> ReadAll()
        {
            return context.Set<TEntity>();
        }

        public TEntity? Update(Action<TEntity> updateAction, params object[] id)
        {
            TEntity? entity = Read(id);
            if (entity != null)
            {
                updateAction(entity);
                context.SaveChanges();
            }
            return entity;
        }
    }
}