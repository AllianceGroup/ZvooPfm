using mPower.Aggregation.Contract.Domain.Data;
using mPower.Aggregation.Contract.Domain.Enums;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Membership.User.Events;
using mPower.EventHandlers;
using NUnit.Framework;
using Paralect.Domain;
using System.Collections.Generic;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Aggregation
{
    [TestFixture]
    public class AggregationCallbackTest: TransactionTest
    {
        private new const string UserId = "user1";
        private AggregationCallback _callback;

        [TestFixtureSetUp]
        public void SetUp()
        {
            // we have to call this method right here because of dispatching events at "PrepareEvents" step
            ApplyCreateNewDatabaseSetting(true);

            _callback = _container.GetInstance<AggregationCallback>();
        }

        public override IEnumerable<IEvent> Given()
        {
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = "application1"};
            yield return Ledger_Created();
            yield return new Ledger_User_AddedEvent {LedgerId = _id, UserId = UserId};
            yield return Ledger_Account_Added("Aggregated Account 1", AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Aggregated Intuit Account 1");
            yield return Ledger_Account_Added("Aggregated Account 2", AccountTypeEnum.Asset, AccountLabelEnum.Investment, "Aggregated Intuit Account 2");
        }

        /// <summary>
        /// Dispatching event before "When" mothod executed
        /// becouce we need created fake ledger in Intuit_Transactions_CreateCommandHandler
        /// </summary>
        protected override void PrepareEvents()
        {
            var @events = Given();
            foreach (var @event in events)
            {
                _eventDispatcher.Dispatch(@event);
            }
            base.PrepareEvents();
        }

        public override IEnumerable<ICommand> When()
        {
            var accountBalanceUpdates = new List<AggregatedAccountBalanceUpdateData>
            {
                new AggregatedAccountBalanceUpdateData
                {
                    LedgerAccountId = "Aggregated Account 2",
                    LedgerId = _id,
                    AccountName = "Aggregated Account 2 Name",
                    NewBalance = 1,
                    OldValue = 10,
                    LogonId = UserId,
                }
            };

            var transactions = new List<AggregatedTransactionData>
            {
                new AggregatedTransactionData
                {
                    IntuitAccountId = 1,
                    AmountInCents = 1000,
                    LedgerId = _id,
                    //FeeAmountInCents = 1000,
                    Id = "id1",
                    ConfirmationNumber = "c1",
                    Description = "desc",
                    //EscrowAmountInCents = 1000,
                    //InterestAmountInCents = 1000,
                    IntuitTransactionType = AggregatedTransactionType.Bank,
                    LedgerAccountId = "Aggregated Account 1",
                    LedgerAccountName = "Ledger Account Name 1",
                    NormalizedDescription = "nord desc",
                    PostedDate = CurrentDate,
                    //PrincipalAmountInCents = 1000,
                    Categories = new List<string> {"Aggregated Account 2"},
                    ReferenceNumber = "123"
                }
            };

            _callback.TransactionsAggregated(_id, transactions, accountBalanceUpdates);

            yield break;
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Transaction_CreatedEvent
            {
                BaseEntryAccountId = "Aggregated Account 1",
                BaseEntryAccountType = AccountTypeEnum.Asset,
                Imported = true,
                ImportedTransactionId = "id1",
                IsMultipleInsert = true,
                UserId = UserId,
                LedgerId = _id,
                Memo = "desc",
                ReferenceNumber = "123",
                TransactionId = "id1",
                Type = TransactionType.Deposit,
                Entries = new List<ExpandedEntryData>
                {
                    new ExpandedEntryData
                    {
                        AccountId = "Aggregated Account 1",
                        AccountLabel = AccountLabelEnum.Bank,
                        AccountName = "Ledger Account Name 1",
                        AccountType = AccountTypeEnum.Asset,
                        //BookedDate= {3/19/2012 5:04:01 PM},
                        CreditAmountInCents = 0,
                        DebitAmountInCents = 1000,
                        LedgerId = _id,
                        Memo = "desc",
                        OffsetAccountId = "Aggregated Account 2",
                        OffsetAccountName = "Aggregated Account 2",
                        Payee = "nord desc",
                        TransactionId = "id1",
                        TransactionImported = true
                    },
                    new ExpandedEntryData
                    {
                        AccountId = "Aggregated Account 2",
                        AccountLabel = AccountLabelEnum.Investment,
                        AccountName = "Aggregated Account 2",
                        AccountType = AccountTypeEnum.Asset,
                        //BookedDate= {3/19/2012 5:04:01 PM},
                        CreditAmountInCents = 1000,
                        DebitAmountInCents = 0,
                        LedgerId = _id,
                        Memo = "desc",
                        OffsetAccountId = "Aggregated Account 1",
                        OffsetAccountName = "Ledger Account Name 1",
                        Payee = "nord desc",
                        TransactionId = "id1",
                        TransactionImported = true
                    }
                }
            };
            yield return new Ledger_Account_AggregatedBalanceUpdatedEvent
            {
                AccountId = "Aggregated Account 2",
                LedgerId = _id,
                AccountName = "Aggregated Account 2 Name",
                NewBalance = 100,
                OldValueInCents = 1000,
                UserId = UserId,
            };
        }

        [Ignore]
        [Test]
        public void Test()
        {
            Validate("BookedDate","Date");
        }
    }
}