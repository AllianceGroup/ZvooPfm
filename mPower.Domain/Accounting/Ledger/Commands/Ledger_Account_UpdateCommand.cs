using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_UpdateCommand : Command
    {
        public String LedgerId { get; set; }

        public String AccountId { get; set; }

        public String Name { get; set; }

        public string ParentAccountId { get; set; }

        public string Description { get; set; }

        public string Number { get; set; }

        public float InterestRatePerc { get; set; }

        public long MinMonthPaymentInCents { get; set; }

        public long CreditLimitInCents { get; set; }

        public string InstitutionName { get; set; }
    }
}
