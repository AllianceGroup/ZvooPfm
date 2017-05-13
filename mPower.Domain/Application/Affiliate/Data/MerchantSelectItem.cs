using System.ComponentModel;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class MerchantSelectItem
    {
        public int Index { get; set; }

        public LogicalOperationEnum Operation { get; set; }

        public string MerchantName { get; set; }
    }

    public enum LogicalOperationEnum
    {
        [Description("and")]
        And,

        [Description("or")]
        Or,

        [Description("not")]
        Not
    }
}