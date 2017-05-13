using System.ComponentModel.DataAnnotations;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.MortgageAcceleration
{
    public class MortgageProgramModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public decimal LoanAmountInDollars { get; set; }

        public float InterestRatePerYear { get; set; }

        public float LoanTermInYears { get; set; }

        public PaymentPeriodEnum PaymentPeriod { get; set; }

        public decimal ExtraPaymentInDollarsPerPeriod { get; set; }

        public DisplayResolutionEnum DisplayResolution { get; set; }

        public MortgageAccelerationProgramDocument.CalcDataForComparison OriginalParams { get; set; }

        public MortgageAccelerationProgramDocument.CalcDataForComparison AcceleratedParams { get; set; }

        public ProgramDetailsModel Details { get; set; }

        public bool AddedToCalendar { get; set; }

        public MortgageProgramModel()
        {
            PaymentPeriod = PaymentPeriodEnum.Monthly;
            DisplayResolution = DisplayResolutionEnum.Medium;
            OriginalParams = new MortgageAccelerationProgramDocument.CalcDataForComparison();
            AcceleratedParams = new MortgageAccelerationProgramDocument.CalcDataForComparison();
            Details = new ProgramDetailsModel();
        }
    }
}