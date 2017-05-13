using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace mPower.Framework.Services
{
    public abstract class BaseMongoService<T, TFilter>
        where T : class
        where TFilter : BaseFilter
    {
        /// <summary>
        /// Use this property with Name method to avoid string names in queris to mongodb  
        /// </summary>
        public T Doc = null;

        public string Name<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        protected abstract MongoCollection Items { get; }

        protected abstract IEnumerable<IMongoQuery> BuildFilterQuery(TFilter filter);

        public IMongoQuery GetFilterQuery(TFilter filter)
        {
            return BuildFilterQuery(filter).Any() ? Query.And(BuildFilterQuery(filter).ToArray()) : Query.Null;
        } 

        protected virtual IMongoSortBy BuildSortExpression(TFilter filter)
        {
            return SortBy.Null;
        }

        public virtual string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public virtual List<T> GetByFilter(TFilter filter)
        {
            var queries = BuildFilterQuery(filter).ToList();
            //if filter was not applied we not return all documents, we just return empty list
            if (!queries.Any() && filter.PagingInfo == null)
                return new List<T>();

            var list = GetByQuery(queries, x =>
                                             {
                                                 var sortOrder = BuildSortExpression(filter);
                                                 if (sortOrder != SortBy.Null)
                                                     x.SetSortOrder(sortOrder);

                                                 if (filter.ExcludeFields.Count > 0)
                                                     x.SetFields(Fields.Exclude(filter.ExcludeFields.ToArray()));

                                                 if (filter.IsPagingEnabled)
                                                 {
                                                     var pagingInfo = filter.PagingInfo;
                                                     x.SetSkip(pagingInfo.Skip);
                                                     x.SetLimit(pagingInfo.Take);
                                                     pagingInfo.TotalCount = x.Count();
                                                 }

                                             });

            if (filter.IsPagingEnabled)
                filter.PagingInfo.ActualLoadedItemCount = list.Count;

            return list;
        }

        protected List<T> GetByQuery(IMongoQuery query, Action<MongoCursor<T>> action)
        {
            var cursor = Items.FindAs<T>(query);
            action(cursor);
            return cursor.ToList();
        }


        protected List<T> GetByQuery(List<IMongoQuery> queries, Action<MongoCursor<T>> action)
        {
            var cursor = queries.Any()? Items.FindAs<T>(Query.And(queries.ToArray())) : Items.FindAllAs<T>();
            action(cursor);
            return cursor.ToList();
        }

        public List<T> GetByQuery(IMongoQuery query)
        {
            var cursor = Items.FindAs<T>(query);
            return cursor.ToList();
        }

        public long Count(TFilter filter)
        {
            var query = BuildFilterQuery(filter).ToList();
            if (query.Any())
            {
                return Items.Count(Query.And(query.ToArray()));
            }
            return Items.Count();
        }

        public long Count()
        {
            return Items.Count(Query.Null);
        }

        public virtual T GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
                return Items.FindOneByIdAs<T>(id);
            return null;
        }

        public virtual BsonDocument GetBsonDocumentById(string id)
        {
            if (!String.IsNullOrEmpty(id))
                return Items.FindOneByIdAs<BsonDocument>(id);

            return null;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Items.FindAllAs<T>();
        }

        public virtual SafeModeResult Save(T document)
        {
           return Items.Save(document);
        }

        public virtual void Insert(T document)
        {
            Items.Insert(document);
        }

        public virtual void InsertMany(IEnumerable<T> documents)
        {
            Items.InsertBatch(documents);
        }

        public virtual void Update(IMongoQuery query, IMongoUpdate update)
        {
            Items.Update(query, update);
        }

        public virtual void UpdateMany(IMongoQuery query, IMongoUpdate update)
        {
            Items.Update(query, update, UpdateFlags.Multi);
        }

        public virtual void RemoveById(string id)
        {
            Items.Remove(Query.EQ("_id", id));
        }

        public virtual void RemoveAll()
        {
            Items.Drop();
        }

        public virtual void Remove(IMongoQuery query)
        {
            Items.Remove(query);
        }

        public virtual void Remove(TFilter filter)
        {
            Remove(GetFilterQuery(filter));
        }

        public virtual T FindOne()
        {
            return Items.FindOneAs<T>();
        }

        public virtual IQueryable<T> GetQuery()
        {
            return Items.AsQueryable<T>();
        }

        protected TRes FindAndModify<TRes>(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update)
        {
            var result = Items.FindAndModify(query, sortBy, update, true);
            var doc = result.GetModifiedDocumentAs<TRes>();

            return doc;
        }

        public MapReduceResult MapReduce(IMongoQuery query, BsonJavaScript map, BsonJavaScript reduce, IMongoMapReduceOptions mapReduceOptions)
        {
            return Items.MapReduce(query, map, reduce, mapReduceOptions);
        }
    }
}
