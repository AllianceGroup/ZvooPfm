using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.EventHandlers.Immediate.Dto;
using mPower.Framework;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class EntryDocumentEventHandler :
        IMessageHandler<Transaction_CreatedEvent>,
        IMessageHandler<Transaction_ModifiedEvent>,
        IMessageHandler<Transaction_DeletedEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>
    {
        private readonly EntryDocumentService _entryDocumentService;
        private readonly IEventService _eventService;
        private readonly LedgerDocumentService _ledgerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EntryDocumentEventHandler(
            EntryDocumentService entryDocumentService,
            IEventService eventService,
            LedgerDocumentService ledgerService)
        {
            _entryDocumentService = entryDocumentService;
            _eventService = eventService;
            _ledgerService = ledgerService;
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            SaveEntries(message.Entries, message.TransactionId, message.LedgerId, message.UserId ?? message.Metadata.UserId);
        }

        public void Handle(Transaction_ModifiedEvent message)
        {
            var balanceAccounts = new List<AccountBalanceDto>();
            RemoveEntries(message.TransactionId, message.LedgerId, message.Metadata.UserId, balanceAccounts, false);
            SaveEntries(message.Entries, message.TransactionId, message.LedgerId, message.Metadata.UserId, balanceAccounts);
        }

        public void Handle(Transaction_DeletedEvent message)
        {
            RemoveEntries(message.TransactionId, message.LedgerId, message.Metadata.UserId);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            //Query 1
            var query = Query.And(Query.EQ("AccountId", message.AccountId), Query.EQ("LedgerId", message.LedgerId));
            var update = Update<EntryDocument>.Set(x => x.AccountName, message.Name);

            //Query 2
            var query2 = Query.And(Query.EQ("OffsetAccountId", message.AccountId), Query.EQ("LedgerId", message.LedgerId));
            var update2 = Update<EntryDocument>.Set(x => x.OffsetAccountName, message.Name);

            _entryDocumentService.UpdateMany(query, update);
            _entryDocumentService.UpdateMany(query2, update2);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var filter = new EntryFilter { LedgerId = message.LedgerId };
            _entryDocumentService.Remove(filter);
        }

        private void SaveEntries(List<ExpandedEntryData> rawEntries, string transactionId, string ledgerId, string userId, List<AccountBalanceDto> balanceAccounts = null, bool applyUpdatedBalance = true)
        {
            if (rawEntries == null || rawEntries.Count == 0)
            {
                return;
            }
            var entries = Map(rawEntries, transactionId, ledgerId);
            _entryDocumentService.InsertMany(entries);
            if (balanceAccounts == null)
            {
                balanceAccounts = new List<AccountBalanceDto>();
            }
            UpdateBalanceAccounts(balanceAccounts, rawEntries.Select(MapAccountBalance));
            if (applyUpdatedBalance)
            {
                SendUpdateBalanceEvent(balanceAccounts, ledgerId, userId);
            }
            var entriesAddedMessage = new Entries_AddedMessage
            {
                UserId = userId,
                LedgerId = ledgerId,
                TransactionId = transactionId,
                Entries = rawEntries
            };
            _eventService.Send(entriesAddedMessage);
        }

        private void RemoveEntries(string transactionId, string ledgerId, string userId, List<AccountBalanceDto> balanceAccounts = null, bool applyUpdatedBalance = true)
        {
            var filter = new EntryFilter { LedgerId = ledgerId, TransactionId = transactionId };
            var entriesToRemove = _entryDocumentService.GetByFilter(filter);
            if (balanceAccounts == null)
            {
                balanceAccounts = new List<AccountBalanceDto>();
            }
            if (entriesToRemove == null || entriesToRemove.Count == 0)
            {
                return;
            }
            _entryDocumentService.Remove(filter);
            UpdateBalanceAccounts(balanceAccounts, entriesToRemove.Select(MapAccountBalance), true);
            if (applyUpdatedBalance)
            {
                SendUpdateBalanceEvent(balanceAccounts, ledgerId, userId);
            }
            var entriesRemovedMessage = new Entries_RemovedMessage
            {
                UserId = userId,
                LedgerId = ledgerId,
                TransactionId = transactionId,
                Entries = entriesToRemove.Select(MapEntry).ToList()
            };
            _eventService.Send(entriesRemovedMessage);
        }

        private void SendUpdateBalanceEvent(IEnumerable<AccountBalanceDto> accounts, string ledgerId, string userId)
        {
            var ledger = _ledgerService.GetById(ledgerId);
            if (ledger != null)
            {
                foreach (var acc in accounts)
                {
                    var account = ledger.Accounts.Find(a => a.Id == acc.Id);
                    if (account != null)
                    {
                        var currentBalance = account.Denormalized.Balance;
                        var balance = currentBalance + acc.BalanceDelta;
                        var evt = new Ledger_Account_BalanceChangedEvent
                        {
                            UserId = userId,
                            AccountId = acc.Id,
                            AccountName = acc.Name,
                            AccountLabel = account.LabelEnum,
                            LedgerId = ledgerId,
                            OldValueInCents = currentBalance,
                            BalanceInCents = balance,
                            Date = acc.Date,
                        };
                        _eventService.Send(evt);
                    }
                }
            }
        }

        private void UpdateBalanceAccounts(List<AccountBalanceDto> balanceAccounts, IEnumerable<AccountBalanceDto> entries, bool negative = false)
        {
            var multiplier = negative ? -1 : 1;
            foreach (var x in entries)
            {
                var balanceDelta =
                    AccountingFormatter.
                        FormatDebitCreditToPositiveOrNegativeNumberByAccountType
                        (x.DebitAmountInCents, x.CreditAmountInCents,
                         x.AccountType) * multiplier;

                var currentBalanceAccount = balanceAccounts.SingleOrDefault(y => y.Id == x.Id);
                if (currentBalanceAccount == null)
                {
                    balanceAccounts.Add(new AccountBalanceDto { Id = x.Id, Name = x.Name, BalanceDelta = balanceDelta, Date = x.Date });
                }
                else
                {
                    currentBalanceAccount.BalanceDelta += balanceDelta;
                }
            }
        }

        public static List<EntryDocument> Map(List<ExpandedEntryData> rawEntries, string transactionId, string ledgerId)
        {
            int index = 0;

            var entries = new List<EntryDocument>();

            foreach (var x in rawEntries)
            {
                entries.Add(new EntryDocument()
                {
                    Id = String.Format("{0}e{1}", transactionId, index),
                    AccountId = x.AccountId,
                    BookedDate = x.BookedDate,
                    CreditAmountInCents = x.CreditAmountInCents,
                    DebitAmountInCents = x.DebitAmountInCents,
                    BookedDateString = x.BookedDate.ToString("MM-dd-yyyy"),
                    Payee = x.Payee,
                    Memo = x.Memo,
                    TransactionId = transactionId,
                    LedgerId = ledgerId,
                    FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(x.DebitAmountInCents - x.CreditAmountInCents, true),
                    AccountLabel = x.AccountLabel,
                    AccountType = x.AccountType,
                    AccountName = x.AccountName,
                    OffsetAccountId = x.OffsetAccountId,
                    OffsetAccountName = x.OffsetAccountName,
                    Imported = x.TransactionImported
                });

                index++;
            }
            return entries;
        }

        private AccountBalanceDto MapAccountBalance(ExpandedEntryData entry)
        {
            return new AccountBalanceDto
            {
                AccountType = entry.AccountType,
                CreditAmountInCents = entry.CreditAmountInCents,
                DebitAmountInCents = entry.DebitAmountInCents,
                Id = entry.AccountId,
                Name = entry.AccountName,
                Date = entry.BookedDate,
            };
        }

        private AccountBalanceDto MapAccountBalance(EntryDocument entry)
        {
            return new AccountBalanceDto
            {
                AccountType = entry.AccountType,
                CreditAmountInCents = entry.CreditAmountInCents,
                DebitAmountInCents = entry.DebitAmountInCents,
                Id = entry.AccountId,
                Name = entry.AccountName,
                Date = entry.BookedDate,
            };
        }

        private ExpandedEntryData MapEntry(EntryDocument doc)
        {
            return new ExpandedEntryData
            {
                AccountId = doc.AccountId,
                AccountName = doc.AccountName,
                AccountType = doc.AccountType,
                AccountLabel = doc.AccountLabel,
                OffsetAccountName = doc.OffsetAccountName,
                OffsetAccountId = doc.OffsetAccountId,
                BookedDate = doc.BookedDate,
                CreditAmountInCents = doc.CreditAmountInCents,
                DebitAmountInCents = doc.DebitAmountInCents,
                Payee = doc.Payee,
                Memo = doc.Memo,
                LedgerId = doc.LedgerId,
                TransactionId = doc.TransactionId,
                TransactionImported = doc.Imported,
            };
        }
    }
}
