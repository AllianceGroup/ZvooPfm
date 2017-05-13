using System.ComponentModel;

namespace mPower.Domain.Accounting.Enums
{
    public enum ContentServiceTypesEnum
    {
        [Description("Banks")]
        bank,
        [Description("Credit Cards")]
        credits,
        [Description("Stocks")]
        stocks,
        bill_payment,
        [Description("Loans")]
        loans,
        [Description("Mortgages")]
        mortgage,
        mail,
        insurance,
        miles,
        bills,
        cable_satellite,
        utilities,
        other_assets,
        isp,
        minutes,
		news,

		unknown
    }
}