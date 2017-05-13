using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.ExternalServices;
using mPower.Domain.Accounting.Ledger;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Registries;
using mPower.MembershipApi;
using mPower.Tests.Environment;
using NUnit.Framework;
using Paralect.Config.Settings;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;

namespace mPower.Tests.UnitTests.Web.Controllers
{
    public abstract class ControllerBaseTest
    {
        protected IContainer _container;
        

        [TestFixtureSetUp]
        public void Setup()
        {
            _container = Bootstrapper.Bootstrap();
            _container.Configure(config =>
            {
                var settings = SettingsMapper.Map<MPowerSettings>();
                config.For<MPowerSettings>().Singleton().Use(settings);

                //Membership API
                config.For<MembershipApiService>().Add(
                    new MembershipApiService(settings.MembershipApiKey,
                                            settings.MembershipBaseUrl));

                config.For<ChargifyService>().Add(new ChargifyService());
                                         
            });

            

            var service = _container.GetInstance<AffiliateDocumentService>();

            var affiliates = service.GetAll();

            var tenants = new List<IApplicationTenant>();

            foreach (var tenant in affiliates.Select(MapApplicationTenant))
            {
                ConfigureTenantContainer(tenant);
                tenants.Add(tenant);
            }

            //// resolves the tenant based on the url from the request
            var tenantSelector = new TenantSelector(tenants);

            //Set Tenant Selector to a Static Class
            TenantTools.Selector = tenantSelector;

            //// resolves the IoC container from the request
            var containerSelector = new ContainerResolver(tenantSelector);


            CreateTestUser();

        }

        private void CreateTestUser()
        {
            var membership = _container.GetInstance<MembershipApiService>();
            try
            {
                membership.LogIn("controller@demo.com", "demo2011");
            }
            catch
            {
                membership.CreateUser("Demo", "Demo", "controller@demo.com", "controller@demo.com", "demo2011");
            }
        }

        private void ConfigureTenantContainer(ApplicationTenant tenant)
        {
            var container = new Container();

            #region Common Configuration

            new SettingsRegistry(container);
            new MongoRegistry(container);

            var settings = container.GetInstance<MPowerSettings>();

            // Id generator
            container.Configure(config =>
            {
                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
            });

            //
            // Service bus
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MemorySynchronousTransport()
                .SetName("Main Service Bus")
                .SetInputQueue(settings.InputQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Event"), settings.InputQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Command"), settings.InputQueueName)
                .AddEndpoint(type => type.FullName.EndsWith("Message"), settings.InputQueueName)
                .Dispatcher(d => d
                    .AddHandlers(typeof(LedgerAR).Assembly)
                    .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly)
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
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<IEventBus>().Use<ParalectServiceBusEventBus>();
                config.For<IEventService>().Use<EventService>();

                // We are using default implementation of repository
                config.For<IRepository>().Use<Repository>();
            });

            #endregion

            // Scan Assemblies based on the Controller Convention
            container.Configure(config =>
            {
                config.Scan(scanner =>
                {
                    //Scan Assembly of Framework Project
                    scanner.Assembly(typeof(IApplicationTenant).Assembly);

                    //Scan Tenant Assembly first if it declared
                    if (!string.IsNullOrEmpty(tenant.AssemblyName))
                        scanner.Assembly(tenant.AssemblyName);

                    //Scan Default Tenant
                    scanner.Assembly(settings.DefaultTenantAssembly);

                    scanner.Convention<ControllerConvention>();
                    scanner.AddAllTypesOf<ITenantConfigurer>();
                });
            });

            //Configure current container if ITenantConfigurer exists in it
            var customConfigurer = container.TryGetInstance<ITenantConfigurer>();
            if (customConfigurer != null)
                customConfigurer.Configure(container);


            tenant.DependencyContainer = container;
            container.Configure(config => config.For<IApplicationTenant>().Singleton().Use(tenant));
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
                ChargifyUrl = affiliate.ChargifyUrl,
                MembershipApiKey = affiliate.MembershipApiKey,
                AssemblyName = affiliate.AssemblyName,
                JanrainAppApiKey = affiliate.JanrainAppApiKey,
                JanrainAppUrl = affiliate.JanrainAppUrl,
            };

            return tenant;
        }

    }
}
