using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_DisplayMode_UpdateCommandHandler : IMessageHandler<DebtElimination_DisplayMode_UpdateCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_DisplayMode_UpdateCommandHandler(IRepository repository)
         {
             _repository = repository;
         }

        public void Handle(DebtElimination_DisplayMode_UpdateCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.Id);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.UpdateDisplayMode(message.DisplayMode);

            _repository.Save(debtEliminationAr);
        }
    }
}