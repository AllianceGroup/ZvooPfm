using System.ComponentModel;

namespace mPower.Domain.Application.Enums
{
    public enum OfferTypeEnum
    {
        [Description("Inline Transaction")]
        InlineTransaction = 0,
        Email = 1,
        Sms = 2
    }
}