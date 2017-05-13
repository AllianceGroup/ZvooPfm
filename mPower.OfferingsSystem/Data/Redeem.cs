using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Redeem: AccessRecord
    {
        [CsvField(Name = "redeemIdentifier")]
        public string RedeemId { get; set; }

        [CsvField(Name = "publicationChannels")]
        public string PublicationChannels { get; set; }

        [CsvField(Name = "redeemMethod")]
        public string Method { get; set; }

        [CsvField(Name = "redeemInstruction")]
        public string Instruction { get; set; }

        [CsvField(Name = "redeemCode")]
        public string Code { get; set; }

        [CsvField(Name = "redeemCouponName")]
        public string CouponName { get; set; }
    }
}