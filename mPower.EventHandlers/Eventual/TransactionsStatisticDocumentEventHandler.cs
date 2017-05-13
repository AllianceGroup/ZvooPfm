using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Framework;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class TransactionsStatisticDocumentEventHandler :
        IMessageHandler<Entries_AddedMessage>,
        IMessageHandler<Entries_RemovedMessage>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>
    {
        private readonly TransactionsStatisticDocumentService _statisticService;
        private readonly IEventService _eventService;

        public TransactionsStatisticDocumentEventHandler(TransactionsStatisticDocumentService statisticService, IEventService eventService)
        {
            _statisticService = statisticService;
            _eventService = eventService;
        }

        public void Handle(Entries_AddedMessage message)
        {
            if (message.Entries.Count > 0)
            {
                var date = message.Entries[0].BookedDate;
                var groupedEntries = GroupData(message.Entries.Select(e => new SimpleEntry(e.AccountId, e.AccountType,e.AccountName, e.DebitAmountInCents, e.CreditAmountInCents)));
                foreach (var entGroup in groupedEntries)
                {
                    _statisticService.AddStatistic(message.LedgerId, entGroup.AccountId, entGroup.AccountType, date, entGroup.DebitAmountInCents, entGroup.CreditAmountInCents);
                }

                var statisticAddedMessage = new Transaction_Statistic_AddedMessage
                {
                    UserId = message.UserId,
                    LedgerId = message.LedgerId,
                    TransactionId = message.TransactionId,
                    Entries = message.Entries
                };
                _eventService.Send(statisticAddedMessage);
            }
        }

        public void Handle(Entries_RemovedMessage message)
        {
            if (message.Entries.Count > 0)
            {

                var date = message.Entries[0].BookedDate;
                var groupedEntries = GroupData(message.Entries.Select(e => new SimpleEntry(e.AccountId, e.AccountType,e.AccountName, e.DebitAmountInCents, e.CreditAmountInCents)));
                foreach (var entGroup in groupedEntries)
                {
                    _statisticService.AddStatistic(message.LedgerId, entGroup.AccountId, entGroup.AccountType, date, -entGroup.DebitAmountInCents, -entGroup.CreditAmountInCents);
                }
            }
        }

        private static IEnumerable<SimpleEntry> GroupData(IEnumerable<SimpleEntry> entries)
        {
            var result = new List<SimpleEntry>();
            var groups = entries.GroupBy(e => e.AccountId);
            foreach (var entGroup in groups)
            {
                var debit = entGroup.Sum(eg => eg.DebitAmountInCents);
                var credit = entGroup.Sum(eg => eg.CreditAmountInCents);
                var accountFirstEntry = entGroup.FirstOrDefault();
                if ((debit != 0 || credit != 0) && accountFirstEntry != null)
                {
                    result.Add(new SimpleEntry(entGroup.Key, accountFirstEntry.AccountType, accountFirstEntry.AccountName, debit, credit));
                }
            }
            return result;
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var filter = new TransactionsStatisticFilter { LedgerId = message.LedgerId };
            _statisticService.Remove(filter);
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var filter = new TransactionsStatisticFilter { LedgerId = message.LedgerId, AccountId = message.AccountId };

            _statisticService.Remove(filter);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            var query = Query.EQ("AccountId", message.AccountId);
            var update = Update.Set("AccountName", message.Name);
            _statisticService.Update(query,update);
        }
    }
}
