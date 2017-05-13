using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson.Serialization;
using mPower.Aggregation.Client;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.Ledger;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Patches;
using mPower.EventHandlers;
using mPower.EventHandlers.ComplexPatches;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mongo;
using mPower.Framework.Mvc;
using mPower.Framework.Registries;
using mPower.Framework.Services;
using mPower.Framework.Utils;
using mPower.Framework.Utils.Security;
using mPower.TempDocuments.Server.Notifications;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;
using IIdGenerator = mPower.Framework.Environment.IIdGenerator;
using Repository = Paralect.Domain.Repository;
using StructureMapServiceLocator = Paralect.ServiceLocator.StructureMap.StructureMapServiceLocator;

namespace mPower.Web.Admin
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
        public void BootstrapStructureMap(IContainer container)
        {
            _container = container;

            new SettingsRegistry(_container);

            new MongoRegistry(_container);

            var settings = _container.GetInstance<MPowerSettings>();

            // Id generator
            _container.Configure(config =>
            {
                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
                config.For<IEmailHtmlBuilder>().Use<NuggetHtmlBuilder>();
                config.For<LedgerDocumentService>().Use<LedgerDocumentService>();
            });

            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));
            //
            // Service bus
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MemorySynchronousTransport()
                .SetName("Main Service Bus")
                .SetInputQueue("admin.sync.queue")
                .AddEndpoint(type => type.FullName.EndsWith("Event"), "admin.sync.queue")
                .AddEndpoint(type => type.FullName.EndsWith("Command"), "admin.sync.queue")
                .AddEndpoint(type => type.FullName.EndsWith("Message"), "admin.sync.queue")
                .Dispatcher(d => d
                    .AddHandlers(typeof(LedgerAR).Assembly)
                    .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly) 
                )
            );

            _container.Configure(config => config.For<IServiceBus>().Singleton().Use(bus));

            // 
            // Domain and Event store configuration
            //
            var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            var transitionsRepository = new MongoTransitionRepository(
                new AssemblyQualifiedDataTypeRegistry(),
                settings.MongoWriteDatabaseConnectionString);

            var transitionsStorage = new TransitionStorage(transitionsRepository);
            var encrypt = new EncryptionService();
            var mongoUtil = new MongoUtil(settings.PathToBackuper, settings.PathToRestorer, new CommonUtil());

            _container.Configure(config =>
            {
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<ITransitionRepository>().Singleton().Use(transitionsRepository);
                config.For<ISnapshotRepository>().Singleton().Use(new MongoSnapshotRepository(settings.MongoWriteDatabaseConnectionString));
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<IEventBus>().Use<ParalectServiceBusEventBus>();
                config.For<IEventService>().Use<EventService>();
                config.For<IObjectRepository>().Use<ObjectRepository>();
                config.For<ICommandService>().Use<CommandService>();
                config.For<IEncryptionService>().Singleton().Use(encrypt);

                // We are using default implementation of repository
                config.For<IRepository>().Use<Repository>();
                config.For<MongoUtil>().Singleton().Use(mongoUtil);
                config.For<BaseMongoService<NLogMongoTarget.NlogMongoItem, NLogMongoFilter>>().Use<NLogMongoService>();
                config.Scan(scanner =>
                {
                    scanner.Assembly(typeof(IPatch).Assembly);
                    // patches that use not only write model
                    // should not be used in regeneration (only from admin area)
                    scanner.Assembly(typeof(Patch6RestoreInstitutionId).Assembly);
                    scanner.AddAllTypesOf<IPatch>();
                });
                config.For<IAggregationCallback>().Use<AggregationCallback>();
            });

            var callback = container.GetInstance<IAggregationCallback>();
            container.Configure(configure =>
            {
                configure.For<IAggregationClient>().Singleton().Use(new AggregationClientFactory(callback).GetClient());
            });


            BsonClassMap.RegisterClassMap<TaxLien>();
            BsonClassMap.RegisterClassMap<Bankruptcy>();
            BsonClassMap.RegisterClassMap<LegalItem>();
            BsonClassMap.RegisterClassMap<Transaction_CreatedEvent>();
        }
    }
}
