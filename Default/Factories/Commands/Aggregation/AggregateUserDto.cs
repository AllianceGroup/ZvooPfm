
namespace Default.Factories.Commands.Aggregation
{
    public class AggregateUserDto
    {
        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public bool IsAutoUpdate { get; set; }
    }
}