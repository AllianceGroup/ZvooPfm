using System.Linq;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.TempDocuments.Server.Notifications.Messages;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class DashboardAlertsHandler :
        IMessageHandler<AlertsRemovedMessage>,
        IMessageHandler<Transaction_Statistic_AddedMessage>,
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Ledger_Budget_ExceededEvent>,
        IMessageHandler<Ledger_Account_AggregatedBalanceUpdatedEvent>
    {
        private readonly DashboardAlertBuilder _builder;

        public DashboardAlertsHandler(DashboardAlertBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(AlertsRemovedMessage message)
        {
            _builder.RemoveAlerts(message.Ids, message.PublicKeys, message.Type);
        }

        public void Handle(Transaction_Statistic_AddedMessage message)
        {
            if (message.Entries.Count > 0)
            {
                _builder.CheckForUnusualSpending(message.UserId, message.LedgerId, message.Entries, message.Entries.First().BookedDate);
                _builder.CheckForLargePurchase(message.UserId, message.LedgerId, message.Entries, message.Entries.First().BookedDate);
            }
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            var data = new BalanceChangedData
            {
                UserId = message.UserId,
                LedgerId = message.LedgerId,
                AccountId = message.AccountId,
                AccountName = message.AccountName,
                OldValueInCents = message.OldValueInCents,
                BalanceInCents = message.BalanceInCents,
                Date = message.Date,
            };
            _builder.CheckAvailableCredit(data);
            _builder.CheckForLowBalance(data);
        }

        public void Handle(Ledger_Account_AggregatedBalanceUpdatedEvent message)
        {
            var data = new BalanceChangedData
            {
                UserId = message.UserId,
                LedgerId = message.LedgerId,
                AccountId = message.AccountId,
                AccountName = message.AccountName,
                OldValueInCents = message.OldValueInCents,
                BalanceInCents = message.NewBalance,
                Date = message.Date,
            };
            _builder.CheckAvailableCredit(data);
            _builder.CheckForLowBalance(data);
        }

        public void Handle(Ledger_Budget_ExceededEvent message)
        {
            var data = new BudgetExceededData
            {
                UserId = message.Metadata.UserId,
                BudgetId = message.BudgetId,
                LedgerId = message.LedgerId,
                AccountName = message.AccountName,
                Month = message.Month,
                BudgetAmount = message.BudgetAmount,
                SpentAmountWithSubAccounts = message.SpentAmountWithSubAccounts,
                Date = message.Date,
            };

            _builder.CreateOverBudget(data);
        }
    }
}