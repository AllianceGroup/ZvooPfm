using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_AddToCalendarCommandHandler : IMessageHandler<DebtElimination_AddToCalendarCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_AddToCalendarCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_AddToCalendarCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.DebtEliminationId);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.AddEliminationToCalendar(message.CalendarId);

            _repository.Save(debtEliminationAr);
        }
    }
}