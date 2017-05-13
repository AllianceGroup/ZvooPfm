using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Segments;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Domain.Accounting.DebtElimination.Events;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Goal.Events;
using mPower.Domain.Accounting.Goal.Messages;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Ledger.Messages;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Domain.Membership.User.Events;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Messages;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.EventHandlers.Eventual.Segments
{
    public class UserSegmentEventHandler :
        IMessageHandler<User_CreatedMessage>,
        IMessageHandler<User_LoggedInEvent>,
        IMessageHandler<User_DeletedMessage>,
        IMessageHandler<User_ActivatedEvent>,
        IMessageHandler<User_DeactivatedEvent>,
        IMessageHandler<DebtElimination_EliminationPlanUpdatedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_AddedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_UpdatedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_DeletedEvent>,
        IMessageHandler<CreditIdentity_Report_AddedEvent>,
        IMessageHandler<Ledger_Account_AddedEvent>,
        IMessageHandler<Ledger_Account_RemovedMessage>,
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Ledger_Account_AggregatedBalanceUpdatedMessage>,
        IMessageHandler<Ledger_Budget_SetEvent>,
        IMessageHandler<Ledger_Budget_ExceededEvent>,
        IMessageHandler<Entries_AddedMessage>,
        IMessageHandler<Entries_RemovedMessage>,
        IMessageHandler<User_Realestate_AddedEvent>,
        IMessageHandler<User_Realestate_DeletedEvent>,
        IMessageHandler<Goal_CreatedEvent>,
        IMessageHandler<Goal_DeletedMessage>,
        IMessageHandler<CreditIdentity_CreatedEvent>,
        IMessageHandler<CreditIdentity_DeletedEvent>
    {
        private readonly SegmentAggregationService _segmentAggregationService;
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly LedgerDocumentService _ledgerService;

        public UserSegmentEventHandler(SegmentAggregationService segmentAggregationService,
            DebtEliminationDocumentService debtEliminationService,
            LedgerDocumentService ledgerService)
        {
            _segmentAggregationService = segmentAggregationService;
            _debtEliminationService = debtEliminationService;
            _ledgerService = ledgerService;
        }

        public void Handle(User_CreatedMessage message)
        {
            // obligatorily specify Affiliate Info to prevent SegmentAggregationService from retrieving current user, which may not exists yet
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.CreateDate, message.UserId,
                Update.Inc("AggregateData.UserAddedCounter", 1), message.AffiliateId, message.AffiliateName);

            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.CreateDate, message.UserId,
                   Update<UserSegment>.Set(x => x.AggregateData.IsActive, message.IsActive));
        }

        public void Handle(User_LoggedInEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Day, message.Date, message.UserId,
                Update<UserSegment>.Inc(x=> x.AggregateData.Logins, 1).Set(x => x.LastLoginDate, message.Date));

        }

        public void Handle(User_DeletedMessage message)
        {
            // obligatorily specify Affiliate Info to prevent SegmentAggregationService from retrieving current user, which may not exists already
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update.Inc("AggregateData.UserAddedCounter", -1), message.AffiliateId, message.AffiliateName);
        }

        public void Handle(DebtElimination_EliminationPlanUpdatedEvent message)
        {
            var programm = _debtEliminationService.GetById(message.Id);
            if (programm != null)
            {
                _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, programm.UserId,
                    Update<UserSegment>.Set(x => x.AggregateData.HasDeptEliminationProgramm, true));
            }
        }

        public void Handle(DebtElimination_MortgageProgram_AddedEvent message)
        {
            var programm = _debtEliminationService.GetById(message.DebtEliminationId);

            if (programm != null)
            {
                var data = new MortgageData
                {
                    Id = message.Id,
                    AmountInCents = message.LoanAmountInCents,
                    InterestRate = message.InterestRatePerYear,
                };

                _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, programm.UserId,
                    Update.Push("AggregateData.Mortgages", data.ToBsonDocument()));
            }
        }

        public void Handle(DebtElimination_MortgageProgram_UpdatedEvent message)
        {
            var programm = _debtEliminationService.GetById(message.DebtEliminationId);

            if (programm != null)
            {
                var mortgageData = new MortgageData
                {
                    Id = message.Id,
                    AmountInCents = message.LoanAmountInCents,
                    InterestRate = message.InterestRatePerYear,
                };

                _segmentAggregationService.UpdateMortgage(
                    UserSegmentTypeEnum.Global,
                    message.Metadata.StoredDate,
                    programm.UserId,
                    mortgageData);
            }
        }

        public void Handle(DebtElimination_MortgageProgram_DeletedEvent message)
        {
            var programm = _debtEliminationService.GetById(message.DebtEliminationId);

            if (programm != null)
            {
                _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, programm.UserId,
                    Update.Push("AggregateData.RemovedMortgagesIds", message.Id));
            }
        }

        public void Handle(CreditIdentity_Report_AddedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update<UserSegment>.Set(x => x.AggregateData.PulledCreditScore, true));
        }

        public void Handle(Ledger_Account_AddedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                var update = new UpdateBuilder();
                var counterName = String.Empty;

                switch (message.AccountLabelEnum)
                {
                    case AccountLabelEnum.Bank:
                        counterName = "Banks";
                        break;
                    case AccountLabelEnum.CreditCard:
                        if (message.CreditLimitInCents != 0)
                        {
                            update = update.Inc("AggregateData.AvailableCreditInCents", message.CreditLimitInCents);
                        }
                        counterName = "CreditCards";
                        break;
                    case AccountLabelEnum.Investment:
                        counterName = "Investments";
                        break;
                    case AccountLabelEnum.Loan:
                        counterName = "Loans";
                        break;
                }

                if (!String.IsNullOrEmpty(counterName) && !BaseAccounts.All().Contains(message.AccountId))
                {
                    foreach (var user in ledger.Users)
                    {
                        update = update.Inc("AggregateData." + counterName, 1);
                        if (message.Aggregated)
                            update = update.Inc("AggregateData.AggregatedAccounts", 1);

                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, user.Id, update);
                    }
                }

                CheckMortgageCreated(message);
            }
        }

        public void Handle(Ledger_Account_RemovedMessage message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                var counterName = String.Empty;
                var update = new UpdateBuilder();

                switch (message.LabelEnum)
                {
                    case AccountLabelEnum.Bank:
                        counterName = "Banks";
                        break;
                    case AccountLabelEnum.CreditCard:
                        if (message.CreditLimitInCents > message.Balance)
                        {
                            update = update.Inc("AggregateData.AvailableCreditInCents", (Math.Min(message.Balance, message.CreditLimitInCents) - message.CreditLimitInCents));
                        }
                        counterName = "CreditCards";
                        break;
                    case AccountLabelEnum.Investment:
                        counterName = "Investments";
                        break;
                    case AccountLabelEnum.Loan:
                        counterName = "Loans";
                        break;
                }

                if (!String.IsNullOrEmpty(counterName) && !BaseAccounts.All().Contains(message.AccountId))
                {
                    update = update.Inc("AggregateData." + counterName, -1);
                    foreach (var user in ledger.Users)
                    {
                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Date, user.Id, update);
                    }
                }

                if (message.IsAggregated)
                {
                    HandleBalanceChange(new BalanceChangedDto
                    {
                        LedgerId = message.LedgerId,
                        AccountId = message.AccountId,
                        AccountName = message.AccountName,
                        LabelEnum = message.LabelEnum,
                        OldValueInCents = message.Balance,
                        BalanceInCents = 0,
                        Date = message.Date,
                        CreditLimitInCents = message.CreditLimitInCents,
                    });
                }

                CheckMortgageRemoved(message);
            }
        }

        public void Handle(User_Realestate_AddedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update.Inc("AggregateData.Zillows", 1));
        }

        public void Handle(User_Realestate_DeletedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update.Inc("AggregateData.Zillows", -1));
        }

        public void Handle(Goal_CreatedEvent message)
        {
            var update = Update.Inc("AggregateData.Goals", 1);

            switch (message.Type)
            {
                case GoalTypeEnum.Emergency:
                    update.Inc("AggregateData.EmergencyFundsCount", 1);
                    break;
                case GoalTypeEnum.Retirement:
                    update.Inc("AggregateData.RetirementPlansCount", 1);
                    break;
            }

            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.StartDate, message.UserId, update);
        }

        public void Handle(Goal_DeletedMessage message)
        {
            var update = Update.Inc("AggregateData.Goals", -1);

            switch (message.Type)
            {
                case GoalTypeEnum.Emergency:
                    update.Inc("AggregateData.EmergencyFundsCount", -1);
                    break;
                case GoalTypeEnum.Retirement:
                    update.Inc("AggregateData.RetirementPlansCount", -1);
                    break;
            }

            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.StartDate, message.UserId, update);
        }

        public void Handle(CreditIdentity_CreatedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update.Inc("AggregateData.AuthenticatedCreditIdentitiesCount", 1));
        }

        public void Handle(CreditIdentity_DeletedEvent message)
        {
            if (!String.IsNullOrEmpty(message.Metadata.UserId))
                _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.Metadata.UserId,
                    Update.Inc("AggregateData.AuthenticatedCreditIdentitiesCount", -1));
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                var account = ledger.Accounts.Find(x => x.Id == message.AccountId);
                if (account != null && !account.IsAggregated)
                {
                    var data = new BalanceChangedDto
                    {
                        LedgerId = message.LedgerId,
                        AccountId = message.AccountId,
                        AccountName = message.AccountName,
                        LabelEnum = account.LabelEnum,
                        OldValueInCents = message.OldValueInCents,
                        BalanceInCents = message.BalanceInCents,
                        Date = message.Date,
                        CreditLimitInCents = account.CreditLimitInCents,
                    };
                    HandleBalanceChange(data);
                    CheckMortgageBalanceSet(data);
                }
            }
        }

        public void Handle(Ledger_Account_AggregatedBalanceUpdatedMessage message)
        {
            var data = new BalanceChangedDto
            {
                LedgerId = message.LedgerId,
                AccountId = message.AccountId,
                AccountName = message.AccountName,
                LabelEnum = message.LabelEnum,
                OldValueInCents = message.OldValueInCents,
                BalanceInCents = message.NewBalance,
                Date = message.Date,
                CreditLimitInCents = message.CreditLimitInCents,
            };
            HandleBalanceChange(data);
            CheckMortgageBalanceSet(data);
        }

        public void Handle(Ledger_Budget_SetEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                foreach (var user in ledger.Users)
                {
                    _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, user.Id,
                        Update<UserSegment>.Set(x => x.AggregateData.SetupBudget, true));
                    var accountsNames = message.Budgets.Select(x => x.AccountName).ToList();
                    foreach (var budget in message.Budgets)
                    {
                        accountsNames.AddRange(budget.SubBudgets.Select(x => x.AccountName));
                    }
                    _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Month, message.Metadata.StoredDate, user.Id,
                        Update.PushAll("AggregateData.AccountsWithoutExceededBudget", new BsonArray(accountsNames)));
                }
            }
        }

        public void Handle(Ledger_Budget_ExceededEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                foreach (var user in ledger.Users)
                {
                    _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Month, message.Date, user.Id,
                        Update.Pull("AggregateData.AccountsWithoutExceededBudget", message.AccountName)
                            .Push("AggregateData.AccountsWithExceededBudget", message.AccountName));
                }
            }
        }

        public void Handle(Entries_AddedMessage message)
        {
            HandleEntiesListChange(message.Entries, true);
        }

        public void Handle(Entries_RemovedMessage message)
        {
            HandleEntiesListChange(message.Entries, false);
        }

        private void HandleBalanceChange(BalanceChangedDto data)
        {
            UpdateBuilder globalUpdate = null;
            UpdateBuilder monthUpdate = null;

            var ledger = _ledgerService.GetById(data.LedgerId);
            if (ledger != null)
            {
                var balanceIncrement = data.BalanceInCents - data.OldValueInCents;
                switch (data.LabelEnum)
                {
                    case AccountLabelEnum.Investment:
                        globalUpdate = Update.Inc("AggregateData.InvestmentAccountsBalanceInCents", balanceIncrement);
                        break;
                    case AccountLabelEnum.Bank:
                        globalUpdate = Update.Inc("AggregateData.AvailableCashInCents", balanceIncrement);
                        break;
                    case AccountLabelEnum.CreditCard:
                        globalUpdate = Update.Inc("AggregateData.TotalDebtInCents", balanceIncrement)
                            .Inc("AggregateData.CreditCardsDebtInCents", balanceIncrement);
                        monthUpdate = Update.Inc("AggregateData.MonthlyDebtInCents", balanceIncrement);

                        var availableCreditIncrement = Math.Min(data.CreditLimitInCents, data.OldValueInCents) -
                                                        Math.Min(data.CreditLimitInCents, data.BalanceInCents);
                        if (availableCreditIncrement != 0)
                        {
                            globalUpdate = globalUpdate.Inc("AggregateData.AvailableCreditInCents", availableCreditIncrement);
                        }
                        break;
                    case AccountLabelEnum.Loan:
                        globalUpdate = Update.Inc("AggregateData.TotalDebtInCents", balanceIncrement);
                        monthUpdate = Update.Inc("AggregateData.MonthlyDebtInCents", balanceIncrement);
                        break;
                }

                if (globalUpdate != null)
                {
                    foreach (var user in ledger.Users)
                    {
                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, data.Date, user.Id, globalUpdate);
                    }
                }
                if (monthUpdate != null)
                {
                    foreach (var user in ledger.Users)
                    {
                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Month, data.Date, user.Id, monthUpdate);
                    }
                }
            }
        }

        private void HandleEntiesListChange(List<ExpandedEntryData> entries, bool isAdding)
        {
            var firstEntry = entries.First();
            var ledger = _ledgerService.GetById(firstEntry.LedgerId);

            if (ledger != null)
            {
                var incomeEntries = entries.Where(x => x.AccountType == AccountTypeEnum.Income).ToList();
                if (incomeEntries.Count > 0)
                {
                    var incomeInCents = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                        incomeEntries.Sum(x => x.DebitAmountInCents), incomeEntries.Sum(x => x.CreditAmountInCents),
                        AccountTypeEnum.Income);
                    if (incomeInCents != 0)
                    {
                        foreach (var user in ledger.Users)
                        {
                            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Month, firstEntry.BookedDate, user.Id,
                                Update.Inc("AggregateData.MonthlyIncomeInCents", incomeInCents * (isAdding ? 1 : -1)));
                        }
                    }
                }

                var expenseEntries = entries.Where(x => x.AccountType == AccountTypeEnum.Expense).ToList();
                foreach (var entry in expenseEntries)
                {
                    var spentAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(entry.DebitAmountInCents, entry.CreditAmountInCents, AccountTypeEnum.Expense) * (isAdding ? 1 : -1);
                    foreach (var user in ledger.Users)
                    {
                        var merchant = string.IsNullOrEmpty(entry.Payee) ? "" : entry.Payee.Trim();
                        _segmentAggregationService.UpdateExpenseSegment(firstEntry.BookedDate, user.Id,
                            Update.Inc("AggregateData.SpentAmountInCents", spentAmount)
                            .Inc("AggregateData.ShopVisitsNumber", isAdding ? 1 : -1), merchant, entry.AccountName);
                    }
                }
            }
        }

        #region Recognize mortgages

        private static string GetMortgageId(string accountId)
        {
            return "acc_" + accountId;
        }

        private static bool IsMortgageAccount(AccountLabelEnum label, string name)
        {
            return label == AccountLabelEnum.Loan && name.IndexOf("Mortgage", StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        private void CheckMortgageCreated(Ledger_Account_AddedEvent message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                if (IsMortgageAccount(message.AccountLabelEnum, message.Name))
                {
                    var data = new MortgageData
                    {
                        Id = GetMortgageId(message.AccountId),
                        AmountInCents = 0,
                        InterestRate = message.InterestRatePerc,
                    };

                    foreach (var user in ledger.Users)
                    {
                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, user.Id,
                            Update.Push("AggregateData.Mortgages", data.ToBsonDocument()));
                    }
                }
            }
        }

        private void CheckMortgageBalanceSet(BalanceChangedDto data)
        {
            if (IsMortgageAccount(data.LabelEnum, data.AccountName))
            {
                _segmentAggregationService.UpdateSegments(
                    Query.EQ("AggregateData.Mortgages.Id", GetMortgageId(data.AccountId)),
                    Update.Set("AggregateData.Mortgages.$.AmountInCents", data.BalanceInCents));
            }
        }

        private void CheckMortgageRemoved(Ledger_Account_RemovedMessage message)
        {
            var ledger = _ledgerService.GetById(message.LedgerId);
            if (ledger != null)
            {
                if (IsMortgageAccount(message.LabelEnum, message.AccountName))
                {
                    foreach (var user in ledger.Users)
                    {
                        _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, user.Id,
                            Update.Push("AggregateData.RemovedMortgagesIds", GetMortgageId(message.AccountId)));
                    }
                }
            }
        }

        #endregion

        public void Handle(User_ActivatedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                   Update<UserSegment>.Set(x => x.AggregateData.IsActive, true));
        }

        public void Handle(User_DeactivatedEvent message)
        {
            _segmentAggregationService.UpdateSegment(UserSegmentTypeEnum.Global, message.Metadata.StoredDate, message.UserId,
                Update<UserSegment>.Set(x => x.AggregateData.IsActive, false));
        }
    }
}
