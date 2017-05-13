using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_AddedEvent : Event
    {
        public string InstitutionName { get; set; }
        [Obsolete]
        public long AggregatedBalanceInCents { get; set; }
        [Obsolete]
        public bool IsUpdating { get; set; }
        public string ParentAccountId { get; set; }
        public AccountLabelEnum AccountLabelEnum { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public AccountTypeEnum AccountTypeEnum { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Number { get; set; }
        public bool Imported { get; set; }
        public float InterestRatePerc { get; set; }
        public long MinMonthPaymentInCents { get; set; }
        public long CreditLimitInCents { get; set; }

        public bool Aggregated { get; set; }
        /// <summary>
        /// IntuitInstitutionId
        /// </summary>
        [Obsolete]
        public String YodleeContentServiceId { get; set; }
        public long? ContentServiceId
        {
            get
            {
                long id;
                return long.TryParse(YodleeContentServiceId, out id) ? id : (long?)null;
            }
            set { YodleeContentServiceId = value.ToString(); }
        }

        /// <summary>
        /// IntuitAccountId
        /// </summary>
        public String YodleeItemAccountId { get; set; }
        public string IntuitAccountNumber { get; set; }
        public List<string> IntuitCategoriesNames { get; set; }

        public Ledger_Account_AddedEvent()
        {
            IntuitCategoriesNames = new List<string>();
        }
    }
}