using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class CreditReportDocument
    {
        [BsonId]
        public string Id { get; set; }

        public byte[] CreditReportData { get; set; }
        public List<BorrowerDocument> Borrowers { get; set; }
        public List<InquiryDocument> Inquiries { get; set; }
        public List<MessageDocument> Messages { get; set; }
        public List<PublicRecordDocument> PublicRecords { get; set; }
        public List<CreditorDocument> Creditors { get; set; }
        public List<AccountsGroupDocument> AccountsGroups { get; set; }
        
        public string CurrentVersion { get; set; }

        public int Score { get; set; }
        public DateTime ScoreDate { get; set; }
        public string Grade { get; set; }
        public int QualitativeRank { get; set; }
        public int PopulationRank { get; set; }
        public string BureauSource { get; set; }

        public List<string> NegativeFators { get; set; }


        public bool IsDeceased { get; set; }
        public bool IsFraud { get; set; }
        public bool SafetyCheckPassed { get; set; }
        public bool IsFrozen { get; set; }
        public int InquiriesInLastTwoYears { get; set; }
        public int PublicRecordCount { get; set; }
        public int ClosedAccountsCount { get; set; }
        public int DeliquentAccountsCount { get; set; }
        public int DerogatoryAccountsCount { get; set; }
        public int OpenAccountsCount { get; set; }
        public int TotalAccountsCount { get; set; }
        public double TotalBalances { get; set; }
        public double TotalMonthlyPayments { get; set; }
        public string Status { get; set; }

        public byte[] ReportData { get; set; }
    }
}
