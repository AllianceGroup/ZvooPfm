using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class LedgerUserDocument
    {
        [BsonId]
        public string Id { get; set; }
    }

    public class LedgerDocument
    {
        public LedgerDocument()
        {
            Accounts = new List<AccountDocument>();
            Users = new List<LedgerUserDocument>();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public LedgerTypeEnum TypeEnum { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string TaxId { get; set; }

        public List<AccountDocument> Accounts { get; set; }

        private DateTime _fiscalYearStart;

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime FiscalYearStart
        {
            get { return _fiscalYearStart.Date; }
            set { _fiscalYearStart = value.Date; }
        }

       
        public DateTime FiscalYearEnd
        {
            get { return FiscalYearStart.AddYears(1).AddMilliseconds(-1); }
        }

        public DateTime CreatedDate { get; set; }

        public List<LedgerUserDocument> Users { get; set; }

        public List<KeywordMapDocument> KeywordMap { get; set; }


        
        #region Budgets Info

        public bool IsBudgetSet { get; set; }

        public int MinBudgetMonth { get; set; }

        public int MinBudgetYear { get; set; }

        public int MaxBudgetMonth { get; set; }

        public int MaxBudgetYear { get; set; }

        #endregion
    }
}
