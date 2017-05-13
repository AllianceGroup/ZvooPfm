using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Framework;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class BudgetDocumentEventHandler :
        IMessageHandler<Entries_AddedMessage>,
        IMessageHandler<Entries_RemovedMessage>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>
    {
        private readonly IEventService _eventService;
        private readonly BudgetDocumentService _budgetService;

        public BudgetDocumentEventHandler(IEventService eventService, BudgetDocumentService budgetService)
        {
            _eventService = eventService;
            _budgetService = budgetService;
        }

        public void Handle(Entries_AddedMessage message)
        {
            if (message.Entries.Count > 0)
            {
                var date = message.Entries[0].BookedDate;
                var exceededBudgetsIdsBefore = GetExceededBudgets(message.LedgerId, date.Year, date.Month).Select(b => b.Id);

                AddNewEntriesSpentAmount(message.LedgerId, message.Entries);

                var exceededBudgetsAfter = GetExceededBudgets(message.LedgerId, date.Year, date.Month);
                var newExceededBudgets = exceededBudgetsAfter.Where(b => !exceededBudgetsIdsBefore.Contains(b.Id)).ToList();
                ThrowNewBudgetsExceededEvents(newExceededBudgets, message.UserId, date);
            }
        }

        public void Handle(Entries_RemovedMessage message)
        {
            if (message.Entries == null || message.Entries.Count == 0)
                return;

            var date = message.Entries[0].BookedDate;
            foreach (var entry in message.Entries)
            {
                var spentAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType
                                      (entry.DebitAmountInCents, entry.CreditAmountInCents, entry.AccountType) * -1;

                var query = Query.And(
                    Query.EQ("LedgerId", message.LedgerId),
                    Query.EQ("AccountId", entry.AccountId),
                    Query.EQ("Month", date.Month),
                    Query.EQ("Year", date.Year));

                var update = Update.Inc("SpentAmount", spentAmount);
                _budgetService.Update(query, update);


                query = Query.And(
                    Query.EQ("LedgerId", message.LedgerId),
                    Query.EQ("SubBudgets.AccountId", entry.AccountId),
                    Query.EQ("Month", date.Month),
                    Query.EQ("Year", date.Year));

                update = Update.Inc("SubBudgets.$.SpentAmount", spentAmount);
                _budgetService.Update(query, update);
            }
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var query = Query.EQ("LedgerId", message.LedgerId);

            _budgetService.Remove(query);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            var query = Query.And(
                   Query.EQ("LedgerId", message.LedgerId),
                   Query.EQ("AccountId", message.AccountId));


            _budgetService.UpdateMany(query, Update<BudgetDocument>.Set(x => x.AccountName, message.Name));

            if (!String.IsNullOrEmpty(message.ParentAccountId))
            {
                query = Query.And(
                    Query.EQ("LedgerId", message.LedgerId),
                    Query.EQ("SubBudgets.AccountId", message.AccountId));

                _budgetService.UpdateMany(query, Update.Set("SubBudgets.$.AccountName", BsonValue.Create(message.Name) ?? BsonNull.Value));
            }
        }

        private void AddNewEntriesSpentAmount(string ledgerId, List<ExpandedEntryData> entries)
        {
            if (entries == null || entries.Count == 0)
                return;

            var date = entries[0].BookedDate;
            foreach (var entry in entries)
            {
                var spentAmount =
                    AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(entry.DebitAmountInCents,
                                                                                                 entry.CreditAmountInCents,
                                                                                                 entry.AccountType);

                var query = Query.And(
                    Query.EQ("LedgerId", ledgerId),
                    Query.EQ("AccountId", entry.AccountId),
                    Query.EQ("Month", date.Month),
                    Query.EQ("Year", date.Year));

                var update = Update.Inc("SpentAmount", spentAmount);
                _budgetService.Update(query, update);

                query = Query.And(
                    Query.EQ("LedgerId", ledgerId),
                    Query.EQ("SubBudgets.AccountId", entry.AccountId),
                    Query.EQ("Month", date.Month),
                    Query.EQ("Year", date.Year));

                update = Update.Inc("SubBudgets.$.SpentAmount", spentAmount);
                _budgetService.Update(query, update);
            }
        }

        private IEnumerable<BudgetDocument> GetExceededBudgets(string ledgerId, int year, int month)
        {
            var filter = new BudgetFilter
            {
                LedgerId = ledgerId,
                Year = year,
                Month = month,
            };
            return _budgetService.GetByFilter(filter).Where(b => b.SpentAmountWithSubAccounts > b.BudgetAmount);
        }

        private void ThrowNewBudgetsExceededEvents(IEnumerable<BudgetDocument> budgets, string userId, DateTime date)
        {
            foreach (var budgetDocument in budgets)
            {
                var newEvent = new Ledger_Budget_ExceededEvent
                {
                    BudgetId = budgetDocument.Id,
                    LedgerId = budgetDocument.LedgerId,
                    AccountId = budgetDocument.AccountId,
                    AccountName = budgetDocument.AccountName,
                    Month = budgetDocument.Month,
                    BudgetAmount = budgetDocument.BudgetAmount,
                    SpentAmountWithSubAccounts = budgetDocument.SpentAmountWithSubAccounts,
                    Date = date,
                    Metadata = { UserId = userId },
                };
                _eventService.Send(newEvent);
            }
        }
    }
}
