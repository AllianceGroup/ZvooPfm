using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_DeleteCommandHandler : IMessageHandler<Goal_DeleteCommand>
    {
        private readonly IRepository _repository;

        public Goal_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        public void Handle(Goal_DeleteCommand message)
        {
            var goal = _repository.GetById<GoalAR>(message.GoalId);
            goal.SetCommandMetadata(message.Metadata);
            goal.Delete();
            _repository.Save(goal);
        }
    }
}