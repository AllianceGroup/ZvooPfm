using System;
using System.ComponentModel;
using mPower.Framework.Utils;

namespace mPower.Domain.Accounting.Enums
{
    [Serializable]
    public enum AccountLabelEnum
    {   
        //**  ASSETS

        [IifName("BANK"), Description("Bank")]
        Bank = 1,

        [IifName("AR"), Description("Accounts Receivable")]
        AccountsReceivable = 2,

        [IifName("OCASSET"), Description("Other Current Asset")]
        OtherCurrentAsset = 3,

        [IifName("FIXASSET"), Description("Fixed Asset")]
        FixedAsset = 4,

        [IifName("OASSET"), Description("Other Asset")]
        OtherAsset = 5,


        //**  LIABILITIES

        [IifName("AP"), Description("Accounts Payable")]
        AccountsPayable = 6,

        [IifName("CCARD"), Description("Credit Card")]
        CreditCard = 7,

        [IifName("OCLIAB"), Description("Other Current Liability")]
        OtherCurrentLiability = 8,

        [IifName("LTLIAB"), Description("Long Term Liability")]
        LongTermLiability = 9,

        //** EQUITY

        [IifName("EQUITY"), Description("Equity")]
        Equity = 10,

        //** INCOME

        [IifName("INC"), Description("Income")]
        Income = 11,

        //** EXPENSE

        [IifName("COGS"), Description("Cost Of Goods Sold")]
        CostOfGoodsSold = 12,

        [IifName("EXP")]
        Expense = 13,

        //** Income

        [IifName("EXINC"), Description("Other Income")]
        OtherIncome = 14,

        //** Expense

        [IifName("EXEXP"), Description("Other Expense")]
        OtherExpense = 15,


        //** Not associated with debit credit account

        [IifName("NONPOSTING"), Description("Non Posting Account")]
        NonPostingAccount = 16,

        [IifName("LTLIAB"), Description("Mortgage/Loan")]
        Loan = 17,

        [IifName("OASSET"), Description("Investment")]
        Investment = 18,
    }
}