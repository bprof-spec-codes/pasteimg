using Microsoft.EntityFrameworkCore;
using Pasteimg.Backend.Data;
using Pasteimg.Backend.Models.Entity;

namespace Pasteimg.Backend.Repository
{
    /// <summary>
    /// A generic repository interface that provides methods to create, read, update, and delete entities that implement the <see cref="IEntity"/> interface.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to perform CRUD operations on.</typeparam>
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Adds a new entity to the database and saves changes.
        /// </summary>
        /// <param name="item">The entity to add to the database.</param>
        void Create(TEntity item);

        /// <summary>
        /// Removes an entity from the database by its id and returns the deleted entity. Returns null if the entity does not exist in the database.
        /// </summary>
        /// <param name="id">The id of the entity to delete from the database.</param>
        /// <returns>The deleted entity, or null if the entity does not exist in the database.</returns>
        TEntity? Delete(params object[] id);

        /// <summary>
        /// Retrieves an entity from the database by its id. Returns null if the entity does not exist in the database.
        /// </summary>
        /// <param name="id">The id of the entity to retrieve from the database.</param>
        /// <returns>The retrieved entity, or null if the entity does not exist in the database.</returns>
        TEntity? Read(params object[] id);

        /// <summary>
        /// Retrieves all entities of a given type from the database.
        /// </summary>
        /// <returns>An IQueryable of all entities of the given type in the database.</returns>
        IQueryable<TEntity> ReadAll();

        /// <summary>
        /// Updates an existing entity in the database by its id and saves changes. Returns null if the entity does not exist in the database.
        /// </summary>
        /// <param name="updateAction">An action to perform on the entity before saving changes to the database.</param>
        /// <param name="id">The id of the entity to update in the database.</param>
        /// <returns>The updated entity, or null if the entity does not exist in the database.</returns>
        TEntity? Update(Action<TEntity> updateAction, params object[] id);
    }

    /// <summary>
    /// A generic repository implementation using Entity Framework Core.
    /// </summary>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private DbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class with the specified <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The application database context to use for data access.</param>
        public Repository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public void Create(TEntity item)
        {
            context.Set<TEntity>().Add(item);
            context.SaveChanges();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public TEntity? Read(params object[] id)
        {
            return context.Set<TEntity>().Find(id);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> ReadAll()
        {
            return context.Set<TEntity>();
        }

        /// <inheritdoc/>
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