using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Default.Areas.Api;
using MongoDB.Bson;
using Moq;
using mPower.Documents.DocumentServices.Membership;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using NUnit.Framework;
using Paralect.Config.Settings;
using Paralect.Domain;
using Paralect.ServiceBus;
using StructureMap;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public abstract class ApiTest
    {
        protected IContainer _container;
        protected UserDocumentService _usersService;

        [TestFixtureSetUp]
        public void Setup()
        {
            _container = new Container();
            TenantTools.Selector = new TenantSelector(new List<IApplicationTenant>());
            TenantTools.Selector.TenantsContainer = _container;
            _container.Configure(config =>
            {
                var settings = SettingsMapper.Map<MPowerSettings>();
                config.For<MPowerSettings>().Singleton().Use(settings);

                var sessionId = String.Format("mpower_{0}", ObjectId.GenerateNewId());
                var mongoRead = new MongoRead(settings.MongoTestReadDatabaseConnectionString + sessionId);
                config.For<MongoRead>().Singleton().Use(mongoRead);
                var mongoTemp = new MongoTemp(settings.MongoTempDatabaseConnectionString + sessionId);
                config.For<MongoTemp>().Singleton().Use(mongoTemp);

                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
            });

            var bus = new Mock<IServiceBus>();
            _container.Configure(x => x.For<IServiceBus>().Singleton().Use(bus.Object));

            var asyncBus = new AsyncServiceBus(bus.Object);
            _container.Configure(x => x.For<AsyncServiceBus>().Singleton().Use(asyncBus));

            var irepository = new Mock<IRepository>();
            _container.Configure(x => x.For<IRepository>().Singleton().Use(irepository.Object));

            var commandService = new CommandService(bus.Object, asyncBus, new MongoObjectIdGenerator());
            _container.Configure(x => x.For<ICommandService>().Singleton().Use(commandService));
            _container.Configure(x => x.For<IEventService>().Singleton().Use<EventService>());

            _usersService = _container.GetInstance<UserDocumentService>();

            PrepareReadModel();
        }

        [TestFixtureTearDown]
        public void CleanUp()
        {
            var mongoRead = _container.GetInstance<MongoRead>();
            mongoRead.Database.Drop();
        }

        protected ApiResponseObject ExecuteApiAction(Func<ActionResult> controllerAction)
        {
            var actionResult = controllerAction();

            var jsonResult = (JsonResult)actionResult;

            return (ApiResponseObject)jsonResult.Data;
        }

        public abstract void PrepareReadModel();

        public abstract void Test();
    }
}
