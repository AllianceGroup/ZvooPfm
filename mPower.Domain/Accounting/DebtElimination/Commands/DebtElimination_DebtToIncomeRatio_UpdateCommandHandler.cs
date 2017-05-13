using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_DebtToIncomeRatio_UpdateCommandHandler : IMessageHandler<DebtElimination_DebtToIncomeRatio_UpdateCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_DebtToIncomeRatio_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_DebtToIncomeRatio_UpdateCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.Id);
            debtEliminationAr.SetCommandMetadata(message.Metadata);

            debtEliminationAr.UpdateDebtToIncomeRatio(message.MonthlyGrossIncomeInCents, message.TotalMonthlyRentInCents, message.TotalMonthlyPitiaInCents, message.TotalMonthlyDebtInCents, message.DebtToIncomeRatio, message.DebtToIncomeRatioString);

            _repository.Save(debtEliminationAr);

        }
    }
}