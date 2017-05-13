using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_MortgageProgram_DeleteCommandHandler : IMessageHandler<DebtElimination_MortgageProgram_DeleteCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_MortgageProgram_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_MortgageProgram_DeleteCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.DebtEliminationId);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.DeleteMortgageProgram(message.Id, message.CalendarId);

            _repository.Save(debtEliminationAr);
        }
    }
}