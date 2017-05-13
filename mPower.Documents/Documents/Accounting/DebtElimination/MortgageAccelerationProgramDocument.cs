using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils.Extensions;

namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class MortgageAccelerationProgramDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }

        public long LoanAmountInCents { get; set; }

        /// <summary>
        /// Interest Rate Per Year in percents
        /// </summary>
        public float InterestRatePerYear { get; set; }

        public float LoanTermInYears { get; set; }

        public PaymentPeriodEnum PaymentPeriod { get; set; }

        public string PaymentPeriodString => PaymentPeriod.GetDescription();

        public long ExtraPaymentInCentsPerPeriod { get; set; }

        public DisplayResolutionEnum DisplayResolution { get; set; }

        public CalcDataForComparison OriginalParams { get; set; }

        public CalcDataForComparison AcceleratedParams { get; set; }

        public List<ProgramDetailsItem> Details { get; set; }

        public bool AddedToCalendar { get; set; }

        public MortgageAccelerationProgramDocument()
        {
            PaymentPeriod = PaymentPeriodEnum.Monthly;
            DisplayResolution = DisplayResolutionEnum.Medium;
            OriginalParams = new CalcDataForComparison();
            AcceleratedParams = new CalcDataForComparison();
            Details = new List<ProgramDetailsItem>();
        }


        public class CalcDataForComparison
        {
            public long PaymentInCents { get; set; }

            public long MonthlyPaymentEquivalentInCents { get; set; }

            public long TotalPaymentAmountInCents { get; set; }

            public long TotalInterestPaidInCents { get; set; }

            public long TotalSavingsInCents { get; set; }

            public double PayoffTime { get; set; }
        }
    }
}