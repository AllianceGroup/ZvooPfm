using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class AccountDocument
    {
        public string InstitutionName { get; set; }

        public AccountDocument()
        {
            Denormalized = new DenormalizedData();
            DateLastAggregated = DateTime.Now;
            AggregatedAccountStatus = AggregatedAccountStatusEnum.Normal;
            IntuitCategoriesNames = new List<string>();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AccountTypeEnum TypeEnum { get; set; }

        public AccountLabelEnum LabelEnum { get; set; }

        public string Number { get; set; }

        public DenormalizedData Denormalized { get; set; }

        public class DenormalizedData
        {
            public Int64 Balance { get; set; }
           
        }

        public string ParentAccountId { get; set; }

        public bool Archived { get; set; }

        public string ReasonToArchive { get; set; }

        public bool Imported { get; set; }

        public float InterestRatePerc { get; set; }

        public long MinMonthPaymentInCents { get; set; }

        public long CreditLimitInCents { get; set; }

        public bool IsBudgetSet { get; set; }

        public bool Aggregated { get; set; }

        public bool IsAggregated
        {
            get { return Aggregated || IntuitInstitutionId.HasValue; }
        }

        public bool IsUnknownCash
        {
            get { return Name == "UC" || Name == "Unknown Cash"; }
        }

        public Int64 ActualBalance
        {
            get { return IsAggregated ? AggregatedBalance : Denormalized.Balance; }
        }

        public Int64 AggregatedBalance { get; set; }
        public AggregatedAccountStatusEnum AggregatedAccountStatus { get; set; }
        public DateTime DateLastAggregated { get; set; }
        public long? IntuitInstitutionId { get; set; }
        public long? IntuitAccountId { get; set; }
        public string IntuitAccountNumber { get; set; }
        public List<string> IntuitCategoriesNames { get; set; }
        public string AggregationExceptionId { get; set; }

        public int Order { get; set; }

        public DateTime Created { get; set; }

        public DateTime AggregationStartedDate { get; set; }
    }
}
