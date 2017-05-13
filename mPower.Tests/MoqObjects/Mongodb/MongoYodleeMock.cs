using MongoDB.Bson;
using Moq;
using mPower.Framework;
using mPower.Tests.Environment;
using mPower.Framework.Mongo;

namespace mPower.Tests.MoqObjects.Mongodb
{
    public class MongoYodleeMock : IMock<MongoYodlee>
    {
        private readonly MPowerSettings _settings;

        public MongoYodleeMock(MPowerSettings settings)
        {
            _settings = settings;
            Create();
        }

        private Mock<MongoYodlee> _current;

        public Mock<MongoYodlee> Create()
        {
            _current = new Mock<MongoYodlee>(_settings.MongoYodleeDatabaseConnectionString + ObjectId.GenerateNewId().ToString());
            return _current;
        }

        public MongoYodlee Object
        {
            get
            {
                return _current.Object;
            }
        }
    }
}
