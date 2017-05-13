using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_CreateCommand : Command
    {
        public string InstitutionName { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String Name { get; set; }
        public AccountTypeEnum AccountTypeEnum { get; set; }
        public AccountLabelEnum AccountLabelEnum { get; set; }
        public String ParentAccountId { get; set; }
        public bool Imported { get; set; }
        public float InterestRatePerc { get; set; }
        public long MinMonthPaymentInCents { get; set; }
        public long CreditLimitInCents { get; set; }
        // Aggregation
        public bool Aggregated { get; set; }
        public long? IntuitInstitutionId { get; set; }
        public string IntuitAccountId { get; set; }
        public string IntuitAccountNumber { get; set; }
        public List<string> IntuitCategoriesNames { get; set; }

        public Ledger_Account_CreateCommand()
        {
            IntuitCategoriesNames = new List<string>();
        }
    }
}