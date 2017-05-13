using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace mPower.Framework
{
    public class InMemoryCollection
    {
        private Dictionary<string, object> _items = new Dictionary<string, object>();
        /// <summary>
        /// name of index -> index
        /// </summary>
        private Dictionary<string, InMemoryIndex> _indexes = new Dictionary<string, InMemoryIndex>();

        public void Set(string id, object doc)
        {
            _items[id] = doc;
        }

        public object Get(string id)
        {
            object value;
            if (!_items.TryGetValue(id, out value))
                return null;

            return value;
        }

        public IEnumerable<object> GetAll()
        {
            return _items.Values;
        }

        public object GetByIndex(string indexName, object indexValue)
        {
            return _indexes[indexName].Get(indexValue);
        }

        public void SetIndex(string indexName, object indexKey, object indexValue)
        {
            _indexes[indexName].Set(indexKey, indexValue);
        }

        public void Delete(string id)
        {
            _items.Remove(id);
        }

        public Type GetDocumentType()
        {
            return _items.Values.First().GetType();
        }
    }

    public class InMemoryDatabase
    {
        private readonly MongoRead _read;

        public InMemoryDatabase(MongoRead read)
        {
            _read = read;
        }

        private readonly Dictionary<string, InMemoryCollection> _collections = new Dictionary<string, InMemoryCollection>();

        public InMemoryCollection GetCollection(string collectionName)
        {
            InMemoryCollection collection;
            if (!_collections.TryGetValue(collectionName, out collection))
                _collections[collectionName] = collection = new InMemoryCollection();
            return collection;
        }

        public void FlushAll()
        {
            foreach (var collection in _collections)
            {
                if (!collection.Key.StartsWith("temp."))
                {
                    var mongoCollection = _read.GetCollection(collection.Key);
                    mongoCollection.InsertBatch(collection.Value.GetDocumentType(), collection.Value.GetAll());
                }
            }
        }
    }

    public class InMemoryIndex
    {
        /// <summary>
        /// index -> document
        /// </summary>
        private Dictionary<object, object> _records = new Dictionary<object, object>();

        public object Get(object id)
        {
            object value;
            if (!_records.TryGetValue(id, out value))
                return null;

            return value;
        }

        public void Set(object id, object doc)
        {
            _records[id] = doc;
        }
    }

}