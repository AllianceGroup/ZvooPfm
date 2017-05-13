using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;

namespace mPower.Framework.Registries
{
    public class CommandServerRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CommandServerRegistry(IContainer container)
        {
            var settings = container.GetInstance<MPowerSettings>();

            //
            // Service Bus configuration and start
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MemorySynchronousTransport()
                .SetName("Command Service Bus")
                .SetInputQueue("MPower.CommandServer")
                .AddEndpoint(type => type.FullName.EndsWith("Event"), "MPower.CommandServer")
                .AddEndpoint(type => type.FullName.EndsWith("Command"), "MPower.CommandServer")
                .Dispatcher(d => d
                    .AddHandlers(typeof(CommandServerRegistry).Assembly)
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
                config.For<IEventBus>().Use<IEventBus>();
            });
        }
    }
}