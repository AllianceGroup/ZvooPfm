using System.Collections.Generic;
using System.ComponentModel;

namespace mPower.Domain.Accounting.Enums
{
    public static class BaseAccounts
    {
        [Description("Opening Balance Equity")]
        public const string OpeningBalanceEquity = "OBE";
        [Description("Interest")]
        public const string Interest = "I";
        [Description("Accounts Receivable")]
        public const string AccountsReceivable = "AR";
        [Description("Accounts Payable")]
        public const string AccountsPayable = "AP";
        [Description("Owner Contribution")]
        public const string OwnerContribution = "OC";
        [Description("Owner Distribution")]
        public const string OwnerDistribution = "OD";
        [Description("UnCategorized Income")]
        public const string UnCategorizedIncome = "UCI";
        [Description("UnCategorized Expense")]
        public const string UnCategorizedExpense = "UCE";
        [Description("Retained Earnings")]
        public const string RetainedEarnings = "RE";
        [Description("Unknown Cash")]
        public const string UnknownCash = "UC";
        [Description("Payments")] 
        public const string Payments = "P";
        [Description("Transfers")] 
        public const string Transfers = "T";
        [Description("Paycheck/Salary")]
        public const string Salary = "S";

        public static IEnumerable<string> All()
        {
            yield return AccountsReceivable;
            yield return AccountsPayable;
            yield return OwnerContribution;
            yield return OwnerDistribution;
            yield return UnCategorizedExpense; 
            yield return UnCategorizedIncome;
            yield return RetainedEarnings;
            yield return Interest;
            yield return UnknownCash;
            yield return OpeningBalanceEquity;
            yield return Payments;
            yield return Transfers;
            yield return Salary;
        } 

    }
}
