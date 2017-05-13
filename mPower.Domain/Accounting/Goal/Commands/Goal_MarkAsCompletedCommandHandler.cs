using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_MarkAsCompletedCommandHandler : IMessageHandler<Goal_MarkAsCompletedCommand>
    {
        private readonly IRepository _repository;

        public Goal_MarkAsCompletedCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Goal_MarkAsCompletedCommand message)
        {
            var goal = _repository.GetById<GoalAR>(message.GoalId);
            goal.SetCommandMetadata(message.Metadata);
            goal.MarkAsCompleted();
            _repository.Save(goal);
        }
    }
}