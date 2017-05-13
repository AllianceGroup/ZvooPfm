using mPower.Domain.Accounting.Enums;

namespace Default.Models
{
    public class AggregationStatusModel
    {
        public AggregatedAccountStatusEnum Status { get; set; }
        public string Message { get; set; }
        public string AggregationExceptionId { get; set; }
        public long ContentServiceId { get; set; }
        public long IntuitAccountId { get; set; }
        public string ErrorId { get; set; }
    }
}