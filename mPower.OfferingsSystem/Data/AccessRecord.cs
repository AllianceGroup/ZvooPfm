using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class AccessRecord
    {
        [CsvField(Name = "recordIdentifier")]
        public string RecordId { get; set; }
        [CsvField(Name = "recordType")]
        public string RecordType { get; set; }
    }
}