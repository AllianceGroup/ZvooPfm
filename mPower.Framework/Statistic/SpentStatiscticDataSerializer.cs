using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace mPower.Framework.Statistic
{
    public class StatisticDisctionary<TKey, TValue> : Dictionary<TKey, TValue>, IBsonSerializable
    {
        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            if (nominalType != typeof (StatisticDisctionary<TKey, TValue>))
                throw new ArgumentException("Cannot deserialize anything but self");
            var ser = new ArraySerializer<StatisticKeyValueDocument<TKey, TValue>>();
            return
                ((StatisticKeyValueDocument<TKey, TValue>[])
                 ser.Deserialize(bsonReader, typeof (StatisticKeyValueDocument<TKey, TValue>[]), options)).ToDictionary(
                     k => k.Key, v => v.Value);
        }

        public bool GetDocumentId(out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            id = null;
            idGenerator = null;
            idNominalType = null;
            return false;
        }

        public void Serialize(BsonWriter bsonWriter, Type nominalType, IBsonSerializationOptions options)
        {
            if (nominalType != typeof (StatisticDisctionary<TKey, TValue>))
                throw new ArgumentException("Cannot serialize anything but self");
            var ser = new ArraySerializer<StatisticKeyValueDocument<TKey, TValue>>();
            ser.Serialize(bsonWriter, typeof (StatisticKeyValueDocument<TKey, TValue>[]),
                          this.Select(x => new StatisticKeyValueDocument<TKey, TValue>
                                               {
                                                   Id = x.Key.ToString(),
                                                   Key = x.Key,
                                                   Value = x.Value
                                               }).ToArray(), options);
        }

        public void SetDocumentId(object id)
        {
            return;
        }
    }

    public class StatisticKeyValueDocument<TKey,TValue>
    {
        [BsonId]
        public string Id { get; set; }

        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}