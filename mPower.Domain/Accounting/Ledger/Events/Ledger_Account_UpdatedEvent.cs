using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_UpdatedEvent : Event
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Number { get; set; }

        public String ParentAccountId { get; set; }

        public float InterestRatePerc { get; set; }
        public long MinMonthPaymentInCents { get; set; }
        public long CreditLimitInCents { get; set; }

        public string InstitutionName { get; set; }

        #region Obsolete Properties

        [Obsolete]
        public AccountTypeEnum AccountTypeEnum { get; set; }
        [Obsolete]
        public AccountLabelEnum AccountLabelEnum { get; set; }
        [Obsolete]
        public bool Aggregated { get; set; }
        [Obsolete]
        public String YodleeContentServiceId { get; set; }
        [Obsolete]
        public String YodleeItemAccountId { get; set; }
        [Obsolete]
        public string IntuitAccountNumber { get; set; }

        #endregion
    }
}