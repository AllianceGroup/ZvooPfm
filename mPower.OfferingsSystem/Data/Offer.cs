using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Offer: AccessRecord
    {
        [CsvField(Name = "offerIdentifier")]
        public string OfferId { get; set; }

        [CsvField(Name = "offerDataIdentifier")]
        public string OfferDataId { get; set; }

        [CsvField(Name = "locationIdentifier")]
        public string LocationId { get; set; }

        [CsvField(Name = "startDate")]
        public string StartDate { get; set; }

        [CsvField(Name = "endDate")]
        public string EndDate { get; set; }

        [CsvField(Name = "categoryIdentifier")]
        public string CategoryId { get; set; }

        [CsvField(Name = "offerType")]
        public string OfferType { get; set; }

        [CsvField(Name = "expressionType")]
        public string ExpressionType { get; set; }

        [CsvField(Name = "award")]
        public string Award { get; set; }

        [CsvField(Name = "minimumPurchase")]
        public string MinimumPurchase { get; set; }

        [CsvField(Name = "maximumAward")]
        public string MaximumAward { get; set; }

        [CsvField(Name = "taxRate")]
        public string TaxRate { get; set; }

        [CsvField(Name = "tipRate")]
        public string TipRate { get; set; }

        [CsvField(Name = "description")]
        public string Desciption { get; set; }

        [CsvField(Name = "awardRating")]
        public string AwardRating { get; set; }

        [CsvField(Name = "dayExclusions")]
        public string DayExclusions { get; set; }

        [CsvField(Name = "monthExclusions")]
        public string MonthExclusions { get; set; }

        [CsvField(Name = "dateExclusions")]
        public string DateExclusions { get; set; }

        [CsvField(Name = "redemptions")]
        public string Redemptions { get; set; }

        [CsvField(Name = "redemptionPeriod")]
        public string RedemptionPeriod { get; set; }

        [CsvField(Name = "redeemIdentifiers")]
        public string RedeemIdentifiers { get; set; }

        [CsvField(Name = "terms")]
        public string Terms { get; set; }

        [CsvField(Name = "disclaimer")]
        public string Disclaimer { get; set; }

        [CsvField(Name = "offerPhotoNames")]
        public string OfferPhotoNames { get; set; }

        [CsvField(Name = "keywords")]
        public string Keywords { get; set; }
    }
}