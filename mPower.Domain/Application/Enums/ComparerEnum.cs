using System.ComponentModel;

namespace mPower.Domain.Application.Enums
{
    public enum ComparerEnum
    {
        [Description("is equal to")]
        Equal = 0,

        [Description("is greater than")]
        Greater = 1,

        [Description("is less than")]
        Less = 2
    }
}