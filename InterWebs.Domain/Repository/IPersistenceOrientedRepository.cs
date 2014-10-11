using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace InterWebs.Domain.Repository
{
    public interface IPersistenceOrientedRepository<TEntity> where TEntity : Entity
    {
        bool Insert(TEntity item);

        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> query);

        MongoCollection<TEntity> GetCollection();
    }
}