using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;

namespace mPower.Domain.Accounting.Ledger
{
    public class LedgerAR : MpowerAggregateRoot
    {
        public AccountCollection Accounts { get; set; }
        public LedgerUsersCollection Users { get; set; }

        public LedgerAR(string ledgerId, LedgerData data, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);

            Apply(new Ledger_CreatedEvent
            {
                LedgerId = ledgerId,
                Address = data.Address,
                Address2 = data.Address2,
                City = data.City,
                Name = data.Name,
                TypeEnum = data.TypeEnum,
                State = data.State,
                TaxId = data.TaxId,
                Zip = data.Zip,
                FiscalYearStart = data.FiscalYearStart,
                CreatedDate = data.CreatedDate,
            });
        }

        /// <summary>
        /// For object reconstraction
        /// </summary>
        public LedgerAR() { }

        public void Delete()
        {
            Apply(new Ledger_DeletedEvent
            {
                LedgerId = _id
            });
        }

        public void CreateAccount(String accountId, AccountData data)
        {
            if (Accounts.Exists(accountId))
                return;

            Apply(new Ledger_Account_AddedEvent
            {
                LedgerId = _id,
                AccountId = accountId,
                AccountTypeEnum = data.TypeEnum,
                Name = data.Name,
                AccountLabelEnum = data.LabelEnum,
                Imported = data.Imported,
                Aggregated = data.Aggregated,
                ContentServiceId = data.IntuitInstitutionId,
                YodleeItemAccountId = data.IntuitAccountId,
                Number = data.Number,
                Description = data.Description,
                ParentAccountId = data.ParentAccountId,
                InterestRatePerc = data.InterestRatePerc,
                MinMonthPaymentInCents = data.MinMonthPaymentInCents,
                CreditLimitInCents = data.CreditLimitInCents,
                InstitutionName = data.InstitutionName,
                IntuitAccountNumber = data.IntuitAccountNumber,
                IntuitCategoriesNames = data.IntuitCategoriesNames,
            });
        }

        public void UpdateAccount(string accountId, AccountData data)
        {
            Apply(new Ledger_Account_UpdatedEvent
            {
                LedgerId = _id,
                AccountId = accountId,
                Name = data.Name,
                Description = data.Description,
                Number = data.Number,
                ParentAccountId = data.ParentAccountId,
                InstitutionName = data.InstitutionName,
                InterestRatePerc = data.InterestRatePerc,
                MinMonthPaymentInCents = data.MinMonthPaymentInCents,
                CreditLimitInCents = data.CreditLimitInCents,
            });
        }

        public void ArchiveAccount(string accountId, string reason)
        {
            GuardAccountExists(accountId);

            Apply(new Ledger_Account_ArchivedEvent
            {
                LedgerId = _id,
                AccountId = accountId,
                Reason = reason,
            });
        }

        public void RemoveAccount(string accountId, AccountLabelEnum? label)
        {
            GuardAccountExists(accountId);

            Apply(new Ledger_Account_RemovedEvent
            {
                LedgerId = _id,
                AccountId = accountId,
                Label = label
            });
        }

        public void UpdateAccountsOrder(List<AccountOrderData> orders)
        {
            Apply(new Ledger_Account_UpdatedOrderEvent
            {
                LedgerId = _id,
                Orders = orders
            });
        }

        public void SetBudgets(List<BudgetData> budgets)
        {
            Apply(new Ledger_Budget_SetEvent
            {
                LedgerId = _id,
                Budgets = budgets
            });
        }

        public void UpdateBudget(string budgetId, long amount, int month, int year)
        {
            Apply(new Ledger_Budget_UpdatedEvent
            {
                LedgerId = _id,
                Amount = amount,
                BudgetId = budgetId,
                Month = month,
                Year = year
            });
        }

        public void ChangeAccountName(String accountId, String name)
        {
            GuardAccountExists(accountId);

            var acc = Accounts.Get(accountId);
            if (acc.Name == name)
            {
                return;
            }
            Apply(new Ledger_Account_RenamedEvent
            {
                LedgerId = _id,
                AccountId = accountId,
                Name = name
            });
        }

        public void RemoveUser(string userId)
        {
            GuardUserExists(userId);

            Apply(new Ledger_User_RemovedEvent
            {
                LedgerId = _id,
                UserId = userId
            });
        }

        public void AddUser(string userId)
        {
            Apply(new Ledger_User_AddedEvent
            {
                LedgerId = _id,
                UserId = userId
            });
        }

        #region Object Reconstruction

        protected void On(Ledger_CreatedEvent created)
        {
            _id = created.LedgerId;
            Accounts = new AccountCollection();
            Users = new LedgerUsersCollection();
        }

        protected void On(Ledger_Account_AddedEvent added)
        {
            var account = new Account(added.AccountId, added.AccountTypeEnum, added.AccountLabelEnum, added.Name, added.AccountId);
            Accounts.Add(account);
        }

        protected void On(Ledger_Account_RenamedEvent @event)
        {
            Accounts.Get(@event.AccountId).Name = @event.Name;
        }

        protected void On(Ledger_Account_ArchivedEvent removed)
        {
            Accounts.Remove(removed.AccountId);
        }

        protected void On(Ledger_Account_RemovedEvent removed)
        {
            Accounts.Remove(removed.AccountId);
        }

        protected void On(Ledger_User_AddedEvent added)
        {
            Users.Add(added.UserId);
        }

        protected void On(Ledger_User_RemovedEvent removed)
        {
            Accounts.Remove(removed.UserId);
        }

        #endregion

        #region Guards

        private void GuardAccountExists(String accountId)
        {
            if (!Accounts.Exists(accountId))
                throw new InvalidOperationException(String.Format("Account [{0}] doesn't exists in Ledger [{1}]", accountId, _id));
        }

        private void GuardUserExists(String userId)
        {
            if (!Users.Exists(userId))
                throw new InvalidOperationException(String.Format("User [{0}] doesn't exists in Ledger [{1}]", userId, _id));
        }

        #endregion

        public void UpdateAccountAggregatedBalance(BalanceChangedData data)
        {
            // Intuit sends over all accounts even if they have not been added to the ledger
          
            if (Accounts.Exists(data.AccountId))
            {
                Apply(new Ledger_Account_AggregatedBalanceUpdatedEvent
                {
                    UserId = data.UserId,
                    LedgerId = data.LedgerId,
                    AccountId = data.AccountId,
                    AccountName = data.AccountName,
                    OldValueInCents = data.OldValueInCents,
                    NewBalance = data.BalanceInCents,
                    Date = data.Date,
                });
            }
        }

        public void UpdateAccountAggregationStatus(string accountId, long intuitAccountId, AggregatedAccountStatusEnum newStatus, DateTime date, string exceptionId = null)
        {
            Apply(new Ledger_Account_AggregationStatus_UpdatedEvent
            {
                AccountId = accountId,
                IntuitAccountId = intuitAccountId,
                NewStatus = newStatus,
                LedgerId = _id,
                AggregationExceptionId = exceptionId,
                Date = date,
            });
        }

        public void AddToItemTransactionMap(string keyword, string accountId)
        {
            Apply(new Ledger_TransactionMap_ItemAddedEvent
            {
                LedgerId = _id,
                Keyword = keyword,
                AccountId = accountId,
            });
        }

        public void ChangeAccountInterestRate(string accountId, float interestRate)
        {
            Apply(new Ledger_Account_InterestRate_ChangedEvent
                      {
                          LedgerId = _id,
                          AccountId = accountId,
                          InterestRatePerc = interestRate
                      });
        }
    }
}
