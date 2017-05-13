using System;
using System.Collections.Generic;
using MongoDB.Bson;
using mPower.Framework;
using mPower.Tests.Environment;
using mPower.Tests.UnitTests.EventHandlers;
using NUnit.Framework;
using StructureMap;

namespace mPower.Tests.UnitTests.Services
{
    [TestFixture]
    public class BaseServiceTest
    {
        protected string _id = ObjectId.GenerateNewId().ToString();
        protected static DateTime CurrentDate = new DateTime(2011, 11, 11, 10, 10, 0, 0, DateTimeKind.Utc);

        protected IContainer _container;
        protected MongoRead _mongoRead;

        public virtual IEnumerable<Object> Given()
        {
            yield break;
        }

        [SetUp]
        public virtual void Setup()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.BootstrapStructureMap();
            bootstrapper.BootstrapWebTests();
            _container = bootstrapper.Container;

            var sessionId = String.Format("_{0}", ObjectId.GenerateNewId());
            var settings = _container.GetInstance<MPowerSettings>();
            var mongoRead = new MongoRead(settings.MongoReadDatabaseConnectionString + sessionId);
            mongoRead.Database.Drop();
            _container.Configure(x => x.For<MongoRead>().Singleton().Use(mongoRead));

            foreach (Object givenDoc in Given())
            {
                var collection = BaseHandlerTest.GetCollection(givenDoc, mongoRead);
                collection.Insert(givenDoc);
            }
        }

        [TearDown]
        public virtual void Cleanup()
        {
            var mongoRead = _container.GetInstance<MongoRead>();
            mongoRead.Database.Drop();
        }
    }
}
