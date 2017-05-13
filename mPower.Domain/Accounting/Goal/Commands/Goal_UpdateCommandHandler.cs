using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Goal.Data;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_UpdateCommandHandler : IMessageHandler<Goal_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Goal_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Goal_UpdateCommand message)
        {
            var goal = _repository.GetById<GoalAR>(message.GoalId);
            goal.SetCommandMetadata(message.Metadata);

            var data = new GoalData
            {
                Title = message.Title,
                TotalAmountInCents = message.TotalAmountInCents,
                MonthlyPlanAmountInCents = message.MonthlyPlanAmountInCents,
                PlannedDate = message.PlannedDate,
                ProjectedDate = message.ProjectedDate,
                Image = message.Image,
                StartingBalanceInCents = message.StartingBalanceInCents
            };

            goal.Update(data);
            _repository.Save(goal);
        }
    }
}