using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Utils;

namespace Default.Factories.Commands
{
    public class Budget_SetCommandFactory : IObjectFactory<BudgetsListDto, Ledger_Budget_SetCommand>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IIdGenerator _idGenerator;
        private readonly BusinessReportDocumentService _businessReportService;

        public Budget_SetCommandFactory(LedgerDocumentService ledgerService, IIdGenerator idGenerator, BusinessReportDocumentService businessReportService)
        {
            _ledgerService = ledgerService;
            _idGenerator = idGenerator;
            _businessReportService = businessReportService;
        }

        public Ledger_Budget_SetCommand Load(BudgetsListDto args)
        {
            var budgetItems = new List<BudgetData>();

            var ledger = _ledgerService.GetById(args.LedgerId);
            var amountsSpent = _businessReportService.GetProfitLossReportData(DateUtil.GetStartOfMonth(args.Month, args.Year), DateUtil.GetEndOfMonth(args.Month, args.Year), ledger);
            var accounts = ledger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Expense || x.TypeEnum == AccountTypeEnum.Income).ToList();

            foreach (var account in accounts)
            {
                var budget = args.Budgets.FirstOrDefault(x => x.IncludeBudget && x.Id == account.Id);
                if (budget != null)
                {
                    var budgetAmount = 0m;
                    Decimal.TryParse(budget.Budget, out budgetAmount);
                    var parentBudget = MapBudget(args.Month, args.Year, CalculateBalance(amountsSpent, account.Id), account, AccountingFormatter.DollarsToCents(budgetAmount));
                    budgetItems.Add(parentBudget);
                    var subAccounts = accounts.Where(x => x.ParentAccountId == account.Id).ToList();
                    if (subAccounts.Count > 0)
                    {
                        foreach (var sub in subAccounts)
                        {
                            var subAccount = MapBudget(args.Month, args.Year, CalculateBalance(amountsSpent, sub.Id), sub, 0);
                            parentBudget.SubBudgets.Add(subAccount);
                        }
                    }
                }
            }

            var command = new Ledger_Budget_SetCommand
            {
                LedgerId = ledger.Id,
                Budgets = budgetItems
            };

            return command;
        }

        private static long CalculateBalance(IEnumerable<LedgerAccountBalanceByDay> amounts, string accountId)
        {
            long balance = 0;

            foreach (var item in amounts)
            {
                if (item.AccountId == accountId)
                {
                    balance += item.AmountPerDay.Sum(x => x.Amount);
                }

                balance += item.SubAccounts
                    .Where(subAccount => subAccount.AccountId == accountId)
                    .Sum(subAccount => subAccount.AmountPerDay.Sum(x => x.Amount));
            }

            return balance;
        }

        private BudgetData MapBudget(int month, int year, long spentAmount, AccountDocument account, long budget)
        {
            var parentBudget = new BudgetData
            {
                Id = _idGenerator.Generate(),
                AccountName = account.Name,
                AccountType = account.TypeEnum,
                BudgetAmount = budget,
                AccountId = account.Id,
                ParentId = account.ParentAccountId,
                Month = month,
                Year = year,
                SpentAmount = spentAmount
            };
            return parentBudget;
        }
    }
}