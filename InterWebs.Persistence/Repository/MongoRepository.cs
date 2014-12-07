using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterWebs.Domain;
using InterWebs.Domain.Repository;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace InterWebs.Persistence.Repository
{
    public class MongoRepository<TEntity> : IPersistenceOrientedRepository<TEntity> where TEntity : Entity
    {
        public virtual bool Insert(TEntity item)
        {
            var result = GetCollection().Insert(item);
            return result.Ok;
        }

        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> query)
        {
            return GetCollection().AsQueryable().Where(query);
        }

        private MongoCollection<TEntity> collection;

        public MongoCollection<TEntity> GetCollection()
        {
            return collection ?? (collection = GetCollection<TEntity>());
        }

        private MongoCollection<TType> GetCollection<TType>()
        {
            var mongoDb = MongoConnectionSettings.GetDatabase();
            var collectionName = typeof(TEntity).Name;
            if (!mongoDb.CollectionExists(collectionName))
            {
                mongoDb.CreateCollection(collectionName);
            }

            return mongoDb.GetCollection<TType>(collectionName);
        }
    }
}