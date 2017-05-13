using System;

namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class ProgramDetailsItem
    {
        public int Step { get; set; }

        public DateTime Date { get; set; }

        public bool ShowInGrid { get; set; }

        public bool ShowInGraph { get; set; }


        public long OrigBalanceInCents { get; set; }

        public long OrigTotalInterestInCents { get; set; }

        public long OrigTotalPaymentsInCents { get; set; }


        public long AccBalanceInCents { get; set; }

        public long AccPaymentInCents { get; set; }

        public long AccPrincipalInCents { get; set; }

        public long AccInterestInCents { get; set; }

        public long AccEquityInCents { get; set; }

        public long AccTotalInterestInCents { get; set; }

        public long AccTotalPaymentsInCents { get; set; }
    }
}