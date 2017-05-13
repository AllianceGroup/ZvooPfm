using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class AccountData
    {
        public string InstitutionName { get; set; }
        public AccountLabelEnum LabelEnum { get; set; }
        public AccountTypeEnum TypeEnum { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Number { get; set; }
        public String ParentAccountId { get; set; }
        public bool Imported { get; set; }
        public float InterestRatePerc { get; set; }
        public long MinMonthPaymentInCents { get; set; }
        public long CreditLimitInCents { get; set; }
        
        public bool Aggregated { get; set; }
        public long? IntuitInstitutionId { get; set; }
        public String IntuitAccountId { get; set; }
        public String IntuitAccountNumber { get; set; }
        public List<string> IntuitCategoriesNames { get; set; }

        public AccountData()
        {
            Imported = false;
            IntuitCategoriesNames = new List<string>();
        }
    }
}