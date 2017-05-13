using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Channel: AccessRecord
    {
        [CsvField(Name = "channelIdentifier")]
        public string ChannelId { get; set; }

        [CsvField(Name = "channelName")]
        public string Name { get; set; }

        [CsvField(Name = "channelDescription")]
        public string Description { get; set; }

        [CsvField(Name = "channelLogoName")]
        public string LogoName { get; set; }
    }
}