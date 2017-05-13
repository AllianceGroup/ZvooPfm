using Default.Helpers;
using Default.ViewModel.GettingStartedController;

namespace Default.Factories.Commands.Aggregation
{
    public class AggregateAccountsDto
    {
        public DiscoverySession DiscoverySession { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public AssignAccountsToLedgerViewModel Model { get; set; }

        public bool AggregationLoggingEnabled { get; set; }
    }
}