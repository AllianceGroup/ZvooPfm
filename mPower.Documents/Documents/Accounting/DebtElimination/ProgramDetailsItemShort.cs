using System;

namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class ProgramDetailsItemShort
    {
        public string Id { get; set; }

        public string Debt { get; set; }

        public DateTime Date { get; set; }

        public long BalanceInCents { get; set; }

        public long MinPaymentInCents { get; set; }

        public long ActualPaymentInCents { get; set; }

        public long PrincipalPaymentInCents { get; set; }

        public long InterestPaymentInCents { get; set; } 
    }
}