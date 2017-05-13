using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Default.Models;
using Default.ViewModel.Areas.Finance.TrendsController;
using FusionChartsMVC.FusionCharts;
using FusionChartsMVC.FusionCharts.Charts;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Mvc.Helpers;

namespace Default.Factories.ViewModels
{
    public class TrendsViewModelFactory : 
        IObjectFactory<TrendsViewModelFilter,TrendsModel>,
        IObjectFactory<TransactionsStatisticFilter, List<TrendCategoryItem>>
    {
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly TransactionsStatisticDocumentService _statisticService;

        public TrendsViewModelFactory(LedgerDocumentService ledgerDocumentService, TransactionsStatisticDocumentService statisticService)
        {
            _ledgerDocumentService = ledgerDocumentService;
            _statisticService = statisticService;
        }

        public TrendsModel Load(TrendsViewModelFilter input)
        {
            var model = new TrendsModel();
            model.Monthes = BuildMonthesModel();
            model.TrendFilters = GetTrendFilters();
            model.SelectedFilter = input.Filter;
            model.ShowFormats = GetShowFormats();
            model.SelectedShowFormat = input.ShowFormat;
            model.All = input.All;
            model.From = input.From;
            model.To = input.To;
            model.LoadSubAccounts = String.IsNullOrEmpty(input.AccountId);

            var accountType = input.ShowFormat == ShowFormatEnum.Spending ? AccountTypeEnum.Expense : AccountTypeEnum.Income;

            #region Apply filtering

            var budgets = new List<TrendCategoryItem>();
            var budgetFilter = new TransactionsStatisticFilter
            {
                LedgerId = input.LedgerId,
                AccountType = accountType,
                ParentAccountId = input.AccountId
            };
            switch (input.Filter)
            {
                case TrendModelFilterEnum.AllTime:
                    break;
                case TrendModelFilterEnum.SelectedMonth:
                case TrendModelFilterEnum.PrevMonth:
                    model.Date = new DateTime(input.Year, input.Month, 1);
                    budgetFilter.Month = input.Month;
                    budgetFilter.Year = input.Year;
                    break;
                case TrendModelFilterEnum.Year:
                    budgetFilter.Year = DateTime.Now.Year;
                    break;
            }
            budgets = Load(budgetFilter);
            #endregion

            model.Categories = budgets.OrderByDescending(x => x.Amount).Take(input.TakeCategories).ToList();
            model.SelectedCategoryName = String.IsNullOrEmpty(input.AccountId) ? null : model.Categories.Single(x => x.AccountId == input.AccountId).Name;
            model.SelectedCategoryId = input.AccountId;


            var piePieces = new List<SetElement>();
            foreach (var category in model.Categories)
            {
                piePieces.Add(new SetElement((double)AccountingFormatter.CentsToDollars(category.Amount), category.Name)
                {
                    Link = category.HasSubCategories && String.IsNullOrEmpty(input.AccountId)
                        ? new Link(LinkType.JavaScriptFuncParams, string.Format("mPower.Trends.ShowSubCategories-{0}", category.AccountId))
                        : new Link(LinkType.Simple, input.FilterByAccountUrl.Replace(TrendsViewModelFilter.CategoryAcccountIdUrlKey,category.AccountId)),
                });
            }

            model.Chart = Charts.TrendsChart(piePieces);

            return model;
        }

        private static Dictionary<int, string> GetShowFormats()
        {
            return new Dictionary<int, string> { { 1, "Spending" }, { 2, "Income" } };
        }

        private static Dictionary<int, string> GetTrendFilters()
        {
            return new Dictionary<int, string> { { 1, "Selected month" }, { 2, "Last month" }, { 3, "This year" }, { 4, "All Time" } };
        }

        private List<TrendMonthModel> BuildMonthesModel()
        {
            var monthes = new List<TrendMonthModel>();
            //showing previous 8 months
            var initialDate = DateTime.Now.AddMonths(-8);

            var text = String.Empty;
            for (int i = 0; i < 12; i++)
            {
                text = initialDate.Month == 1
                           ? initialDate.ToString(@"MMM <\span cla\s\s='year'>yyyy</\span>")
                           : initialDate.ToString("MMM");

                monthes.Add(new TrendMonthModel() { Month = initialDate.Month, Year = initialDate.Year, Text = text });
                initialDate = initialDate.AddMonths(1);
            }

            return monthes;
        }


        public List<TrendCategoryItem> Load(TransactionsStatisticFilter filter)
        {
            var ledger = _ledgerDocumentService.GetById(filter.LedgerId);
            var accounts = ledger.Accounts;

            accounts = accounts.Where(x => x.TypeEnum == filter.AccountType).ToList();
            var categories = new Dictionary<string, TrendCategoryItem>();
            var accountIds = new List<string>();

            if (String.IsNullOrEmpty(filter.ParentAccountId))
            {
                //add top level accounts
                var parentAccounts = accounts.Where(x => String.IsNullOrEmpty(x.ParentAccountId)).ToList();
                foreach (var account in parentAccounts)
                {
                    categories.Add(account.Id, new TrendCategoryItem()
                    {
                        AccountId = account.Id,
                        Name = account.Name,
                        Amount = 0,
                        HasSubCategories = false
                    });
                }
            }
            else
            {
                accountIds.AddRange(accounts.Where(x => x.ParentAccountId == filter.ParentAccountId).Select(x => x.Id));
                accountIds.Add(filter.ParentAccountId);
                foreach (var accountId in accountIds)
                {
                    var account = accounts.Single(x => x.Id == accountId);
                    categories.Add(accountId, new TrendCategoryItem()
                    {
                        AccountId = account.Id,
                        Name = account.Name,
                        Amount = 0,
                        HasSubCategories = false
                    });
                }
            }
            List<TransactionsStatisticDocument> transactionsStatistic;
            if (accountIds.Count == 0)
            {
                transactionsStatistic = _statisticService.GetByFilter(filter);
            }
            else
            {
                filter.AccountIds = accountIds;
                transactionsStatistic = _statisticService.GetByFilter(filter);
            }

            foreach (var item in transactionsStatistic)
            {
                var account = accounts.Single(x => x.Id == item.AccountId);
                var parentAccount = accounts.FirstOrDefault(x => x.Id == account.ParentAccountId);

                if (parentAccount == null)
                {
                    //add amount from other monthes
                    categories[item.AccountId].Amount += AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(item.DebitAmountInCents, item.CreditAmountInCents, item.AccountType);
                    categories[item.AccountId].HasSubCategories = accounts.Count(x => x.ParentAccountId == account.Id) > 0;
                }
                else // sub account
                {
                    //that means that we are need only root level categories
                    //When user clicking parent category we need show sub categories and parent as well, just without sub categories budgets
                    if (accountIds.Count == 0)
                    {
                        categories[parentAccount.Id].Amount += AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(item.DebitAmountInCents, item.CreditAmountInCents, item.AccountType);
                        categories[parentAccount.Id].HasSubCategories = accounts.Count(x => x.ParentAccountId == parentAccount.Id) > 0;
                    }
                    else
                    {
                        categories[account.Id].Amount += AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(item.DebitAmountInCents, item.CreditAmountInCents, item.AccountType);
                    }
                }
            }

            return categories.Values.ToList();

        }
    }
}