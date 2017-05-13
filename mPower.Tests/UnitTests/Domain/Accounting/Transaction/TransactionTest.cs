using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework.Utils;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public abstract class TransactionTest : AggregateTest<TransactionAR>
    {
        public const string UserId = "user_123";
        public const string ApplicationId = "app_123";

        private readonly DateTime _currentDate = DateTime.Now;

        public DateTime CurrentDate
        {
            get { return _currentDate; }
        }

        public void CheckBalance(params KeyValuePair<string, long>[] balances)
        {
            var ledgerService = _container.GetInstance<LedgerDocumentService>();
            var ledger = ledgerService.GetById(_id);

            foreach (var balance in balances)
            {
                Assert.AreEqual(ledger.Accounts.Single(x => x.Id == balance.Key).Denormalized.Balance, balance.Value);
            }
        }

        #region Events

        public IEvent Ledger_Created()
        {
            return new Ledger_CreatedEvent
            {
                LedgerId = _id,
                Name = "Sample Company, LLC",
                FiscalYearStart = DateUtil.GetStartOfCurrentYear().AddMonths(6),
                CreatedDate = DateTime.Now.AddYears(-2)
            };
        }

        public IEvent Ledger_Account_Added(String id, AccountTypeEnum typeEnum, AccountLabelEnum labelEnum, string intuitAccountId = null)
        {
            var acc = new Ledger_Account_AddedEvent
            {
                AccountId = id,
                AccountTypeEnum = typeEnum,
                AccountLabelEnum = labelEnum,
                LedgerId = _id,
                Name = id,
                Aggregated = !string.IsNullOrEmpty(intuitAccountId),
                YodleeItemAccountId = intuitAccountId,
                Metadata = {StoredDate = DateTime.Now},
            };

            return acc;
        }

        public IEvent Ledger_Account_BalanceChanged(String accountId, Int64 balance)
        {
            return new Ledger_Account_BalanceChangedEvent
            {
                AccountId = accountId,
                LedgerId = _id,
                BalanceInCents = balance,
                UserId = UserId,
            };
        }

        public Transaction_CreatedEvent Transaction_Created(String transactionId,TransactionType type = TransactionType.Bill)
        {
            return new Transaction_CreatedEvent
            {
                LedgerId = _id,
                TransactionId = transactionId,
                Type = type,
                UserId = UserId,
                Metadata = {UserId = UserId},
            };
        }

        public Transaction_ModifiedEvent Transaction_Modified(String transactionId, TransactionType type = TransactionType.Bill)
        {
            return new Transaction_ModifiedEvent
            {
                LedgerId = _id,
                TransactionId = transactionId,
                Type = type,
            };
        }

        public Transaction_DeletedEvent Transaction_Deleted(String transactionId)
        {
            return new Transaction_DeletedEvent()
            {
                LedgerId = _id,
                TransactionId = transactionId,
            };
        }

        #endregion

        #region Commands

        public ICommand Ledger_Account_Create(String accountId, AccountTypeEnum accountTypeEnum, AccountLabelEnum accountLabelEnum)
        {
            return new Ledger_Account_CreateCommand
            {
                AccountId = accountId,
                AccountTypeEnum = accountTypeEnum,
                AccountLabelEnum = accountLabelEnum,
                LedgerId = _id,
                Name = accountId,
            };
        }

        public Transaction_CreateCommand Transaction_Create(String transactionId, TransactionType type = TransactionType.Bill)
        {
            return new Transaction_CreateCommand
            {
                LedgerId = _id,
                TransactionId = transactionId,
                Type = type,
                UserId = UserId,
            };
        }

        #endregion
      
    }

    public static class LedgerEventsExtensions
    {
        public static Transaction_CreatedEvent AddEntry(this Transaction_CreatedEvent evnt, String accountId, Int64 debit, Int64 credit, DateTime bookedDate, AccountTypeEnum accountType, AccountLabelEnum accountLabel, string offsetAccountId, string offsetAccountName)
        {   
            var entry = new ExpandedEntryData
            {
                AccountId = accountId,
                BookedDate = bookedDate,
                CreditAmountInCents = credit,
                DebitAmountInCents = debit,
                AccountType = accountType,
                AccountLabel = accountLabel,
                AccountName = accountId,
                OffsetAccountId = offsetAccountId,
                OffsetAccountName = offsetAccountName,
                LedgerId = evnt.LedgerId,
                TransactionId = evnt.TransactionId,
                Memo = null,
                Payee = null,
                TransactionImported = false
            };

            if (evnt.Entries == null)
                evnt.Entries = new List<ExpandedEntryData>();

            evnt.Entries.Add(entry);
            return evnt;
        }

        public static Transaction_ModifiedEvent AddEntry(this Transaction_ModifiedEvent evnt, String accountId, Int64 debit, Int64 credit, DateTime bookedDate, AccountTypeEnum accountType, AccountLabelEnum accountLabel, string offsetAccountId, string offsetAccountName)
        {
            var entry = new ExpandedEntryData
            {
                AccountId = accountId,
                BookedDate = bookedDate,
                CreditAmountInCents = credit,
                DebitAmountInCents = debit,
                AccountType = accountType,
                AccountLabel = accountLabel,
                AccountName = accountId,
                OffsetAccountId = offsetAccountId,
                OffsetAccountName = offsetAccountName,
                LedgerId = evnt.LedgerId,
                TransactionId = evnt.TransactionId,
                Memo = null,
                Payee = null,
                TransactionImported = false
            };

            if (evnt.Entries == null)
                evnt.Entries = new List<ExpandedEntryData>();

            evnt.Entries.Add(entry);
            return evnt;
        }
    }

    public static class LedgerCommandsExtensions
    {
        public static Transaction_CreateCommand AddEntry(this Transaction_CreateCommand evnt, String accountId, Int64 debit, Int64 credit, DateTime bookedDate, AccountTypeEnum accountType, AccountLabelEnum accountLabel, string offsetAccountId = null, string offsetAccountName = null)
        {
            var entry = new ExpandedEntryData()
            {
                AccountId = accountId,
                BookedDate = bookedDate,
                CreditAmountInCents = credit,
                DebitAmountInCents = debit,
                AccountType = accountType,
                AccountLabel = accountLabel,
                AccountName = accountId,
                OffsetAccountId = offsetAccountId,
                OffsetAccountName = offsetAccountName,
                LedgerId = evnt.LedgerId,
                TransactionId = evnt.TransactionId,
                Memo = null,
                Payee = null,
                TransactionImported = false
            };

            if (evnt.Entries == null)
                evnt.Entries = new List<ExpandedEntryData>();

            evnt.Entries.Add(entry);
            return evnt;
        }
    }
}

