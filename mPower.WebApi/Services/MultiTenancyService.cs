using System.Linq;
using System.Reflection;
using Default;
using Default.Factories.ViewModels;
using Default.Helpers;
using mPower.Aggregation.Client;
using mPower.Documents;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Segments;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger;
using mPower.EventHandlers;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mongo;
using mPower.Framework.Mvc;
using mPower.Framework.Mvc.Validation;
using mPower.Framework.Registries;
using mPower.Framework.Utils;
using mPower.Framework.Utils.Security;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.Notifications;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Nuggets;
using mPower.WebApi.Factories.Command.Calendar;
using mPower.WebApi.Tenants.Services;
using Microsoft.Practices.ServiceLocation;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;
using BsonTypesMapper = Default.BsonTypesMapper;
using ObjectRepository = mPower.WebApi.Framework.ObjectRepository;
using DebtViewModelBuilder = mPower.WebApi.Tenants.Services.DebtViewModelBuilder;
using StructureMapServiceLocator = Paralect.ServiceLocator.StructureMap.StructureMapServiceLocator;
using TrendsViewModelFactory = mPower.WebApi.Factories.TrendsViewModelFactory;


namespace mPower.WebApi.Services
{
    public class MultiTenancyService
    {
        private readonly IContainer _container;

        public MultiTenancyService(IContainer container)
        {
            _container = container;
        }

        public void Configure()
        {
            BsonTypesMapper.RegisterBsonMaps();
            SetupMultiTenancy();
        }

        private void SetupMultiTenancy()
        {
            new SettingsRegistry(_container);
            new MongoRegistry(_container);

            var service = _container.GetInstance<AffiliateDocumentService>();

            var affiliates = service.GetAll();

            foreach (var tenant in affiliates.Select(MapApplicationTenant))
                ConfigureTenantContainer(tenant);

            var settings = _container.GetInstance<MPowerSettings>();
            var encrypt = new EncryptionService();
            var mongoUtil = new MongoUtil(settings.PathToBackuper, settings.PathToRestorer, new CommonUtil());

            _container.Configure(config =>
            {
                config.For<IEncryptionService>().Singleton().Use(encrypt);
                config.For<MongoUtil>().Singleton().Use(mongoUtil);
                config.For<AccountsService>();
                config.For<UploadUtil>();
                config.For<CommandService>();
                config.For<DebtViewModelBuilder>();
                config.For<DebtCalculator>();
                config.For<TrendsViewModelFactory>();
                config.For<SegmentViewHelper>();
                config.For<HtmlToPdfWriter>();
                config.For<SegmentEstimationHelper>();
                config.For<ICommandService>().Use<CommandService>();
                config.For<IObjectRepository>().Use<ObjectRepository>();
                config.For<ZillowHelpers>().Use<ZillowHelpers>();
                config.For<MembershipService>().Use<MembershipService>();
            });
        }

