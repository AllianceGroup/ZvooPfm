using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.DebtElimninationProgramController
{
    public class DebtModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public AccountLabelEnum LabelEnum { get; set; }

        public long Balance { get; set; }

        public float InterestRatePerc { get; set; }

        public long MinMonthPaymentInCents { get; set; }

        public bool UseInProgram { get; set; }
    }
}