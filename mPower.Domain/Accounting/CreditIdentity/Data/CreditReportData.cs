using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class CreditReportData
    {
        public List<BorrowerData> Borrowers { get; set; }
        public List<InquiryData> Inquiries { get; set; }
        public List<MessageData> Messages { get; set; }
        public List<PublicRecordData> PublicRecords { get; set; }
        public List<CreditorData> Creditors { get; set; }
        public List<AccountsGroupData> AccountsGroups { get; set; }

        public string CurrentVersion { get; set; }

        public int Score { get; set; }
        public DateTime ScoreDate { get; set; }
        public int QualitativeRank { get; set; }
        public int PopulationRank { get; set; }
        public string BureauSource { get; set; }

        public List<string> NegativeFactors { get; set; }


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