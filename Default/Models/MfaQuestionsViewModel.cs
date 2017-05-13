using mPower.Aggregation.Contract.Data;

namespace Default.Models
{
    public class MfaQuestionsViewModel
    {
        public long ContentServiceId { get; set; }
        public AggregationQuestion[] Questions { get; set; }
        public long? AccountId { get; set; }
    }
}