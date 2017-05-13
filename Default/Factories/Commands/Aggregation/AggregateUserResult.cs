using System.Collections.Generic;
using mPower.Domain.Accounting.Ledger.Commands;
using Paralect.Domain;

namespace Default.Factories.Commands.Aggregation
{
    public class AggregateUserResult
    {
        public AggregateUserResult()
        {
            SetStatusCommands = new List<Ledger_Account_AggregationStatus_UpdateCommand>();
            IntuitAccountsIds = new List<long>();
        }

        public List<Ledger_Account_AggregationStatus_UpdateCommand> SetStatusCommands { get; set; }

        public List<long> IntuitAccountsIds { get; set; }

        public ICommand SetUserAutoUpdateCommand { get; set; }
    }
}