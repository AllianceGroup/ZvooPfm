using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace mPower.Framework.Mongo
{
    public static class MongoConvention
    {
        public static void Configure()
        {
            var serializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
            BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);
            
            var conventions = new ConventionPack { new NoDefaultPropertyIdConvention() };
            ConventionRegistry.Register("id conventions", conventions, t => true);
        }
    }
}