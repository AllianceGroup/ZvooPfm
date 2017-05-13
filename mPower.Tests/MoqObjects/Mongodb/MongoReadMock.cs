using MongoDB.Bson;
using Moq;
using mPower.Framework;
using mPower.Tests.Environment;
using mPower.Framework.Mongo;

namespace mPower.Tests.MoqObjects.Mongodb
{
    public class MongoReadMock : IMock<MongoRead>
    {
        private readonly MPowerSettings _settings;

        public MongoReadMock(MPowerSettings settings)
        {
            _settings = settings;
            Create();
        }

        private Mock<MongoRead> _current;

        public Mock<MongoRead> Create()
        {
            _current = new Mock<MongoRead>(_settings.MongoTestReadDatabaseConnectionString + ObjectId.GenerateNewId().ToString());
            return _current;
        }

        public MongoRead Object
        {
            get
            {
                return _current.Object;
            }
        }
    }
}
