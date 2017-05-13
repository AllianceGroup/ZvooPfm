using System;
using System.IO;
using Default.Factories.ViewModels;
using mPower.Aggregation.Client;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.EventHandlers;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Mvc;
using mPower.Framework.Mvc.Validation;
using mPower.Framework.Registries;
using mPower.TempDocuments.Server.Notifications;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus.Dispatching;
using StructureMap;
using StructureMapServiceLocator = Paralect.ServiceLocator.StructureMap.StructureMapServiceLocator;

namespace mPower.Tests.Environment
{
    /// <summary>
    /// Bootstrapper for Test server
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// StructureMap container
        /// </summary>
        private IContainer _container;

        /// <summary>
        /// StructureMap container
        /// </summary>
        public IContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Run bootstrapping logic
        /// </summary>
        public void BootstrapStructureMap()
        {
            _container = new Container();

            new SettingsRegistry(_container);
            new MongoRegistry(_container);
            new TestServerRegistry(_container);

            var settings = _container.GetInstance<MPowerSettings>();
            var luceneIndexesDirectory = "LuceneIndexes";
            settings.LuceneIndexesDirectory = luceneIndexesDirectory;

            _container.Configure(config => config.ForSingletonOf<MPowerSettings>().Use(settings));

            if (Directory.Exists(luceneIndexesDirectory))
            {
                Directory.Delete(luceneIndexesDirectory, true);
            }
            _container.Configure(config =>
            {
                config.For<IEmailHtmlBuilder>().Use<NuggetHtmlBuilder>();
                config.For<IObjectRepository>().Use<ObjectRepository>();
                config.For<IAggregationClient>().Use<AggregationClient>();
                config.For<IAggregationCallback>().Use<AggregationCallback>();
            });

            var callback = _container.GetInstance<IAggregationCallback>();
            _container.Configure(config => config.AddRegistry(new AggregationClientRegister(callback)));

            _container.Configure(config => config.Scan(scanner =>
            {
                scanner.Assembly(settings.DefaultTenantAssembly);
                scanner.AssemblyContainingType<AccountsSidebarFactory>();
                scanner.AddAllTypesOf(typeof(IObjectFactory<,>));
                scanner.AddAllTypesOf(typeof(IValidator<>));
            }));

            DateTimeSerializationOptions.Defaults = DateTimeSerializationOptions.UtcInstance;
            KillDatabases(_container.GetInstance<MongoRead>());
        }

        public Dispatcher BootsrapDispatcher()
        {
            var _eventBus = new InMemoryEventBus();
            _container.Configure(config => config.For<IEventBus>().Use(_eventBus));

            var _eventDispatcher = Dispatcher.Create(d => d
                .SetServiceLocator(new StructureMapServiceLocator(_container))
                .AddHandlers(typeof(Ledger_CreateCommand).Assembly) // mPower.Domain assembly
                .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly) //mPower.EventHandlers
            );

            var eventService = new TestEventService(_eventDispatcher);
            _container.Configure(config => config.For<IEventService>().Use(eventService));

            return _eventDispatcher;
        }

        public void BootstrapWebTests()
        {
        }

        /// <summary>
        /// Convenient method to create new context
        /// </summary>
        public static IContainer Bootstrap()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.BootstrapStructureMap();

            return bootstrapper.Container;
        }

        public static void KillDatabases(MongoRead mongoRead)
        {
            var databases = mongoRead.AllDatabases();
            var id = ObjectId.Empty;
            foreach (var name in databases)
            {
                ObjectId.TryParse(name.Replace("mpower_read_", String.Empty), out id);
                ObjectId.TryParse(name.Replace("mpower_stage_read_", String.Empty), out id);
                if (id != ObjectId.Empty)
                {
                    mongoRead.GetDatabase(name).Drop();
                }
            }

        }
    }
}
