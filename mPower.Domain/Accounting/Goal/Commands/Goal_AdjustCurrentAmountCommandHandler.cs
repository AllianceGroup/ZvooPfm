using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_AdjustCurrentAmountCommandHandler : IMessageHandler<Goal_AdjustCurrentAmountCommand>
    {
        private readonly IRepository _repository;

        public Goal_AdjustCurrentAmountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Goal_AdjustCurrentAmountCommand message)
        {
            var goal = _repository.GetById<GoalAR>(message.GoalId);
            goal.SetCommandMetadata(message.Metadata);
            goal.AdjustCurrentAmount(message.ValueInCents, message.Date);
            _repository.Save(goal);
        }
    }
}