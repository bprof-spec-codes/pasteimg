﻿using Pasteimg.Server.Models;

namespace Pasteimg.Server.Repository
{
    public interface IRepository<TEntity> where TEntity : class,IEntity
    {
        void Create(TEntity item);
        TEntity? Delete(params object[] id);
        TEntity? Read(params object[] id);
        IQueryable<TEntity> ReadAll();
        void Update(Action<TEntity> updateAction, params object[] id);
    }
}