using System.Linq;
using System.Reflection;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Framework.Domain
{
    public class AggregateCommandHandler<TAggregate> where TAggregate : MpowerAggregateRoot
    {
        public TAggregate Aggregate { get; set; }
    }

    public class MpowerCommand: Command
    {
        public string AggregateId { get; set; }
    }

    public class DynamicHandler
    {
        private readonly IRepository _repository;

        public DynamicHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void HandleFor<TAggregate>(AggregateCommandHandler<TAggregate> handler, MpowerCommand command) where TAggregate : MpowerAggregateRoot
        {
            var ar = _repository.GetById<TAggregate>(command.AggregateId);
            handler.Aggregate = ar;
            ar.SetCommandMetadata(command.Metadata);
            InvokeHandler(handler,command);       
            _repository.Save(ar);
        }

        private void InvokeHandler<TAggregate>(AggregateCommandHandler<TAggregate> handler, MpowerCommand command) where TAggregate : MpowerAggregateRoot
        {
            var commandType = command.GetType();
            var handlerType = handler.GetType();
            foreach (var method in handlerType.GetMethods())
            {
                var parameters = method.GetParameters();
                if (parameters.Count() == 1 && parameters[0].ParameterType == commandType)
                {
                    method.Invoke(handler, parameters: new[] { command });
                    return;
                }
            }
        }
    }
}