using System;
using System.Collections.Generic;
using System.Linq;
using Default.Factories.Commands;
using Default.ViewModel.Areas.Finance.BudgetController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.WebApi.Tenants.Model.Budget;
using mPower.WebApi.Tenants.Model.Dashboard;
using mPower.WebApi.Tenants.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetFilter = Default.ViewModel.Areas.Finance.BudgetController.Filters.BudgetFilter;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class BudgetController : BaseController
    {
        private readonly BudgetDocumentService _budgetService;
        private readonly TransactionsStatisticDocumentService _statisticService;
        private readonly LedgerDocumentService _lenderService;
        private readonly IObjectRepository _objectRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly LedgerDocumentService _ledgerService;

        public BudgetController(BudgetDocumentService budgetService, TransactionsStatisticDocumentService statisticService, 
            LedgerDocumentService lenderService, IObjectRepository objectRepository, IIdGenerator idGenerator, 
            LedgerDocumentService ledgerService, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _budgetService = budgetService;
            _statisticService = statisticService;
            _lenderService = lenderService;
            _objectRepository = objectRepository;
            _idGenerator = idGenerator;
            _ledgerService = ledgerService;
        }

        [HttpGet("GetTopTenBudgets")]
        public DashboardBudgetViewModel GetTopTenBudgets(string ledgerId = null)
        {
            if (string.IsNullOrEmpty(ledgerId)) ledgerId = GetLedgerId();

            var date = DateTime.Now;
            var topTenBudgets = BuildTopTenBudgets(date);
            var totalIncome = GetMonthLyStatistic(date, AccountTypeEnum.Income);
            var totalExpense = GetMonthLyStatistic(date, AccountTypeEnum.Expense);
            var remainingTotal = totalIncome - totalExpense;

            return new DashboardBudgetViewModel
            {
                Budgets = topTenBudgets,
                TotalIncome = AccountingFormatter.ConvertToDollarsThenFormat(totalIncome),
                TotalExpense = AccountingFormatter.ConvertToDollarsThenFormat(totalExpense),
                RemainingTotal = AccountingFormatter.ConvertToDollarsThenFormat(remainingTotal)
            };
        }

        [HttpGet("GetAllTenBudgets")]
        public BudgetModel GetAllTenBudgets(string setup = null, int? month = null, int? year = null)
        {
            var model = BuildBudgetModel(month ?? DateTime.Now.Month, year ?? DateTime.Now.Year, setup);

            return model;
        }

        [HttpGet("Show/{month}/{year}")]
        public BudgetModel Show(int? month = null, int? year = null, string setup = null)
        {
            var model = BuildBudgetModel(month ?? DateTime.Now.Month, year ?? DateTime.Now.Year, setup);

            return model;
        }

        [HttpPost("UpdateBudget")]
        public IActionResult UpdateBudget([FromBody] BudgetUpdateModel model)
        {
            var budget = _budgetService.GetById(model.BudgetId);

            if(budget == null)
                throw new Exception("Budget was not found");

            var command = new Ledger_Budget_UpdateCommand
            {
                BudgetId = model.BudgetId,
                AmountInCents = model.Amount,
                LedgerId = GetLedgerId(),
                Month = model.Month,
                Year = model.Year
            };

            Send(command);

            return new OkResult();
        }

        [HttpPost("CreateBudget")]
        public IActionResult CreateBudget([FromBody] CreateBudgetModel model)
        {
            var contract = new BudgetsContract {income = model.Income, expense = model.Expense};
            var modelBudgets = new List<BudgetItemContract>();
            modelBudgets.AddRange(contract.expense);
            modelBudgets.AddRange(contract.income);

            CreateBudgets(modelBudgets, model.Month, model.Year);
            return new OkResult();
        }

        [HttpPost("AddBudget")]
        public IActionResult AddAccount([FromBody] AddBudgetModel model)
        {
            if (string.IsNullOrEmpty(model.AccountName))
            {
                ModelState.AddModelError(string.Empty, "AccountName of account is required");
                return new BadRequestObjectResult(ModelState);
            }
            if (!(model.Type == AccountTypeEnum.Income || model.Type == AccountTypeEnum.Expense))
            {
                ModelState.AddModelError(string.Empty, "Invalid account type. Should be expense or income");
                return new BadRequestObjectResult(ModelState);
            }

            if(ModelState.IsValid)
            {
                var cmd = new Ledger_Account_CreateCommand
                {
                    AccountId = _idGenerator.Generate(),
                    Name = model.AccountName,
                    LedgerId = GetLedgerId(),
                    AccountLabelEnum = model.Type == AccountTypeEnum.Income ? AccountLabelEnum.Income : AccountLabelEnum.Expense,
                    AccountTypeEnum = model.Type,
                    ParentAccountId = string.Empty,
                    Aggregated = false,
                    Description = string.Empty,
                    Imported = false,
                    Number = string.Empty,
                    InterestRatePerc = 0,
                    MinMonthPaymentInCents = 0,
                };

                Send(cmd);

                var ledger = _ledgerService.GetById(cmd.LedgerId);
                var account = ledger.Accounts.Single(x => x.Id == cmd.AccountId);
                var budgetModel = new BudgetItemModel
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    BudgetAmountInDollars = model.BudgetAmountInDollars,
                    IsIncludedInBudget = true,
                };

                return new OkObjectResult(budgetModel);
            }

            return new BadRequestResult();
        }

        #region private part

        private BudgetModel BuildBudgetModel(int month, int year, string setup = null)
        {
            var ledger = _lenderService.GetById(GetLedgerId());
            if (ledger.IsBudgetSet)
            {
                var budgets = _budgetService.GetLedgetBudgetsByMonthAndYear(month, year, ledger.Id);

                if (budgets.Count == 0)
                {
                    var copyFromBudgets = _budgetService.GetNearestBudgets(month, year, ledger.Id);
                    var modelBudgets = copyFromBudgets.Select(x => new BudgetItemContract
                    {
                        Budget = AccountingFormatter.CentsToDollarString(x.BudgetAmount),
                        Id = x.AccountId,
                        IncludeBudget = true
                    }).ToList();

                    CreateBudgets(modelBudgets, month, year);
                }
            }

            var filter = new BudgetFilter
            {
                Ledger = ledger,
                Year = year,
                Month = month,
                Setup = setup,
            };

            return _objectRepository.Load<BudgetFilter, BudgetModel>(filter);
        }

        private void CreateBudgets(List<BudgetItemContract> modelBudgets, int month, int year)
        {
            var dto = new BudgetsListDto
            {
                LedgerId = GetLedgerId(),
                Budgets = modelBudgets,
                Year = year,
                Month = month,
            };
            var command = _objectRepository.Load<BudgetsListDto, Ledger_Budget_SetCommand>(dto);

            Send(command);
        }

        private List<DashboardBudgetModel> BuildTopTenBudgets(DateTime date)
        {
            var budgets = _budgetService.GetLedgetBudgetsByMonthAndYear(date.Month, date.Year, GetLedgerId());

            var topTenBudgets = budgets.Select(budget => new DashboardBudgetModel
            {
                AccountName = budget.AccountName,
                BudgetAmount = budget.BudgetAmount,
                SpentAmount = budget.SpentAmountWithSubAccounts,
                Percentage = budget.Persent,
                AccountType = budget.AccountType
            }).ToList().OrderByDescending(x => x.BudgetAmount).ThenBy(x => x.SpentAmount).Take(10).ToList();

            return topTenBudgets;
        } 

        private long GetMonthLyStatistic(DateTime date, AccountTypeEnum accountType)
        {
            var filter = new TransactionsStatisticFilter
            {
                LedgerId = GetLedgerId(),
                Year = date.Year,
                Month = date.Month,
                AccountType = accountType
            };

            return
                _statisticService.GetByFilter(filter)
                    .Sum(s => AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                        s.DebitAmountInCents, s.CreditAmountInCents, s.AccountType));
        }

        #endregion
    }
}