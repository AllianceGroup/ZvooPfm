using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class AccountData
    {
        public string AccountCondition { get; set; }
        public string AccountConditionSymbol { get; set; }
        public int AccountConditionRank { get; set; }
        public string AccountDesignator { get; set; }
        public string AccountNumber { get; set; }
        public string Bureau { get; set; }
        public string CreditorName { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime AccountStatusDate { get; set; }
        public DateTime DateClosed { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime DateReported { get; set; }
        public DateTime DateVerified { get; set; }
        public string DisputeDescription { get; set; }
        public decimal HighBalance { get; set; }
        public string IndustryCodeDescription { get; set; }
        public string IndustryCodeSymbol { get; set; }
        public string OpenClosedDescription { get; set; }
        public string OpenClosedSymbol { get; set; }
        public string PayStatusDescription { get; set; }
        public string PayStatusSymbol { get; set; }
        public List<string> Remarks { get; set; }
        public string SubscriberCode { get; set; }
        public string VerificationIndicator { get; set; }

        public string AccountTypeAbbreviation { get; set; }
        public string AccountTypeDescription { get; set; }
        public string AccountTypeSymbol { get; set; }
        public string CreditTypeAbbreviation { get; set; }
        public string CreditTypeDescription { get; set; }
        public string CreditTypeSymbol { get; set; }


        public decimal CreditLimit;
        public decimal AmountPastDue;
        public string Collateral;
        public DateTime DateLastPayment;
        public DateTime DatePastDue;
        public DateTime DateWorstPayStatus;
        public decimal Late30Count;
        public decimal Late60Count;
        public decimal Late90Count;
        public decimal MonthlyPayment;
        public decimal MonthsReviewed;
        public decimal TermMonths;
        public decimal WorstPayStatusCount;

        public List<MonthlyPayStatusData> MonthlyPayStatus;
        public string PayStatus;
        public DateTime PayStartDate;
    }
}