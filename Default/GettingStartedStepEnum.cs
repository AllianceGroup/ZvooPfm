using System.ComponentModel;

namespace Default
{
    public enum GettingStartedStepEnum
    {
        [Description("add accounts")]
        AddAccounts = 1,

        [Description("get credit reports")]
        GetCreditReports = 2,

        [Description("you're done")]
        Finish = 3
    }
}