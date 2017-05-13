using System.ComponentModel;

namespace mPower.Domain.Application.Enums
{
    public enum FrequencyEnum
    {
        [Description("per year")]
        Year,

        [Description("per month")]
        Month,

        [Description("per week")]
        Week,
    }
}