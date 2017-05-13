using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Status: AccessRecord
    {
        [CsvField(Name = "fileName")]
        public string FileName { get; set; }

        [CsvField(Name = "lineNumber")]
        public string LineNumber { get; set; }

        [CsvField(Name = "originalRecordIdentifier")]
        public string OriginalRecordId { get; set; }

        [CsvField(Name = "originalRecordType")]
        public string OriginalRecordType { get; set; }

        [CsvField(Name = "recordStatus")]
        public string RecordStatus { get; set; }

        [CsvField(Name = "recordStatusMessage")]
        public string RecordStatusMessage { get; set; }
    }
}