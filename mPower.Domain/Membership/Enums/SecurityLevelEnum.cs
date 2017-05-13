using System.ComponentModel;

namespace mPower.Domain.Membership.Enums
{
    public enum SecurityLevelEnum
    {
        [Description("Level 1 (Username and password only)")]
        LoginAndPassword = 0,

        [Description("Level 2 (Username, password and question)")]
        LoginPasswordAndQuestion = 1,
    }
}