using System.ComponentModel;

namespace mPower.Domain.Accounting.Enums
{
    public enum DebtEliminationPlanEnum
    {
        NotInitialized = 0,

        [Description("Quick Wins: Pay off smallest debts first")]
        QuickWins = 1,

        [Description("Highest Interest: Pay off loans with highest rates first")]
        HighestInterest = 2,

        [Description("Balanced: Pay off everything a little at a time")]
        Balanced = 3,

        [Description("Highest payment, smallest debt first")]
        SmallestDebtHighestPayment = 4
    }
}
