using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_CreateCommandHandler : IMessageHandler<DebtElimination_CreateCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_CreateCommand message)
        {
            var debtEliminationAr = new DebtEliminationAR(message.Id, message.UserId, message.LedgerId , message.Metadata);

            _repository.Save(debtEliminationAr);

        }
    }
}