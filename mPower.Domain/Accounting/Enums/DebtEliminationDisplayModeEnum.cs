using System.ComponentModel;

namespace mPower.Domain.Accounting.Enums
{
    public enum DebtEliminationDisplayModeEnum
    {
        [Description("Show Balance Using Program")]
        Program = 0,

        [Description("Show Balances Paying Just Minimums")]
        Minimums = 1,

        [Description("Compare Using Program To Paying Just Minimums")]
        ProgramAndMinimums = 2,
    }
}