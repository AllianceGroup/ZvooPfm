using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_ArchiveCommandHandler : IMessageHandler<Goal_ArchiveCommand>
    {
        private readonly IRepository _repository;

        public Goal_ArchiveCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Goal_ArchiveCommand message)
        {
            var goal = _repository.GetById<GoalAR>(message.GoalId);
            goal.SetCommandMetadata(message.Metadata);
            goal.Archive();
            _repository.Save(goal);
        }
    }
}