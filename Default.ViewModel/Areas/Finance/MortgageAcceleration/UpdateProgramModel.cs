using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.MortgageAcceleration
{
    public class UpdateProgramModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The 'Program Name' field is required.")]
        public string Title { get; set; }

        [Range(0.001, double.MaxValue, ErrorMessage = "The field 'Loan Amount' must be between 0.001 and 1.79E+308.")]
        public decimal LoanAmountInDollars { get; set; }

        [Range(0.001, 1000000, ErrorMessage = "The field 'Interest Rate (% per Year)' must be between 0.001 and 1000000.")]
        public float InterestRatePerYear { get; set; }

        [Range(0.001, 200, ErrorMessage = "The field 'Loan Term (Years)' must be between 0.001 and 200.")]
        public float LoanTermInYears { get; set; }

        public PaymentPeriodEnum PaymentPeriod { get; set; }

        [Numeric]
        public decimal ExtraPaymentInDollarsPerPeriod { get; set; }

        public DisplayResolutionEnum DisplayResolution { get; set; }
    }
}