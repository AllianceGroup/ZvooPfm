using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework.Utils;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger
{
    public abstract class LedgerTest : AggregateTest<LedgerAR>
    {
        private readonly DateTime _currentDate = DateTime.Now;

        public DateTime CurrentDate
        {
            get { return _currentDate; }
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

        public IEvent Ledger_Deleted()
        {
            return new Ledger_DeletedEvent
            {
                LedgerId = _id,
            };
        }

        public IEvent Ledger_Account_Added(String id, AccountTypeEnum typeEnum, AccountLabelEnum labelEnum)
        {
            return new Ledger_Account_AddedEvent
            {
                AccountId = id,
                AccountTypeEnum = typeEnum,
                AccountLabelEnum = labelEnum,
                LedgerId = _id,
                Name = id
            };
        }

        public IEvent Ledger_Account_BalanceChanged(String accountId, Int64 balance)
        {
            return new Ledger_Account_BalanceChangedEvent
            {
                AccountId = accountId,
                LedgerId = _id,
                BalanceInCents = balance
            };
        }

        public IEvent Ledger_Account_Archived(String id, String reason)
        {
            return new Ledger_Account_ArchivedEvent
            {
                AccountId = id,
                LedgerId = _id,
                Reason = reason,
            };
        }

        public IEvent Ledger_Account_Removed(String id)
        {
            return new Ledger_Account_RemovedEvent
            {
                AccountId = id,
                LedgerId = _id,
            };
        }

        public IEvent Ledger_Budget_Created(List<BudgetData> budgets)
        {
            return new Ledger_Budget_SetEvent
            {
                LedgerId = _id,
                Budgets = budgets,
            };
        }

        public IEvent Ledger_Budget_Updated(string budgetId, int year, int month, long newAmountInCents)
        {
            return new Ledger_Budget_UpdatedEvent
            {
                LedgerId = _id,
                BudgetId = budgetId,
                Amount = newAmountInCents,
                Year = year,
                Month = month,
            };
        }

        #endregion

        #region Commands

        public ICommand Ledger_Delete()
        {
            return new Ledger_DeleteCommand
            {
                LedgerId = _id,
            };
        }

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

        public ICommand Ledger_Account_Archive(String accountId, String reason)
        {
            return new Ledger_Account_ArchiveCommand
            {
                AccountId = accountId,
                LedgerId = _id,
                Reason = reason,
            };
        }

        public ICommand Ledger_Account_Remove(String accountId)
        {
            return new Ledger_Account_RemoveCommand
            {
                AccountId = accountId,
                LedgerId = _id,
            };
        }

        public ICommand Ledger_Budget_Create(List<BudgetData> budgets)
        {
            return new Ledger_Budget_SetCommand
            {
                LedgerId = _id,
                Budgets = budgets,
            };
        }

        public ICommand Ledger_Budget_Update(string budgetId, int year, int month, long newAmountInCents)
        {
            return new Ledger_Budget_UpdateCommand
            {
                LedgerId = _id,
                BudgetId = budgetId,
                AmountInCents = newAmountInCents,
                Year = year,
                Month = month,
            };
        }

        #endregion

    }
}