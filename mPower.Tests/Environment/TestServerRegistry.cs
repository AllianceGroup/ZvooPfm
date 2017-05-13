using Microsoft.Practices.ServiceLocation;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Membership.User;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Environment;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using StructureMap;

namespace mPower.Tests.Environment
{
    public class TestServerRegistry
    {
        public TestServerRegistry(IContainer container)
        {
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));
            container.Configure(config =>
            {
                var bus = ServiceBus.Run(c => c
                    .SetServiceLocator(new StructureMapServiceLocator(container))
                    .MemorySynchronousTransport()
                    .SetName("Test Service Bus")
                    .SetInputQueue("MPower.TestServer")
                    .AddEndpoint(type => type.FullName.EndsWith("Command"), "MPower.TestServer")
                    .AddEndpoint(type => type.FullName.EndsWith("Event"), "MPower.TestServer")
                    .AddEndpoint(type => type.FullName.EndsWith("Message"), "MPower.TestServer")
                    .Dispatcher(d => d
                        .AddHandlers(typeof(UserAR).Assembly) //mpower.Domain assembly
                        .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly) //mPower.EventHandlers assembly
                    )
                );

                config.For<IServiceBus>().Singleton().Use(bus);
                // DataTypeRegistry (Used to resolve CLR type from string)
                config.For<IDataTypeRegistry>().Singleton().Use<AssemblyQualifiedDataTypeRegistry>();
                // All transitions stored in InMemory repository
                config.For<ITransitionStorage>().Singleton().Use(new TransitionStorage(new InMemoryTransitionRepository()));
                config.For<ISnapshotRepository>().Singleton().Use(new FakeSnapshotsRepository());

                config.For<IEventBus>().Use<ParalectServiceBusEventBus>();
                config.For<IEventService>().Use<EventService>();
                // We are using default implementation of repository
                
                config.For<IRepository>().Use<Repository>();
                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
            });

        }
    }

    public class FakeSnapshotsRepository : ISnapshotRepository
    {
        public Snapshot Load<T>(string id)
        {
            return null;
        }

        public void Save(Snapshot ar, int minTransitionsForSnapshot = 30)
        {
        }
    }
}