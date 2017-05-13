using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Goal.Data;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_CreateCommandHandler : IMessageHandler<Goal_CreateCommand>
    {
        private readonly IRepository _repository;

        public Goal_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Goal_CreateCommand message)
        {
            var data = new GoalData
            {
                Type = message.Type,
                Title = message.Title,
                TotalAmountInCents = message.TotalAmountInCents,
                MonthlyPlanAmountInCents = message.MonthlyPlanAmountInCents,
                StartDate = message.StartDate,
                PlannedDate = message.PlannedDate,
                ProjectedDate = message.ProjectedDate,
                UserId = message.UserId,
                Image = message.Image,
                StartingBalanceInCents = message.StartingBalanceInCents
            };

            var goal = new GoalAR(message.GoalId, data, message.Metadata);
            _repository.Save(goal);
        }
    }
}