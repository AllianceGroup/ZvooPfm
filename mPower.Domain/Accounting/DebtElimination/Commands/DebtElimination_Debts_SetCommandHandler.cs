using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_Debts_SetCommandHandler : IMessageHandler<DebtElimination_Debts_SetCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_Debts_SetCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_Debts_SetCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.DebtEliminationId);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.SetDebts(message.Debts);

            _repository.Save(debtEliminationAr);
        }
    }
}