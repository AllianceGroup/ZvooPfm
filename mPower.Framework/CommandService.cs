using System;
using mPower.Framework.Environment;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Framework
{
    public interface ICommandService
    {
        void Send(params ICommand[] commands);
        void SendAsync(params ICommand[] commands);
        void PrepareCommands(params ICommand[] commands);
    }

    public class CommandService : ICommandService
    {
        private readonly IIdGenerator _idGenerator;
        private readonly IServiceBus _bus;
        private readonly AsyncServiceBus _asyncServiceBus;

        public CommandService(IServiceBus bus, AsyncServiceBus asyncServiceBus, IIdGenerator idGenerator)
        {
            _bus = bus;
            _asyncServiceBus = asyncServiceBus;
            _idGenerator = idGenerator;
        }

        public virtual void Send(params ICommand[] commands)
        {
            PrepareCommands(commands);
            _bus.Send(commands);
        }

        public virtual void SendAsync(params ICommand[] commands)
        {
            PrepareCommands(commands);
            _asyncServiceBus.Bus.Send(commands);
        }

        public void PrepareCommands(params ICommand[] commands)
        {
            foreach (ICommand t in commands)
            {
                t.Metadata.CommandId = _idGenerator.Generate();
                t.Metadata.CreatedDate = DateTime.UtcNow;
                t.Metadata.TypeName = t.GetType().FullName;
            }
        }
    }
}