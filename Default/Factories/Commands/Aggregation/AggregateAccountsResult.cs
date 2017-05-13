using System.Collections.Generic;
using mPower.Domain.Accounting.Ledger.Commands;

namespace Default.Factories.Commands.Aggregation
{
    public class AggregateAccountsResult
    {
        public AggregateAccountsResult()
        {
            AddAccountCommands = new List<Ledger_Account_CreateCommand>();
            UpdateBalanceCommands = new List<Ledger_Account_AggregatedBalanceUpdateCommand>();
            SetStatusCommands = new List<Ledger_Account_AggregationStatus_UpdateCommand>();
            IntuitAccountsIds = new List<long>();
        }

        public List<Ledger_Account_CreateCommand> AddAccountCommands { get; set; }

        public List<Ledger_Account_AggregatedBalanceUpdateCommand> UpdateBalanceCommands { get; set; }

        public List<Ledger_Account_AggregationStatus_UpdateCommand> SetStatusCommands { get; set; }

        public List<long> IntuitAccountsIds { get; set; }
    }
}