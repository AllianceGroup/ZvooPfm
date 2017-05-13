
namespace Default.Factories.Commands.Aggregation
{
    public class AuthenticateDto
    {
        public long InstitutionId { get; set; }

        public string UserId { get; set; }

        public bool AggregationLoggingEnabled { get; set; }
    }
}