        private void ConfigureTenantContainer(ApplicationTenant tenant)
        {
            var settings = _container.GetInstance<MPowerSettings>();
            var encrypt = new EncryptionService();

            // Id generator & Membership API
            _container.Configure(config =>
            {
                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
                config.For<IEncryptionService>().Singleton().Use(encrypt);
                config.For<IEmailHtmlBuilder>().Use<NuggetHtmlBuilder>();
                config.For<LedgerDocumentService>().Use<LedgerDocumentService>();
                config.For<NuggetHtmlBuilder>();
            });


            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(_container));
            //
            // Service bus
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(_container))
                .MemorySynchronousTransport()
                .SetName("Main Service Bus")
                .SetInputQueue("mpower.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Event"), "mpower.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Command"), "mpower.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Message"), "mpower.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Signal"), "mpower.sync.server")
                .Dispatcher(d => d
                                    .AddHandlers(Assembly.GetExecutingAssembly())
                    .AddHandlers(typeof(LedgerAR).Assembly) //mPower.Domain command handler
                    .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly, new[] { "mPower.EventHandlers.Immediate" }) //sync event handlers
                 )
            );

            _container.Configure(config => config.For<IServiceBus>().Singleton().Use(bus));

            var asyncBus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(_container))
                .MsmqTransport()
                .SetName("Async Service Bus")
                .SetInputQueue($"{settings.InputQueueName}_{tenant.ApplicationName}")
                .SetErrorQueue($"{settings.ErrorQueueName}_{tenant.ApplicationName}")
                .AddEndpoint(type => type.FullName.EndsWith("Event"),
                    $"{settings.InputQueueName}_{tenant.ApplicationName}")
                .AddEndpoint(type => type.FullName.EndsWith("Command"),
                    $"{settings.InputQueueName}_{tenant.ApplicationName}")
                .AddEndpoint(type => type.FullName.EndsWith("Message"),
                    $"{settings.InputQueueName}_{tenant.ApplicationName}")
                .AddEndpoint(type => type.FullName.EndsWith("Signal"),
                    $"{settings.InputQueueName}_{tenant.ApplicationName}")
                .Dispatcher(d => d
                            .AddHandlers(typeof(LedgerAR).Assembly)
                            .AddHandlers(typeof(NotificationTempService).Assembly)
                            .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly, new[] { "mPower.EventHandlers.Eventual" }) //async event handlers
                            .AddHandlers(Assembly.GetExecutingAssembly())
                )
            );

            _container.Configure(config => config.For<AsyncServiceBus>().Singleton().Use(new AsyncServiceBus(asyncBus)));

            // 
            // Domain and Event store configuration
            //
            var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            var transitionsRepository = new MongoTransitionRepository(
                new AssemblyQualifiedDataTypeRegistry(),
                settings.MongoWriteDatabaseConnectionString);

            var transitionsStorage = new TransitionStorage(transitionsRepository);

            _container.Configure(config =>
            {
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<ISnapshotRepository>().Singleton().Use(new MongoSnapshotRepository(settings.MongoWriteDatabaseConnectionString));
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<IEventBus>().Use<DualEventBus>();
                config.For<IRepository>().Use<Repository>();
                config.For<IEventService>().Use<EventService>();
                config.For<IAggregationCallback>().Use<AggregationCallback>();
            });

            var callback = _container.GetInstance<IAggregationCallback>();
            _container.Configure(config => config.AddRegistry(new AggregationClientRegister(callback)));

            _container.Configure(config => config.For<IApplicationTenant>().Singleton().Use(tenant));

            //Scan for IObjectFactory<TInput, TOutput> and IValidator implementations
            _container.Configure(configure => configure.Scan(scanner =>
            {
                scanner.Assembly(typeof(GroupedSeletcListItemFactoryAlternate).Assembly);
                scanner.Assembly(typeof(BudgetViewModelFactory).Assembly);
                scanner.Assembly(typeof(Calendar_OnetimeCalendarEvent_CreateCommandFactory).Assembly);
                scanner.ConnectImplementationsToTypesClosing(typeof(IObjectFactory<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
            }));

            //Scan DocumentServices
            _container.Configure(config =>
            {
                foreach (var type in typeof(TransactionDocumentService).Assembly.GetTypes().Where(t => !t.IsAbstract))
                {
                    if (type.Name.EndsWith("Service"))
                        config.For(type);
                }

                foreach (var type in typeof(CommandLogDocumentService).Assembly.GetTypes().Where(t => !t.IsAbstract))
                {
                    if (type.Name.EndsWith("Service"))
                        config.For(type);
                }
            });

            _container.Configure(config => config.Scan(scanner =>
            {
                scanner.Assembly(typeof(INugget).Assembly);
                scanner.AddAllTypesOf(typeof(INugget));
            }));
        }

        private ApplicationTenant MapApplicationTenant(AffiliateDocument affiliate)
        {
            var tenant = new ApplicationTenant
            {
                ApplicationId = affiliate.ApplicationId,
                ApplicationName = affiliate.ApplicationName,
                UrlPaths = affiliate.UrlPaths,
                LegalName = affiliate.LegalName,
                ContactPhoneNumber = affiliate.ContactPhoneNumber,
                EmailSuffix = affiliate.EmailSuffix,
                DisplayName = affiliate.DisplayName,
                Smtp = affiliate.CreateSmptClient(),
                ChargifySharedKey = affiliate.ChargifySharedKey,
                ChargifyApiKey = affiliate.ChargifyApiKey,
                ChargifyUrl = affiliate.ChargifyUrl,
                MembershipApiKey = affiliate.MembershipApiKey,
                AssemblyName = affiliate.AssemblyName,
                JanrainAppApiKey = affiliate.JanrainAppApiKey,
                JanrainAppUrl = affiliate.JanrainAppUrl,
                PfmEnabled = affiliate.PfmEnabled,
                BfmEnabled = affiliate.BfmEnabled,
                CreditAppEnabled = affiliate.CreditAppEnabled,
                SavingsEnabled = affiliate.SavingsEnabled,
                MarketingEnabled = affiliate.MarketingEnabled,
                Address = affiliate.Address,
            };

            return tenant;
        }
    }
}