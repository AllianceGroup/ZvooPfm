using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;

namespace mPower.Framework.Mongo
{
    /// <summary>
    /// Do not search for ID property
    /// </summary>
    public class NoDefaultPropertyIdConvention : ConventionBase, IPostProcessingConvention
    {
        public void PostProcess(BsonClassMap classMap)
        {
            var type = classMap.ClassType.GetTypeInfo();
            if (!type.GetProperties().Any(prop => prop.IsDefined(typeof(BsonIdAttribute))))
            {
                classMap.SetIdMember(null);
            }
        }
    }
}