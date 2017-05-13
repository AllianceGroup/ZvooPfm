using Default.Factories.ViewModels;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson.Serialization;
using mPower.Aggregation.Client;
using mPower.Domain.Membership.User;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Mvc.Validation;
using mPower.Framework.Registries;
using mPower.Framework.Utils.Security;
using mPower.OfferingsSystem;
using mPower.Schedule.Server.Environment;
using mPower.TempDocuments.Server.Notifications;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using mPower.TempDocuments.Server.Notifications.Documents.System;
using mPower.TempDocuments.Server.Notifications.Documents.Triggers;
using mPower.TempDocuments.Server.Notifications.Handlers;
using mPower.TempDocuments.Server.Notifications.Nuggets;
using NLog.Config;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using Quartz.Spi;
using StructureMap;
using Topshelf;
using TransUnionWrapper;
using IIdGenerator = mPower.Framework.Environment.IIdGenerator;

namespace mPower.Schedule.Server
{
    /// <summary>
    /// The server's main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            RegisterBsonMappers();
            var container = InitializeContainer();
            var host = HostFactory.New(x =>
            {
                x.Service<QuartzServer>(s =>
                {
                    s.ConstructUsing(builder =>
                    {
                        var server = container.GetInstance<QuartzServer>();
                        server.Initialize();
                        return server;
                    });
                    s.WhenStarted(server => server.Start());
                    s.WhenPaused(server => server.Pause());
                    s.WhenContinued(server => server.Resume());
                    s.WhenStopped(server => server.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription(Configuration.ServiceDescription);
                x.SetDisplayName(Configuration.ServiceDisplayName);
                x.SetServiceName(Configuration.ServiceName);
            });

            host.Run();
        }

        private static void RegisterBsonMappers()
        {
            BsonClassMap.RegisterClassMap<LowBalanceAlertDocument>();
            BsonClassMap.RegisterClassMap<LargePurchaseAlertDocument>();
            BsonClassMap.RegisterClassMap<AvailableCreditAlertDocument>();
            BsonClassMap.RegisterClassMap<UnusualSpendingAlertDocument>();
            BsonClassMap.RegisterClassMap<OverBudgetAlertDocument>();
            BsonClassMap.RegisterClassMap<CalendarEventAlertDocument>();
            BsonClassMap.RegisterClassMap<DashboardAlertDocument>();
            BsonClassMap.RegisterClassMap<NewAggregatedAccountTriggerNotification>();
            BsonClassMap.RegisterClassMap<NewCreditIdentityTriggerNotification>();
            BsonClassMap.RegisterClassMap<ManuallyCreatedNotification>();
        }


        private static Container InitializeContainer()
        {
            var container = new Container();
            container.Configure(config => config.For<IIdGenerator>().Use<MongoObjectIdGenerator>());
            new SettingsRegistry(container);
            new MongoRegistry(container);

            var settings = container.GetInstance<MPowerSettings>();
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));
            //
            // Service bus
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MsmqTransport()
                .SetName("Async Service Bus")
                .SetInputQueue(settings.SchedulerInputQueueName)
                .SetErrorQueue(settings.SchedulerErrorQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Event"), settings.SchedulerInputQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Command"), settings.SchedulerInputQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Message"), settings.SchedulerInputQueueName)
                .Dispatcher(d => d
                    .AddHandlers(typeof(UserAR).Assembly) //mpower.Domain assembly
                    .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly) //mPower.EventHandlers assembly
                    .AddHandlers(typeof(EmailHandler).Assembly) //temp documents assembly for emails
                )
            );

            container.Configure(config => config.For<IServiceBus>().Singleton().Use(bus));

            // 
            // Domain and Event store configuration
            //
            var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            var transitionsRepository = new MongoTransitionRepository(
                new AssemblyQualifiedDataTypeRegistry(),
                settings.MongoWriteDatabaseConnectionString);

            var transitionsStorage = new TransitionStorage(transitionsRepository);

            container.Configure(config =>
            {
                config.For<IEventService>().Use<EventService>();
                config.For<IEventBus>().Use<ParalectServiceBusEventBus>();
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<ISnapshotRepository>().Singleton().Use(new MongoSnapshotRepository(settings.MongoWriteDatabaseConnectionString));
                // We are using default implementation of repository
                config.For<IRepository>().Use<Repository>();
                config.For<IJobFactory>().Use(new IoCJobFactory(container));
                config.For<ITransUnionService>().Use<TransUnionService>();
                config.For<IObjectRepository>().Use<ObjectRepository>();
                config.For<IEncryptionService>().Use<EncryptionService>();
            });

            container.Configure(config =>
            {
                config.AddRegistry(new AggregationClientRegister());
                config.AddRegistry<OfferingSystemRegistry>();
            });

            container.Configure(config => config.Scan(scanner =>
            {
                scanner.Assembly(typeof(Program).Assembly);
                scanner.AddAllTypesOf<IScheduledJob>();
            }));

            //Scan for IObjectFactory<TInput,TOutput> and IValidator implementations
            container.Configure(config =>
            config.Scan(scanner =>
            {
                scanner.Assembly(settings.DefaultTenantAssembly);
                scanner.AssemblyContainingType<AccountsSidebarFactory>();
                scanner.AddAllTypesOf(typeof(IObjectFactory<,>));
                scanner.AddAllTypesOf(typeof(IValidator<>));
            }));

            container.Configure(config => config.Scan(scanner =>
            {
                scanner.Assembly(typeof(INugget).Assembly);
                scanner.AddAllTypesOf(typeof(INugget));
            }));
            container.Configure(config => config.For<IEmailHtmlBuilder>().Use<NuggetHtmlBuilder>());

            return container;
        }

    }
}
