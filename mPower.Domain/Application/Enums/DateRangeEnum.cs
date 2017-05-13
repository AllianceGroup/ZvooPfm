using System.ComponentModel;

namespace mPower.Domain.Application.Enums
{
    public enum DateRangeEnum
    {
        [Description("Last 30 days")]
        Last30Days,

        [Description("Last 60 days")]
        Last60Days,

        [Description("Last 90 days")]
        Last90Days,

        [Description("custom range")]
        Custom
    }
}