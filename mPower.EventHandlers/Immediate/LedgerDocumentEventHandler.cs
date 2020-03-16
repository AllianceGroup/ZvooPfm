using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Ledger.Messages;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Framework;
using Paralect.ServiceBus;
using mPower.Signals;

namespace mPower.EventHandlers.Immediate
{
    public class LedgerDocumentEventHandler :
        IMessageHandler<Ledger_CreatedEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Ledger_Account_AddedEvent>,
        IMessageHandler<Ledger_Account_ArchivedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>,
        IMessageHandler<Ledger_User_AddedEvent>,
        IMessageHandler<Ledger_User_RemovedEvent>,
        IMessageHandler<Ledger_Account_UpdatedOrderEvent>,
        IMessageHandler<Ledger_Budget_SetEvent>,
        IMessageHandler<Ledger_Account_AggregatedBalanceUpdatedEvent>,
        IMessageHandler<Ledger_TransactionMap_ItemAddedEvent>,
        IMessageHandler<Transaction_CreateMultipleMessage>,
        IMessageHandler<Ledger_Account_AggregationStatus_UpdatedEvent>,
        IMessageHandler<Ledger_Account_InterestRate_ChangedEvent>,
        IMessageHandler<Ledger_Account_RenamedEvent>,
        IMessageHandler<Ledger_Account_DateLastAggregated_ChangedEvent>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LedgerDocumentEventHandler(LedgerDocumentService ledgerService, IEventService eventService)
        {
            _ledgerService = ledgerService;
            _eventService = eventService;
        }

        public void Handle(Ledger_CreatedEvent message)
        {
            var doc = new LedgerDocument
            {
                Id = message.LedgerId,
                Address = message.Address,
                Address2 = message.Address2,
                City = message.City,
                Name = message.Name,
                TypeEnum = message.TypeEnum,
                State = message.State,
                TaxId = message.TaxId,
                Zip = message.Zip,
                FiscalYearStart = message.FiscalYearStart,
                CreatedDate = message.CreatedDate,
                KeywordMap = new List<KeywordMapDocument>()
            };

            _ledgerService.Save(doc);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            _ledgerService.RemoveById(message.LedgerId);
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            _ledgerService.SetAccountBalance(message.LedgerId, message.AccountId, message.BalanceInCents);
        }

        public void Handle(Ledger_Account_AddedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);

            var order = ledger.Accounts.Count > 0 ? ledger.Accounts.Max(x => x.Order) + 1 : 0; // place new account at top of accounts

            var account = new AccountDocument
            {
                Denormalized = {Balance = 0},
                Description = message.Description,
                Id = message.AccountId,
                Name = message.Name,
                Number = message.Number,
                TypeEnum = message.AccountTypeEnum,
                LabelEnum = message.AccountLabelEnum,
                Aggregated = message.Aggregated,
                IntuitInstitutionId = message.ContentServiceId,
                IntuitAccountId = string.IsNullOrEmpty(message.YodleeItemAccountId) ? null : new long?(long.Parse(message.YodleeItemAccountId)),
                Imported = message.Imported,
                ParentAccountId = message.ParentAccountId,
                InterestRatePerc = message.InterestRatePerc,
                MinMonthPaymentInCents = message.MinMonthPaymentInCents,
                CreditLimitInCents = message.CreditLimitInCents,
                Order = order,
                Created = message.Metadata.StoredDate == DateTime.MinValue ? DateTime.Now : message.Metadata.StoredDate,
                InstitutionName = message.InstitutionName,
                IntuitAccountNumber = message.IntuitAccountNumber,
                IntuitCategoriesNames = message.IntuitCategoriesNames,
                DateLastAggregated = DateTime.Now,
            };

            
            _ledgerService.AddAccount(message.LedgerId, account);

            // fix for old events
            if (account.IsAggregated && message.AggregatedBalanceInCents != 0)
            {
                _eventService.Send(new Ledger_Account_AggregatedBalanceUpdatedEvent
                {
                    UserId = ledger.Users.First().Id,
                    LedgerId = ledger.Id,
                    AccountId = account.Id,
                    AccountName = account.Name,
                    OldValueInCents = 0,
                    NewBalance = message.AggregatedBalanceInCents,
                    Date = account.DateLastAggregated,
                });
            }

            _eventService.Send(new AccountAddedSignal {UserId = ledger.Users.First().Id });
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            var account = ledger.Accounts.Single(x => x.Id == message.AccountId);

            _ledgerService.RemoveAccount(message.LedgerId,message.AccountId);

