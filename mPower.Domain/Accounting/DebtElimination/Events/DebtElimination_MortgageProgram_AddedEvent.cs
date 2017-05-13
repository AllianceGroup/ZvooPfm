using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_MortgageProgram_AddedEvent : Event
    {
        public string Id { get; set; }

        public string DebtEliminationId { get; set; }

        public string Title { get; set; }

        public long LoanAmountInCents { get; set; }

        public float InterestRatePerYear { get; set; }

        public float LoanTermInYears { get; set; }

        public PaymentPeriodEnum PaymentPeriod { get; set; }

        public long ExtraPaymentInCentsPerPeriod { get; set; }

        public DisplayResolutionEnum DisplayResolution { get; set; }
    }
}