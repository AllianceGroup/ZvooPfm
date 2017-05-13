using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    [BsonIgnoreExtraElements]
    public class SegmentOption
    {
        public bool Enabled { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public ComparerEnum Comparer { get; set; }

        public ConditionEnum Condition { get; set; }

        public FrequencyEnum Frequency { get; set; }

        public OptionTypeEnum Type { get; set; }

        public TrendEnum Trend { get; set; }

        public int Multiplier { get; set; }


        public SegmentOption()
        {
            Multiplier = 1;
            Frequency = FrequencyEnum.Month;
        }
    }
}