            _eventService.Send(new Ledger_Account_RemovedMessage
            {
                UserId = ledger.Users.Single().Id,
                AccountId = message.AccountId,
                AccountName = account.Name,
                LedgerId = message.LedgerId,
                LabelEnum = account.LabelEnum,
                Balance = account.ActualBalance,
                CreditLimitInCents = account.CreditLimitInCents,
                IntuitAccountId = account.IntuitAccountId,
                Date = message.Metadata.StoredDate,
                IsAggregated = account.IsAggregated,
            });
        }

        public void Handle(Ledger_Account_ArchivedEvent message)
        {    
            _ledgerService.ArchiveAccount(message.LedgerId, message.AccountId, message.Reason);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            _ledgerService.UpdateAccount(message);
        }

        public void Handle(Ledger_User_AddedEvent message)
        {    
            var userDocument = new LedgerUserDocument { Id = message.UserId };       
            _ledgerService.AddUser(message.LedgerId, userDocument);
        }

        public void Handle(Ledger_User_RemovedEvent message)
        {
            _ledgerService.RemoveUser(message.LedgerId, message.UserId);
        }

        public void Handle(Ledger_Account_UpdatedOrderEvent message)
        {
            _ledgerService.UpdateAccountsOrder(message.LedgerId,message.Orders);
        }

        public void Handle(Ledger_Budget_SetEvent message)
        {           
            _ledgerService.SetBudget(message.LedgerId, message.Budgets);
        }

        public void Handle(Ledger_TransactionMap_ItemAddedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger.KeywordMap == null)
                ledger.KeywordMap = new List<KeywordMapDocument>();
            else
            {
                var existingKeywordMap = ledger.KeywordMap.SingleOrDefault(x => x.Keyword == message.Keyword);
                if (existingKeywordMap != null)
                    ledger.KeywordMap.Remove(existingKeywordMap);
            }
            ledger.KeywordMap.Add(new KeywordMapDocument
            {
                Keyword = message.Keyword,
                AccountId = message.AccountId
            });
            _ledgerService.Save(ledger);
        }

        public void Handle(Transaction_CreateMultipleMessage message)
        {
             var distinctAggregatedAccountIds = new List<KeyValuePair<string, string>>();
            foreach (var dto in message.Transactions)
            { 
                if (distinctAggregatedAccountIds.Count(x => x.Key == dto.BaseAccountId && x.Value == dto.LedgerId) == 0)
                {
                    distinctAggregatedAccountIds.Add(new KeyValuePair<string, string>(dto.BaseAccountId, dto.LedgerId));

                    Handle(new Ledger_Account_AggregationStatus_UpdatedEvent
                    {
                        AccountId = dto.BaseAccountId,
                        LedgerId = dto.LedgerId,
                        NewStatus = AggregatedAccountStatusEnum.Normal,
                        Date = message.Date,
                    });
                }
            }
        }

        public void Handle(Ledger_Account_AggregationStatus_UpdatedEvent message)
        {
            _ledgerService.SetAccountAggregationStatus(message);
        }

        public void Handle(Ledger_Account_AggregatedBalanceUpdatedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            AccountDocument account = null;
            if (ledger != null)
            {
                account = ledger.Accounts.Find(x => x.Id == message.AccountId);
            }
            _ledgerService.SetAggregatedBalance(message.LedgerId, message.AccountId, message.NewBalance);
            if (account != null)
            {
                _eventService.Send(new Ledger_Account_AggregatedBalanceUpdatedMessage
                {
                    UserId = message.UserId,
                    LedgerId = message.LedgerId,
                    AccountId = message.AccountId,
                    AccountName = account.Name,
                    LabelEnum = account.LabelEnum,
                    OldValueInCents = account.AggregatedBalance,
                    NewBalance = message.NewBalance,
                    Date = message.Date,
                    CreditLimitInCents = account.CreditLimitInCents,
                });
            }
        }

        public void Handle(Ledger_Account_RenamedEvent message)
        {
            _ledgerService.ChangeAccountName(message.LedgerId, message.AccountId, message.Name);
        }

        public void Handle(Ledger_Account_InterestRate_ChangedEvent message)
        {
            _ledgerService.ChangeInterestRate(message.LedgerId, message.AccountId, message.InterestRatePerc);
        }

        public void Handle(Ledger_Account_DateLastAggregated_ChangedEvent message)
        {
            _ledgerService.SetAggregatedDate(message.LedgerId, message.AccountId, message.DateLastAggregated);
        }
    }
}
