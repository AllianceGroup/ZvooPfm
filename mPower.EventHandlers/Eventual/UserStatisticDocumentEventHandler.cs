using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Ledger.Messages;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Domain.Membership.User.Events;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.EventHandlers.Eventual
{
    public class UserStatisticDocumentEventHandler: 
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Entries_AddedMessage>,
        IMessageHandler<Entries_RemovedMessage>
    {
        private readonly UserStatisticDocumentService _userStatisticService;

        public UserStatisticDocumentEventHandler(UserStatisticDocumentService userStatisticService)
        {
            _userStatisticService = userStatisticService;
        }

        public void Handle(User_CreatedEvent message)
        {
            var doc = new UserStatisticDocument
            {
                Id = message.UserId
            };
            _userStatisticService.Save(doc);
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            Update(message.UserId,(doc)=>
            {
                if (message.AccountLabel == AccountLabelEnum.Expense)
                {
                    var value = (message.BalanceInCents - message.OldValueInCents);
                    doc.AddAccountSpentAmount(message.AccountId, message.AccountName, message.Date, value);
                    doc.TotalSpent += value;
                }
            });
        }

        private void Update(string id, Action<UserStatisticDocument> updateAction)
        {
            var doc = _userStatisticService.GetById(id);
            if (doc != null)
            {
                updateAction(doc);
                _userStatisticService.Save(doc);
            }
        }

        public void Handle(Entries_AddedMessage message)
        {
            Update(message.UserId, (doc) => UpdateUserMerchantStatistic(doc, message.Entries, true));
        }

        public void Handle(Entries_RemovedMessage message)
        {
            Update(message.UserId, (doc) => UpdateUserMerchantStatistic(doc, message.Entries, false));
        }

        private void UpdateUserMerchantStatistic(UserStatisticDocument document, IEnumerable<ExpandedEntryData> entries, bool isAdded)
        {
            var expenseEntries = entries.Where(x => x.AccountType == AccountTypeEnum.Expense);
            foreach (var entry in expenseEntries)
            {
                var spentAmount =
                    AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                        entry.DebitAmountInCents, entry.CreditAmountInCents, AccountTypeEnum.Expense)
                        * (isAdded ? 1 : -1);
                if (entry.Payee.HasValue())
                {
                    var merchant = entry.Payee.Trim();
                    document.AddMerchantSpentAmount(merchant, entry.BookedDate, spentAmount);
                }
            }
        }
    }
}