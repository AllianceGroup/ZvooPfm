using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_MortgageProgram_AddToCalendarCommandHandler : IMessageHandler<DebtElimination_MortgageProgram_AddToCalendarCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_MortgageProgram_AddToCalendarCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_MortgageProgram_AddToCalendarCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.DebtEliminationId);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.AddMortgageProgramToCalendar(message.MortgageProgramId, message.CalendarId);

            _repository.Save(debtEliminationAr);
        }
    }
}