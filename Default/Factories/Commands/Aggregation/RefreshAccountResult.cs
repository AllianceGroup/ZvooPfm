using mPower.Domain.Accounting.Ledger.Commands;

namespace Default.Factories.Commands.Aggregation
{
    public class RefreshAccountResult
    {
        public Ledger_Account_AggregationStatus_UpdateCommand SetStatusCommand { get; set; }

        public bool PullTransactions { get; set; }
    }
}