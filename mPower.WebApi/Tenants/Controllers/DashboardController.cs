using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.WebApi.Tenants.Model.Dashboard;
using mPower.WebApi.Tenants.Services;
using mPower.WebApi.Tenants.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class DashboardController : BaseController
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly TransactionsStatisticDocumentService _statisticService;
        private readonly BudgetDocumentService _budgetService;
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly DebtViewModelBuilder _debtBuilder;

        public DashboardController(LedgerDocumentService ledgerService, TransactionsStatisticDocumentService statisticService, 
            BudgetDocumentService budgetService, DebtEliminationDocumentService debtEliminationService, DebtViewModelBuilder debtBuilder,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _ledgerService = ledgerService;
            _statisticService = statisticService;
            _budgetService = budgetService;
            _debtEliminationService = debtEliminationService;
            _debtBuilder = debtBuilder;
        }

        [HttpGet("GetCharts")]
        public ChartsViewModel GetCharts()
        {
            var model = new ChartsViewModel
            {
                AccountsChart = BuildAccountsChart(),
                BudgetsChart = BuildBudgetsChart()
            };

            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());

            if (debt == null) return model;
            var mortgageProgramId = debt.CurrentMortgageProgramId ??
                                    (debt.MortgagePrograms.Count > 0 ? debt.MortgagePrograms.First().Id : null);

            model.LeftChart = _debtBuilder.GetDebtToIncomeRatioModel(debt, true).LeftChart;
            model.RightChart = _debtBuilder.GetDebtToIncomeRatioModel(debt, true).RightChart;
            model.DebtToIncomeRatio = _debtBuilder.GetDebtToIncomeRatioModel(debt, true).DebtToIncomeRatio;

            if (mortgageProgramId == null) return model;
            model.MortageAccelerator = _debtBuilder.GetMortgageProgramModel(debt, mortgageProgramId, true).Details.Chart.Data;
            model.TotalSavingsInCents =
                _debtBuilder.GetMortgageProgramModel(debt, mortgageProgramId, true)
                    .AcceleratedParams.TotalSavingsInCents;

            return model;
        }

        #region private part
        private List<ChartItem> BuildAccountsChart()
        {
            var allowedAccountLabels = new List<AccountLabelEnum>
            {
                AccountLabelEnum.CreditCard,
                AccountLabelEnum.Loan,
                AccountLabelEnum.Bank,
                AccountLabelEnum.Investment
            };

            var ledger = _ledgerService.GetById(GetLedgerId());

            if (ledger != null)
            {
                // get data
                var filter = new TransactionsStatisticFilter
                {
                    LedgerId = ledger.Id,
                    AccountIds = ledger.Accounts.Select(a => a.Id).ToList()
                };
                var accountsStatistic = _statisticService.GetByFilter(filter);

                // group it
                var accountsStatisticGrouped = accountsStatistic.GroupBy(x => x.AccountId).Select(g => new
                {
                    AccountId = g.Key,
                    AmountInCents = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                        g.Sum(x => x.DebitAmountInCents),
                        g.Sum(x => x.CreditAmountInCents),
                        g.First().AccountType),
                });

                // convert to AccountsAndBudgetsChartModel
                var model = new List<ChartItem>();
                foreach (var statistic in accountsStatisticGrouped)
                {
                    var accountInfo = ledger.Accounts.Find(a => a.Id == statistic.AccountId);
                    if (allowedAccountLabels.Contains(accountInfo.LabelEnum))
                    {
                        model.Add(
                            new ChartItem((double) AccountingFormatter.CentsToDollars(statistic.AmountInCents), accountInfo.Name));
                    }
                }

                return model;
            }

            return null;
        }

        private BudgetsChartModel BuildBudgetsChart()
        {
            var date = DateTime.UtcNow;
            var budgetsInfo = _budgetService.GetHalfYearToDateBudgets(GetLedgerId(), date)
                                            .Where(x => x.AccountType == AccountTypeEnum.Expense)
                                            .GroupBy(x => x.Month).ToDictionary(g => g.Key, g => new
                                            {
                                                Spent = g.Sum(x => x.SpentAmountWithSubAccounts),
                                                Budget = g.Sum(x => x.BudgetAmount),
                                            });

            var spent = new List<ChartItem>();
            var budget = new List<ChartItem>();

            for(var i = 1; i <= date.Month; i++)
            {
                var label = string.Format("{0:yyyy-MM}", new DateTime(date.Year, i, 1));
                if(budgetsInfo.ContainsKey(i))
                {
                    var info = budgetsInfo[i];
                    spent.Add(new ChartItem ((double) AccountingFormatter.CentsToDollars(info.Spent), label));
                    budget.Add(new ChartItem ((double)AccountingFormatter.CentsToDollars(info.Budget), label));
                }
                else if(budget.Count > 0) // skip first months without setuped budgets
                {
                    spent.Add(new ChartItem(0, label));
                    budget.Add(new ChartItem(0, label));
                }
            }
            return new BudgetsChartModel {Spent = spent, Budget = budget};
        }
    }
    #endregion
}
