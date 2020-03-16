using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Default.ViewModel.Areas.Finance.DebtElimninationProgramController;
using mPower.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.DebtElimination.Commands;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;
using mPower.EventHandlers.Immediate;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Services;
using mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Step1Model = mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram.Step1Model;
using mPower.Documents.Documents.Accounting.DebtElimination;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using System.Dynamic;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Diagnostics;
using Default;
using mPower.Documents.DocumentServices.Membership;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/debtelimination")]
    public class DebtElimninationProgramController : BaseController
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly DebtViewModelBuilder _debtViewModelBuilder;
        private readonly IIdGenerator _idGenerator;
        private readonly CalendarDocumentService _calendarService;
        private readonly DebtCalculator _calculator;
        private readonly HtmlToPdfWriter _htmlToPdfWriter;
        private readonly UserDocumentService _userService;

        public DebtElimninationProgramController(LedgerDocumentService ledgerService, DebtEliminationDocumentService debtEliminationService,
            DebtViewModelBuilder debtViewModelBuilder, IIdGenerator idGenerator, CalendarDocumentService calendarService,
            DebtCalculator calculator, UserDocumentService userService, ICommandService command, IApplicationTenant tenant) :
            base(command, tenant)
        {
            _ledgerService = ledgerService;
            _debtEliminationService = debtEliminationService;
            _debtViewModelBuilder = debtViewModelBuilder;
            _idGenerator = idGenerator;
            _calendarService = calendarService;
            _calculator = calculator;
            //_htmlToPdfWriter = htmlToPdfWriter;
            _userService = userService;
        }

        [HttpGet("step1")]
        public Step1Model Step1()
        {
            var debtId = CreateDebtIfNotExists();
            var debt = _debtEliminationService.GetById(debtId);
            var debtsIds = debt.Debts.Select(d => d.DebtId).ToList();
            var model = new Step1Model();
            var ledger = _ledgerService.GetById(GetLedgerId());
            model.Debts =
                ledger.Accounts.Where(
                    x => x.LabelEnum == AccountLabelEnum.CreditCard || x.LabelEnum == AccountLabelEnum.Loan || x.LabelEnum == AccountLabelEnum.Bank || x.LabelEnum == AccountLabelEnum.Investment).Select(x =>
                        new DebtModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            LabelEnum = x.LabelEnum,
                            Balance = x.ActualBalance,
                            InterestRatePerc = x.InterestRatePerc,
                            MinMonthPaymentInCents = x.MinMonthPaymentInCents,
                            UseInProgram = debtsIds.Contains(x.Id),
                        }).ToList();

            return model;
        }

        [HttpPost("proceedstep2")]
        public IActionResult ProceedToStep2([FromBody]DebtIdsModel model)
        {
            var meaningfulDebts = GetMeaningfulDebts(model.DebtIds);
            if (!DebtsAreValid(meaningfulDebts))
            {
                return new BadRequestObjectResult(ModelState);
            }

            var command = new DebtElimination_Debts_SetCommand
            {
                DebtEliminationId = CreateDebtIfNotExists(),
                Debts = meaningfulDebts,
            };

            Send(command);

            return new OkResult();
        }

        [HttpGet("step2")]
        public Step2Model Step2()
        {
            var debtId = CreateDebtIfNotExists();
            var currentUser = _userService.GetById(GetUserId());
            var debt = _debtEliminationService.GetById(debtId);
            var model = new Step2Model
            {
                EstimatedInvestmentEarningsRate = debt.EstimatedInvestmentEarningsRate,
                YearsUntilRetirement = debt.YearsUntilRetirement,
                MonthlyBudget = AccountingFormatter.CentsToDollars(debt.MonthlyBudgetInCents),
                AmountToSavings = AccountingFormatter.CentsToDollars(debt.AmountToSavings),
                LumpSumAmount = AccountingFormatter.CentsToDollars(debt.LumpSumAmount),
                CurrentDebtMonth = debt.CurrentDebtMonth,
                NewLoanAmount= debt.NewLoanAmount,
                LoanInterestRate=debt.LoanInterestRate,
                MaxLoans=debt.MaxLoans,
                CurrentSavingsTotal= AccountingFormatter.CentsToDollars(debt.CurrentSavingsTotal),
                CurrentDeathBenefit= AccountingFormatter.CentsToDollars(debt.CurrentDeathBenefit),
                DeathBenefitTerminatesAge=debt.DeathBenefitTerminatesAge,
                MonthlySavingsContribution=AccountingFormatter.CentsToDollars(debt.MonthlySavingsContribution),
                Term1=debt.Term1,
                Term2=debt.Term2,
                Term1Amount=AccountingFormatter.CentsToDollars(debt.Term1Amount),
                Term2Amount=AccountingFormatter.CentsToDollars(debt.Term2Amount),
                MonthlyContributionFBS=debt.MonthlyContributionFBS,
                BudgetForFBS=debt.BudgetForFBS,
                RecommendedBudgetInCents = (long)Math.Round(_ledgerService.GetById(GetLedgerId()).Accounts
                    .Where(
                        x =>
                            (x.LabelEnum == AccountLabelEnum.CreditCard || x.LabelEnum == AccountLabelEnum.Loan) &&
                            x.ActualBalance > 0)
                    .Sum(x => x.MinMonthPaymentInCents) * 0.2, 0),
                Plan = ((int)debt.PlanId).ToString(CultureInfo.InvariantCulture),
                Plans = new List<SelectListItem> { new SelectListItem { Text = "Please Choose...", Value = "0" } }
            };
            // + 20% over all min monthly payments 
            var plans = Enum.GetValues(typeof(DebtEliminationPlanEnum)).Cast<DebtEliminationPlanEnum>()
                .Except(new[] { DebtEliminationPlanEnum.NotInitialized })
                .Select(e => new SelectListItem { Text = e.GetDescription(), Value = ((int)e).ToString(CultureInfo.InvariantCulture) });
            model.Plans.AddRange(plans);
            model.IsAgent = currentUser.IsAgent;
            model.CurrentTotalMonthlyPayments = AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Debts.Sum(x => x.MinMonthPaymentInCents)));
            return model;
        }

        [HttpPost("proceedstep3")]
        public IActionResult ProceedToStep3([FromBody]SelectPlanModel model)
        {
            if ( model != null)
            {
                if (model.Plan == DebtEliminationPlanEnum.NotInitialized || model.MonthlyBudget < 0)
                {
                    ModelState.AddModelError("Budget", "Please, specify program and positive budget.");
                    return new BadRequestObjectResult(ModelState);
                }
                else
                {
                    var debtId = CreateDebtIfNotExists();
                    var debt = _debtEliminationService.GetById(debtId);
                    if ((AccountingFormatter.DollarsToCents(model.MonthlyBudget) + debt.Debts.Sum(x => x.MinMonthPaymentInCents)) < AccountingFormatter.DollarsToCents(model.AmountToSavings))
                    {
                        ModelState.AddModelError("Budget", "Amount to Savings should be greater than the sum of total amount of minimum monthly payment and monthly budget.");
                        return new BadRequestObjectResult(ModelState);
                    }
                    if (!model.BudgetForFBS)
                    {
                        if (model.MonthlyBudget >= 0)
                        {
                            if ((AccountingFormatter.DollarsToCents(model.Term1Amount)) > AccountingFormatter.DollarsToCents(model.MonthlyBudget) || (AccountingFormatter.DollarsToCents(model.Term2Amount)) > AccountingFormatter.DollarsToCents(model.MonthlyBudget))
                            {
                                ModelState.AddModelError("Budget", "Debt elimination budget must be greater than Term 1 or Term 2 amount.");
                                return new BadRequestObjectResult(ModelState);
                            }
                            if(model.YearsUntilRetirement*12 < model.Term1 + model.Term2)
                            {
                                ModelState.AddModelError("Budget", "Term insurance months must be greater than the months until retirement.");
                                return new BadRequestObjectResult(ModelState);
                            }
                            //if ((AccountingFormatter.DollarsToCents(model.Term2Amount)) > AccountingFormatter.DollarsToCents(model.MonthlyBudget))
                            //{
                            //    ModelState.AddModelError("Budget", "Debt elimination budget must be greater than Term 2 amount.");
                            //    return new BadRequestObjectResult(ModelState);
                            //}
                        }
                    }
                    
                }
                
                var command = new DebtElimination_EliminationPlanUpdateCommand
                {
                    Id = CreateDebtIfNotExists(),
                    MonthlyBudgetInCents = AccountingFormatter.DollarsToCents(model.MonthlyBudget),
                    PlanId = model.Plan,
                    EstimatedInvestmentEarningsRate = model.EstimatedInvestmentEarningsRate,
                    YearsUntilRetirement = model.YearsUntilRetirement,
                    AmountToSavings = AccountingFormatter.DollarsToCents(model.AmountToSavings),
                    LumpSumAmount = AccountingFormatter.DollarsToCents(model.LumpSumAmount),
                    NewLoanAmount= model.MaxLoans.Count > 0 ? model.MaxLoans[0].MaxNewLoan : 0,
                    CurrentDebtMonth= model.CurrentDebtMonth,
                    LoanInterestRate=model.LoanInterestRate,
                    MaxLoans=model.MaxLoans,
                    CurrentSavingsTotal= AccountingFormatter.DollarsToCents(model.CurrentSavingsTotal),
                    CurrentDeathBenefit=AccountingFormatter.DollarsToCents(model.CurrentDeathBenefit),
                    DeathBenefitTerminatesAge=model.DeathBenefitTerminatesAge,
                    MonthlySavingsContribution=AccountingFormatter.DollarsToCents(model.MonthlySavingsContribution),
                    Term1=model.Term1,
                    Term2=model.Term2,
                    Term1Amount=AccountingFormatter.DollarsToCents(model.Term1Amount),
                    Term2Amount=AccountingFormatter.DollarsToCents(model.Term2Amount),
                    MonthlyContributionFBS= model.MonthlyContributionFBS,
                    BudgetForFBS= model.BudgetForFBS
                };

                Send(command);
                return new OkResult();
            }
            else
            {
                ModelState.AddModelError("Budget", "Please, specify program and positive budget.");
                return new BadRequestObjectResult(ModelState);
            }
            
        }

        [HttpPost("updatecharts")]
        public Step3Model UpdateChart([FromBody]int displayModeKey)
        {
            var displayMode = (DebtEliminationDisplayModeEnum)displayModeKey;
            var debt = _debtEliminationService.GetById(CreateDebtIfNotExists());
            debt.DisplayMode = displayMode;

            var command = new DebtElimination_DisplayMode_UpdateCommand
            {
                Id = debt.Id,
                DisplayMode = displayMode,
            };

            Send(command);

            return _debtViewModelBuilder.GetDebtEliminationStep3Model(debt);
        }

        [HttpGet("step3")]
        public Step3Model Step3()
        {
            var debtId = CreateDebtIfNotExists();
            var debt = _debtEliminationService.GetById(debtId);
            return _debtViewModelBuilder.GetDebtEliminationStep3Model(debt);
        }
        private static long DollarsToCents(decimal dollars)
        {
            return AccountingFormatter.DollarsToCents(Math.Round(dollars, 2));
        }

        [HttpPost("QuickSavingsAnalysis")]
        public Step3Model QuickSavingsAnalysis([FromBody]DebtEliminationDocument quickSavingsModel)
        {
            List<DebtItemData> debtItem = new List<DebtItemData>();
            var id = 1;
            var debtName = "Debt";
            EstimatedInvestment estimatedInvestment = new EstimatedInvestment();
            foreach (var item in quickSavingsModel.Debts)
            {
                estimatedInvestment.MonthlyDepositInDollars += Convert.ToInt64(item.MinMonthPaymentInCents);
                item.BalanceInCents = DollarsToCents(item.BalanceInCents);
                item.MinMonthPaymentInCents = DollarsToCents(item.MinMonthPaymentInCents);
                item.InterestRatePerc = item.InterestRatePerc;
                item.DebtId = Convert.ToString(id);
                item.Name = debtName + id;
                debtItem.Add(item);
                id++;               
            }
            estimatedInvestment.MonthlyDepositInDollars = estimatedInvestment.MonthlyDepositInDollars + quickSavingsModel.MonthlyBudgetInCents;
            DebtEliminationDocument calculatedData = new DebtEliminationDocument()
            {
                Debts = debtItem,
                MonthlyBudgetInCents = DollarsToCents(quickSavingsModel.MonthlyBudgetInCents),
                DisplayMode = quickSavingsModel.DisplayMode,
                PlanId = quickSavingsModel.PlanId,
                EstimatedInvestmentEarningsRate= quickSavingsModel.EstimatedInvestmentEarningsRate,
                
                // Debt Elimination Changes
                AmountToSavings= quickSavingsModel.AmountToSavings,
                LumpSumAmount=quickSavingsModel.LumpSumAmount

            };

            var origDetails = _calculator.CalcOriginaldDetails(calculatedData, false, false);
            var accDetails = _calculator.CalcAcceleratedDetails(calculatedData, false,true);

            var model = new Step3Model
            {
                TotalPayed = (double)AccountingFormatter.CentsToDollars(origDetails.Sum(d => d.ActualPaymentInCents)),
                TotalPayedViaPlan = (double)AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.ActualPaymentInCents)),
                PayoffTime = origDetails.Count > 0 ? Math.Round((origDetails.Last().Date - origDetails.First().Date).TotalDays / 365, 1) : 0,
                PayoffTimeViaPlan = accDetails.Count > 0 ? Math.Round((accDetails.Last().Date - accDetails.First().Date).TotalDays / 365, 1) : 0,
                DisplayMode = calculatedData.DisplayMode,
                EstimatedInvestmentEarningsRate = quickSavingsModel.EstimatedInvestmentEarningsRate
            };

            foreach (var d in origDetails)
            {
                if (!string.IsNullOrEmpty(d.ErrorMessage))
                {
                    model.ErrorMessage = d.ErrorMessage;
                }
            }

            foreach (var d in accDetails)
            {
                if (!string.IsNullOrEmpty(d.ErrorMessage))
                {
                    model.ErrorMessage = d.ErrorMessage;
                }
            }

            //var calculatedDataQuickSaving = _debtViewModelBuilder.GetDebtEliminationStep3Model(calculatedData, estimatedInvestment);

            // To Calculate the Accelarate Wealth after elimination of all debts.
            estimatedInvestment.EarningsRate = quickSavingsModel.EstimatedInvestmentEarningsRate / 100;
            estimatedInvestment.InvestmentTimeInYears = (quickSavingsModel.YearsUntilRetirement - model.PayoffTimeViaPlan);
            estimatedInvestment.CompoundingInterval = Convert.ToInt32(QuickSavingCompoundEnum.AnnualCompounded);

            double TotalAccelarteWealth = 0;
            double investedTimeInMonths = Math.Round((estimatedInvestment.InvestmentTimeInYears * 12), MidpointRounding.AwayFromZero);

            var wealthId = 1;
            var wealthName = "Wealth";
            
            List<WealthDetailsItemShort> lstWealthDetailsItemShort = new List<Controllers.WealthDetailsItemShort>();

            WealthDetailsItemShort objWealthDetailsItemShort = new WealthDetailsItemShort();

            objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
            objWealthDetailsItemShort.Name = wealthName + wealthId;
            objWealthDetailsItemShort.Wealth = estimatedInvestment.MonthlyDepositInDollars;
           // var investementStartDate = DateTime.Now.AddMonths(Convert.ToInt32(Math.Round((quickSavingsModel.YearsUntilRetirement - model.PayoffTimeViaPlan) * 12))); //new
           var investementStartDate = DateTime.Now.AddMonths(Convert.ToInt32(Math.Round(model.PayoffTimeViaPlan * 12)));
            objWealthDetailsItemShort.Date = investementStartDate;

            var nextInvestementCalulationDateNew = investementStartDate;
            lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);

            var nextInvestementCalulationDate = nextInvestementCalulationDateNew;

            var previoursInventmentValue = investementStartDate;

            while (investedTimeInMonths > 0)
            {
                
                TotalAccelarteWealth += estimatedInvestment.MonthlyDepositInDollars * (Math.Pow((1 + (estimatedInvestment.EarningsRate / estimatedInvestment.CompoundingInterval)), (estimatedInvestment.InvestmentTimeInYears * estimatedInvestment.CompoundingInterval)));
                estimatedInvestment.InvestmentTimeInYears = ((Math.Round((estimatedInvestment.InvestmentTimeInYears * 12), MidpointRounding.AwayFromZero) - 1) / 12);
                investedTimeInMonths--;

                nextInvestementCalulationDate = nextInvestementCalulationDate.AddMonths(1);

                objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                objWealthDetailsItemShort.Name = wealthName + wealthId;

                // if (investedTimeInMonths % 12 == 0 && investedTimeInMonths >= 12)
                
                 var dateDiff = AccountingFormatter.GetMonthsBetween(nextInvestementCalulationDate,previoursInventmentValue);
                if (dateDiff == 12)
                {                    
                    objWealthDetailsItemShort = new WealthDetailsItemShort();
                    objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                    objWealthDetailsItemShort.Name = wealthName + wealthId;
                    objWealthDetailsItemShort.Wealth = TotalAccelarteWealth;
                    objWealthDetailsItemShort.Date = nextInvestementCalulationDate;
                    lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);
                    previoursInventmentValue = nextInvestementCalulationDate;
                }
                else
                {
                    objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                    objWealthDetailsItemShort.Name = wealthName + wealthId;
                    if (investedTimeInMonths == 0)
                    {                        
                        objWealthDetailsItemShort = new WealthDetailsItemShort();
                        objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                        objWealthDetailsItemShort.Name = wealthName + wealthId;
                        objWealthDetailsItemShort.Wealth = TotalAccelarteWealth;
                        objWealthDetailsItemShort.Date = nextInvestementCalulationDate;
                        lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);                       
                    }
                }

            }

            model.TotalEstimatedInvestment = Math.Round(TotalAccelarteWealth, 2);
            model.YearsUntilRetirement = quickSavingsModel.YearsUntilRetirement;


            var displayModes = ((DebtEliminationDisplayModeEnum[])Enum.GetValues(typeof(DebtEliminationDisplayModeEnum))).ToDictionary(dm => (int)dm, dm => dm.GetDescription());
            model.DisplayModes = new SelectList(displayModes, "Key", "Value", (int)calculatedData.DisplayMode);
            model.AddedToCalendar = calculatedData.AddedToCalendar;

            switch (calculatedData.DisplayMode)
            {
                case DebtEliminationDisplayModeEnum.Program:
                    model.Details = accDetails;
                    model.Chart = BuildChart(model.Details);
                    break;
                case DebtEliminationDisplayModeEnum.Minimums:
                    model.Details = origDetails;
                    model.Chart = BuildChart(model.Details);
                    break;
                case DebtEliminationDisplayModeEnum.ProgramAndMinimums:
                   model.Details = accDetails;
                  // model.Chart = BuildChart(origDetails, accDetails);

                   model.Chart = BuildChart(origDetails, accDetails, lstWealthDetailsItemShort);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return model;
           
        }
        
        private ChartViewModel BuildChart(IList<ProgramDetailsItemShort> details1,
            IList<ProgramDetailsItemShort> details2 = null, List<WealthDetailsItemShort>details3 = null)
        {
            var postfix1 = string.Empty;
            var postfix2 = string.Empty;
            var postfix3 = string.Empty;
            var minDate = details1.Count > 0 ? details1[0].Date : DateTime.Now;
            var maxDate = details1.Count > 0 ? details1.Last().Date : DateTime.Now;

            var minDate3 = details3.Count > 0 ? details3[0].Date : DateTime.Now;
            var maxDate3 = details3.Count > 0 ? details3.Last().Date : DateTime.Now;

            if (details2 != null)
            {
                postfix1 = " (Minimums)";
                postfix2 = " (Program)";
                postfix3 = " (Wealth)";

                if (details2.Count > 0)
                {
                    var minDate2 = details2[0].Date;
                    var maxDate2 = details2.Last().Date;

                    minDate = minDate < minDate2 ? minDate : minDate2;
                    maxDate = maxDate > maxDate2 ? maxDate : maxDate2;
                }
            }

            var dates = new Dictionary<DateTime, string>();
            var dates3 = new Dictionary<DateTime, string>();
            var chartViewModel = new ChartViewModel
            {
                XKey = "y",
                Data = new List<dynamic>(),
                Labels = new List<string>(),
                YKeys = new List<string>()
            };
            for (var i = minDate; i <= maxDate; i = i.AddYears(1))
            {
                dates.Add(i, i.ToString("MM/yy"));
                chartViewModel.Data.Add(new ExpandoObject());
            }

            for (var i = minDate3; i <=maxDate3; i = i.AddYears(1))
            {
                dates3.Add(i, i.ToString("MM/yy"));
                chartViewModel.Data.Add(new ExpandoObject());
            }
            if (!dates3.ContainsKey(maxDate3))
            {
                dates3.Add(maxDate3, maxDate3.ToString("MM/yy"));
                chartViewModel.Data.Add(new ExpandoObject());
            }
           

            string result = "";
            result=AddSeries(chartViewModel, details1, postfix1, dates);
            if (details2 != null)
            {
                result=AddSeries(chartViewModel, details2, postfix2, dates, details1.Count);
            }
            var j = 0;
            if (result == "old")
            {
                foreach (var date in dates.Keys)
                {
                    ((IDictionary<string, object>)chartViewModel.Data[j]).Add("y", date.ToString("yyyy-MM"));
                    j++;
                }
            }
            if (details3 != null)
            {
              result=  AddSeries(chartViewModel, details3, postfix3, dates3, j);
            }

            

            if (result == "new")
            {
                
                foreach (var date in dates3.Keys)
                {
                    ((IDictionary<string, object>)chartViewModel.Data[j]).Add("y", date.ToString("yyyy-MM"));
                    j++;
                }
            }
            return chartViewModel;
        }

        private static string AddSeries(ChartViewModel chartViewModel, IEnumerable<ProgramDetailsItemShort> details, string postfix, Dictionary<DateTime, string> dates, int j = 0)
        {
            var detailsGroups = details.GroupBy(d => d.Id).OrderBy(g => g.Key);
            foreach (var group in detailsGroups)
            {
                var name = string.Empty;
                if (group.Any())
                {
                    name = group.First().Debt;
                    var i = 0;
                    foreach (var date in dates)
                    {
                        var item = group.FirstOrDefault(gi => gi.Date == date.Key);
                        var balance = item == null ? 0 : (double)AccountingFormatter.CentsToDollars(item.BalanceInCents);
                        ((IDictionary<string, object>)chartViewModel.Data[i]).Add($"x_{j}", balance);
                        i++;
                    }
                }
                chartViewModel.Labels.Add(name + postfix);
                chartViewModel.YKeys.Add($"x_{j}");
                j++;
            }
            return "old";
        }
        private static string AddSeries(ChartViewModel chartViewModel, IEnumerable<WealthDetailsItemShort> details, string postfix, Dictionary<DateTime, string> dates, int j)
        {
            var detailsGroups = details.GroupBy(d => d.Id).OrderBy(g => g.Key);
            foreach (var group in detailsGroups)
            {
                var name = string.Empty;
                if (group.Any())
                {
                    name = group.First().Name;
                    var i = j;
                    foreach (var date in dates)
                    {
                        var item = group.FirstOrDefault(gi => gi.Date == date.Key);
                        var balance = item == null ? 0 : (item.Wealth);
                        ((IDictionary<string, object>)chartViewModel.Data[i]).Add($"x_{j}", balance);
                        i++;
                    }
                }
                chartViewModel.Labels.Add(name + postfix);
                chartViewModel.YKeys.Add($"x_{j}");
                j++;
            }
            return "new";
        }

        private static void AddSeriesNew(ChartViewModel chartViewModel, IEnumerable<ProgramDetailsItemShort> details, string postfix, Dictionary<DateTime, string> dates, int j = 0)
        {
            var detailsGroups = details.GroupBy(d => d.Id).OrderBy(g => g.Key);
            foreach (var group in detailsGroups)
            {
                var name = string.Empty;
                if (group.Any())
                {
                    name = group.First().Debt;
                    var i = 0;
                    foreach (var date in dates)
                    {
                        var item = group.FirstOrDefault(gi => gi.Date == date.Key);
                        var balance = item == null ? 0 : (double)AccountingFormatter.CentsToDollars(item.BalanceInCents);
                        ((IDictionary<string, object>)chartViewModel.Data[i]).Add($"x_{j}", balance);
                        i++;
                    }
                }
                chartViewModel.Labels.Add(name + postfix);
                chartViewModel.YKeys.Add($"x_{j}");
                j++;
            }
        }


        //public Step3Model QuickSavingsAnalysis([FromBody]DebtEliminationDocument quickSavingsModel)
        //{            
        //    List<DebtItemData> debtItem = new List<DebtItemData>();
        //    var id = 1;
        //    var debtName = "Debt";
        //    foreach (var item in quickSavingsModel.Debts)
        //    {                
        //        item.BalanceInCents = DollarsToCents(item.BalanceInCents);
        //        item.MinMonthPaymentInCents = DollarsToCents(item.MinMonthPaymentInCents);
        //        item.InterestRatePerc = item.InterestRatePerc;
        //        item.DebtId = Convert.ToString(id);
        //        item.Name = debtName + id;
        //        debtItem.Add(item);
        //        id++;
        //    }

        //    DebtEliminationDocument calculatedData = new DebtEliminationDocument()
        //    {
        //        Debts = debtItem,
        //        MonthlyBudgetInCents = DollarsToCents(quickSavingsModel.MonthlyBudgetInCents),
        //        DisplayMode = quickSavingsModel.DisplayMode,
        //        PlanId = quickSavingsModel.PlanId,

        //    };

        //    var calculatedDataQuickSaving =_debtViewModelBuilder.GetDebtEliminationStep3Model(calculatedData);

        //    EstimatedInvestment estimatedInvestment = new EstimatedInvestment()
        //    {
        //        Deposit = quickSavingsModel.MonthlyBudgetInCents,
        //        EarningsRate = quickSavingsModel.EstimatedInvestmentEarningsRate/100,
        //        PayOffTime = (quickSavingsModel.YearsUntilRetirement-calculatedDataQuickSaving.PayoffTimeViaPlan),
        //        AnnualCompounded = Convert.ToInt32(QuickSavingCompoundEnum.AnnualCompounded)
        //    };
        //    double TotalAccelarteWealth = 0;
        //    double investedTimeInMonths = Math.Round((estimatedInvestment.PayOffTime * 12),MidpointRounding.AwayFromZero);
        //    List<WealthDetailsItemShort> lstWealthDetailsItemShort = new List<Controllers.WealthDetailsItemShort>();
        //    WealthDetailsItemShort objWealthDetailsItemShort = new WealthDetailsItemShort();
        //    objWealthDetailsItemShort.Wealth = estimatedInvestment.Deposit;
        //    objWealthDetailsItemShort.Date = DateTime.Now;
        //    lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);

        //    var nextMonth = DateTime.Now;
        //    var date = new DateTime();
        //    while (investedTimeInMonths > 0)
        //    {

        //        TotalAccelarteWealth += estimatedInvestment.Deposit *( Math.Pow((1 + (estimatedInvestment.EarningsRate / estimatedInvestment.AnnualCompounded)), (estimatedInvestment.PayOffTime * estimatedInvestment.AnnualCompounded)));
        //         estimatedInvestment.PayOffTime = ((Math.Round((estimatedInvestment.PayOffTime * 12), MidpointRounding.AwayFromZero) - 1) / 12);
        //        investedTimeInMonths--;
        //        nextMonth = nextMonth.AddMonths(1);
        //        date = new DateTime(nextMonth.Year, nextMonth.Month, 1);

        //        if (investedTimeInMonths % 12 == 0 && investedTimeInMonths >= 12)
        //        {
        //            objWealthDetailsItemShort = new WealthDetailsItemShort();
        //            objWealthDetailsItemShort.Wealth = TotalAccelarteWealth;
        //            objWealthDetailsItemShort.Date = date;
        //            lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);

        //        }
        //        else { // To Be Code
        //        }

        //    }

        //    calculatedDataQuickSaving.TotalEstimatedInvestment = Math.Round(TotalAccelarteWealth, 2);
        //    calculatedDataQuickSaving.YearsUntilRetirement = quickSavingsModel.YearsUntilRetirement;
        //    return calculatedDataQuickSaving;
        //}
        [HttpPost("addtocalendar")]
        public IActionResult AddToCalendar()
        {
            var debt = _debtEliminationService.GetById(CreateDebtIfNotExists());
            var calendar = _calendarService.GetByFilter(new CalendarFilter { LedgerId = GetLedgerId() }).OrderBy(c => c.Type).FirstOrDefault();
            if (debt != null && calendar != null)
            {
                var details = _calculator.CalcAcceleratedDetails(debt);

                var onetimeCommands = details.Select(detailsItem =>
                    new Calendar_OnetimeCalendarEvent_CreateCommand
                    {
                        CalendarEventId = _idGenerator.Generate(),
                        CalendarId = calendar.Id,
                        CreatedDate = DateTime.Now,
                        EventDate = detailsItem.Date.AddDays(-1), // warning a day before payment
                        Description = CalendarDocumentEventHandler.GetEliminationItemDescription(detailsItem),
                        IsDone = false,
                        SendAlertOptions = new SendAlertOption
                        {
                            Mode = AlertModeEnum.Email,
                        },
                        ParentId = debt.Id,
                        UserId = debt.UserId,
                    }).ToList();

                Send(onetimeCommands.ToArray());
                return new OkResult();
            }
            return new BadRequestResult();
        }


        #region Helpers

        private bool DebtsAreValid(ICollection<DebtItemData> meaningfulDebts)
        {
            if (meaningfulDebts.Count == 0)
            {
                ModelState.AddModelError("Debts", "There are no selected debts suitable for calculation: it should has balance more then zero.");
                return false;
            }
            if (!meaningfulDebts.All(x => x.InterestRatePerc >= 0 && x.MinMonthPaymentInCents > 0))
            {
                ModelState.AddModelError("Debts", "All selected debts with a positive balance should have an minimum monthly payment greater than zero.");
                return false;
            }
            if (!meaningfulDebts.All(HasValidMinMonthlyPayment))
            {
                ModelState.AddModelError("Debts", "Some of selected debts have minimum monthly payment less then its interest payment.");
                return false;
            }

            return true;
        }

        private string CreateDebtIfNotExists()
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            string debtId;

            if (debt == null)
            {
                debtId = _idGenerator.Generate();
                var command = new DebtElimination_CreateCommand
                {
                    Id = debtId,
                    LedgerId = GetLedgerId(),
                    UserId = GetUserId(),
                };

                Send(command);
            }
            else
                debtId = debt.Id;

            return debtId;
        }

        private List<DebtItemData> GetMeaningfulDebts(ICollection<string> debtsIds)
        {
            var ledger = _ledgerService.GetById(GetLedgerId());
            var meaningfulDebts = ledger.Accounts.Where(x => debtsIds.Contains(x.Id) && x.ActualBalance > 0).Select(x =>
                new DebtItemData
                {
                    DebtId = x.Id,
                    Name = x.Name,
                    BalanceInCents = x.ActualBalance,
                    InterestRatePerc = x.InterestRatePerc,
                    MinMonthPaymentInCents = x.MinMonthPaymentInCents
                }).ToList();
            return meaningfulDebts;
        }

        private static bool HasValidMinMonthlyPayment(DebtItemData debt)
        {
            return debt.MinMonthPaymentInCents > Convert.ToInt64(debt.BalanceInCents) * Convert.ToInt64(debt.InterestRatePerc / 12 / 100);//usha
        }

        [HttpPost("ImageData")]
        public void ImageData([FromBody]Step3Model model)
        {
            System.IO.File.WriteAllText(@"C:\ChartImageUrl.txt", model.LoanBalanceImageUrl);
            System.IO.File.WriteAllText(@"C:\DebtSummaryImageUrl.txt", model.DebtSummaryImageUrl);
        }

        [HttpGet("ExportToPDF")]
        public IActionResult ExportToPDF(string ChartImage)
        {
            ChartImage = System.IO.File.ReadAllText(@"C:\ChartImageUrl.txt");
            if (System.IO.File.Exists(@"C:\ChartImageUrl.txt"))
            {
                System.IO.File.Delete(@"C:\ChartImageUrl.txt");
            }

            string DebtSummaryImageUrl = System.IO.File.ReadAllText(@"C:\DebtSummaryImageUrl.txt");
            if (System.IO.File.Exists(@"C:\DebtSummaryImageUrl.txt"))
            {
                System.IO.File.Delete(@"C:\DebtSummaryImageUrl.txt");
            }
            var headerFont = FontFactory.GetFont("Arial", 24, Font.NORMAL);
            var headerFontBold = FontFactory.GetFont("Arial", 24, Font.BOLD);
            var titleFont = FontFactory.GetFont("Arial", 18, Font.NORMAL);
            var subtitleFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
            var text = FontFactory.GetFont("Arial", 12, Font.BOLD);
            var document = new Document(new Rectangle(612,850), 15f, 15f, 30f, 30f);
            document.SetMargins(50f, 50f, 38f, 38f);
            var debtTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD, new BaseColor(255, 255, 255)); 
        
            var stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;
            document.Open();

            string path = Directory.GetCurrentDirectory();
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance( path + @"\Images\Alliance-pfm.jpg", true);
            logo.ScaleAbsolute(280f, 100f);
            logo.SetAbsolutePosition(document.Left + 110, document.Top - 130);
            logo.SpacingBefore = 50f;
            logo.SpacingAfter = 50f;
            logo.Alignment = 1;

            var debtId = CreateDebtIfNotExists();
            var debt = _debtEliminationService.GetById(debtId);
            Step3Model model = _debtViewModelBuilder.GetDebtEliminationStep3Model(debt);
            var currentUser = _userService.GetById(GetUserId());

            #region front page
            Paragraph header1 = new Paragraph(new Chunk("Debt Elimination", FontFactory.GetFont("Arial", 30, Font.NORMAL)));
            header1.SpacingBefore = 200;
            header1.Alignment = 1; // for center
            header1.Font = FontFactory.GetFont("Arial", 30, Font.NORMAL);

            Paragraph header2 = new Paragraph(new Chunk("&", FontFactory.GetFont("Arial", 30, Font.NORMAL)));
            header2.SpacingBefore = 10;
            header2.Alignment = 1; // for center
            header2.Font = FontFactory.GetFont("Arial", 30, Font.NORMAL);

            Paragraph header3 = new Paragraph(new Chunk("Family Banking System", FontFactory.GetFont("Arial", 30, Font.NORMAL)));
            header3.SpacingBefore = 10;
            header3.Alignment = 1; // for center
            header3.Font = FontFactory.GetFont("Arial", 30, Font.NORMAL);

            Paragraph header4 = new Paragraph(new Chunk("Report Prepared for:", headerFont));
            header4.SpacingBefore = 20;
            header4.Alignment = 1; // for center
            header4.Font = headerFont;

            Paragraph header5 = new Paragraph(new Chunk(currentUser.FullName, headerFont));
            header5.SpacingBefore = 20;
            header5.Alignment = 1; // for center
            header5.Font = headerFont;

            Paragraph header6 = new Paragraph(new Chunk("If You Follow This Plan \r\n You Will Be Debt Free In", headerFontBold));
            header6.SpacingBefore = 20;
            header6.Alignment = 1; // for center
            header6.Font = headerFontBold;

            Paragraph header7 = new Paragraph(new Chunk(model.PayTime +" Years", FontFactory.GetFont("Arial", 36, Font.BOLD)));
            header7.SpacingBefore = 20;
            header7.SpacingAfter = 20;
            header7.Alignment = 1; // for center
            header7.Font = FontFactory.GetFont("Arial", 36, Font.BOLD);

 
            document.Add(logo);            
            document.Add(header1);
            document.Add(header2);
            document.Add(header3);
            document.Add(header4);
            document.Add(header5);
            document.Add(header6);
            document.Add(header7);

            document.NewPage();

            #endregion

            #region current debt summary
            Paragraph para = new Paragraph(new Chunk("Current Debt Summary", FontFactory.GetFont("Arial", 24, Font.NORMAL)));
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            para.Alignment = 1; // for center
            para.Font = FontFactory.GetFont("Arial", 24, Font.BOLD);

            document.Add(para);

            var phrase1 = new Phrase();
            var phrase2 = new Phrase();
            var pharse3 = new Phrase();
            var pharse4 = new Phrase();
            var pharse5 = new Phrase();
            var pharse6 = new Phrase();
            var pharse7 = new Phrase();
            phrase1.Add(new Chunk("Based on the information that you provided us you will be in debt for the next ", subtitleFont));
            phrase1.Add(new Chunk(model.PayoffTime.ToString() + " Years.", subtitleFont));

            phrase2.Add(new Chunk(model.SavingSummary.CurrentTotalAmountPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), subtitleFont));
            phrase2.Add(new Chunk(" will leave your accounts in the next ", subtitleFont));
            phrase2.Add(new Chunk(model.PayoffTime.ToString() + " years", subtitleFont));
            phrase2.Add(new Chunk(" to pay off those debts, of which ", subtitleFont));
            phrase2.Add(new Chunk(model.SavingSummary.CurrentInterestPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), subtitleFont));
            phrase2.Add(new Chunk(" will go towards interest. You and your family",subtitleFont));
            phrase2.Add(new Chunk(" will never ", FontFactory.GetFont("Arial", 12, Font.BOLDITALIC)));
            phrase2.Add(new Chunk(" be able to spend or earn interest on any of these dollars again.", subtitleFont));

            pharse3.Add(new Chunk(" Your total real debt is ", subtitleFont));
            pharse3.Add(new Chunk(model.SavingSummary.CurrentTotalAmountPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")) + " not ", subtitleFont));
            pharse3.Add(new Chunk(model.SavingSummary.CurrentDebtBalance.ToString("C", CultureInfo.GetCultureInfo("en-US")), subtitleFont));
            pharse3.Add(new Chunk(" which is the debt balance of all your accounts.", subtitleFont));

            pharse4.Add(new Chunk("Unfortunately most people never get out of debt, because they do not have a system and they do not have the education to create good habits.  In fact as soon as they pay off one debt they acquire a new one with the recently “found” money.", subtitleFont));
            pharse5.Add(new Chunk("Typically the only money they have for retirement is in a 401k or other savings plan which in your case would be "+ model.CurrentSavingsTotal.ToString("C", CultureInfo.GetCultureInfo("en-US")), subtitleFont));
            pharse6.Add(new Chunk("If you are one of the very few who saved 100% of your previous payments after your debts were paid off you could potentially have an additional "+model.CPPaymentSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")) + " available for retirement.", subtitleFont));
            pharse7.Add(new Chunk("There is a better way which we have outlined for you here in this report.", subtitleFont));

            Paragraph para1 = new Paragraph(phrase1);
            para1.SpacingBefore = 10f;
            para1.SpacingAfter = 10f;
           // para1.SetLeading(0, 1.75f);
            document.Add(para1);
 
            Paragraph para2 = new Paragraph(phrase2);
            para2.SpacingAfter = 10f;
           // para2.SetLeading(0, 1.75f);
            document.Add(para2);

            Paragraph para3 = new Paragraph(pharse3);
            para3.SpacingAfter = 10f;
          //  para3.SetLeading(0, 1.75f);
            document.Add(para3);

            Paragraph Cppara4 = new Paragraph(pharse4);
            Cppara4.SpacingAfter = 10f;
         //   Cppara4.SetLeading(0, 1.75f);
            document.Add(Cppara4);

            Paragraph Cppara5 = new Paragraph(pharse5);
            Cppara5.SpacingAfter = 10f;
           // Cppara5.SetLeading(0, 1.75f);
            document.Add(Cppara5);

            Paragraph Cppara6 = new Paragraph(pharse6);
            Cppara6.SpacingAfter = 10f;
          //  Cppara6.SetLeading(0, 1.75f);
            document.Add(Cppara6);

            Paragraph Cppara7 = new Paragraph(pharse7);
            Cppara7.SpacingAfter = 10f;
          //  Cppara7.SetLeading(0, 1.75f);
            document.Add(Cppara7);

            byte[] byteImageDebtSummary = Convert.FromBase64String(DebtSummaryImageUrl);
            iTextSharp.text.Image imgDebtSummary = iTextSharp.text.Image.GetInstance(byteImageDebtSummary);
            imgDebtSummary.ScaleAbsolute(document.PageSize.Width, 230f);
            imgDebtSummary.IndentationLeft = 70f;
            imgDebtSummary.Alignment = 1;

            document.Add(imgDebtSummary);

            document.NewPage();
            #endregion

            #region AFW

            var phrase4 = new Phrase();
            var phrase5 = new Phrase();
            var phrase6 = new Phrase();
            var phrase7 = new Phrase();
            Paragraph paraCreditLoan = new Paragraph(new Chunk("Accelerated Family Wealth Plan vs. Current Plan", titleFont));
            paraCreditLoan.SpacingBefore = 20f;
            paraCreditLoan.SpacingAfter = 20f;
            paraCreditLoan.Alignment = 1;
            paraCreditLoan.Font = titleFont;

            double savingsDiff = model.FinalSavings - model.CurrentSavingsTotal;
            savingsDiff= savingsDiff > 0 ? savingsDiff : 0;

            phrase4.Add(new Chunk("The first option is to use a debt roll up strategy combined with a debt elimination budget, we call this the Accelerated Family Wealth plan.", subtitleFont));
            phrase5.Add(new Chunk("Most people who do this get out of debt sooner and establish some good saving and spending habits. However they still have to finance purchases or pay cash and thus will only save a portion of the previous payments and debt elimination budget when all their debts are paid off.", subtitleFont));
            phrase6.Add(new Chunk("You have selected to pay off your debt using the ", subtitleFont));
            phrase6.Add(new Chunk(model.PlanName, text));
            phrase6.Add(new Chunk(" method", subtitleFont));
            phrase7.Add(new Chunk("Using the Accelerated Family Wealth plan you can expect the following results: ", subtitleFont));


            List AFWlist = new List(List.ORDERED, 20f);

            AFWlist.SetListSymbol("\u2022");

            AFWlist.IndentationLeft = 20f;

       
            ListItem li = new ListItem();
            li.Add(new Chunk("Pay off all of the debts you selected in Step 1 in ", subtitleFont));
            li.Add(new Chunk(model.AFWPlanData.PayOffTime.ToString(), text));
            li.Add(new Chunk(" years.", subtitleFont));
            AFWlist.Add(li);

            ListItem li1 = new ListItem();
            li1.Add(new Chunk("Save a total of ", subtitleFont));
            li1.Add(new Chunk(model.AFWPlanData.InterestSaved.ToString("C", CultureInfo.GetCultureInfo("en-US")), text));
            li1.Add(new Chunk(" in interest.", subtitleFont));
            AFWlist.Add(li1);

            ListItem li2 = new ListItem();
            li2.Add(new Chunk("Have a savings account with approximately ", subtitleFont));
            li2.Add(new Chunk(model.AFWPlanData.TotalAmountToSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")), text));
            li2.Add(new Chunk(" in it when you retire in ", subtitleFont));
            li2.Add(new Chunk(model.YearsUntilRetirement.ToString(), text));
            li2.Add(new Chunk(" years. This is based on you saving 50% of the previous payments and debt elimination budget once your debts are paid off.  If you saved 100% of the previous payments and debt elimination budget you would have approximately ", subtitleFont));
            li2.Add(new Chunk(model.AFWPlanData.MaxAmountToSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")), text));
            li2.Add(new Chunk(" when you retire in ", subtitleFont));
            li2.Add(new Chunk(model.YearsUntilRetirement.ToString(), text));
            li2.Add(new Chunk(" years.", subtitleFont));
            AFWlist.Add(li2);
             

            Paragraph para4 = new Paragraph(phrase4);
            para4.SpacingAfter = 20f;
            para4.SetLeading(0, 1.75f);

            Paragraph para5 = new Paragraph(phrase5);
            para5.SpacingAfter = 20f;
            para1.SetLeading(0, 1.75f);

            Paragraph para6 = new Paragraph(phrase6);
            para6.SpacingAfter = 10f;
            para6.SetLeading(0, 1.75f);

            Paragraph para7 = new Paragraph(phrase7);
            para7.SpacingAfter = 10f;
            para7.SetLeading(0, 1.75f);

            Paragraph para8 = new Paragraph(new Chunk("The savings account value is estimated based on Current Savings Total, Monthly Savings/Retirement Contributions, 50% of the debt elimination budget and 50% of the previous payments minus any cost of insurance if any.", FontFactory.GetFont("Arial", 12, Font.NORMAL)));

            byte[] byteImage = Convert.FromBase64String(ChartImage);
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byteImage);
            img.ScaleAbsolute(document.PageSize.Width-10, 230f);
            img.IndentationLeft = 20f;
            img.Alignment = 1;

            document.Add(paraCreditLoan);
            document.Add(para4);
            document.Add(para5);
            document.Add(para6);
            document.Add(para7);
            document.Add(AFWlist);
            document.Add(img);
            document.Add(para8);
            document.NewPage();

            #endregion

            #region family banking system

            var FBSphrase1 = new Phrase();
            var FBSphrase2 = new Phrase();
            var FBSphrase3 = new Phrase();
            var FBSphrase4 = new Phrase();
            Paragraph paraFBS = new Paragraph(new Chunk("Family Banking System vs. Current Plan", titleFont));
            paraFBS.SpacingBefore = 20f;
            paraFBS.SpacingAfter = 20f;
            paraFBS.Alignment = 1;
            paraFBS.Font = titleFont;

            double savingsDiffFBS = model.FinalSavings - model.CurrentSavingsTotal;
            savingsDiff = savingsDiff > 0 ? savingsDiff : 0;

            FBSphrase1.Add(new Chunk("The second option is to use a combination of a debt roll up plan and a permanent life insurance policy specifically designed to accumulate the maximum amount of cash values, we call this the Family Banking System.", subtitleFont));
            FBSphrase2.Add(new Chunk("With the Family Banking System you develop good saving and spending habits and have a plan.  We find that most people will save or funnel 100% of their previous payments into the Family Banking System to build up a pool of capital to borrow against for future purchases.  This allows them to finance their own purchases and not have to borrow from the banks.", subtitleFont));
            FBSphrase3.Add(new Chunk("You have selected to pay off your debt using the ", subtitleFont));
            FBSphrase3.Add(new Chunk(model.PlanName, text));
            FBSphrase3.Add(new Chunk(" method", subtitleFont));
            FBSphrase4.Add(new Chunk("Using the Family Banking System you can expect the following results: ", subtitleFont));


            List FBSlist = new List(List.ORDERED, 20f);

            FBSlist.SetListSymbol("\u2022");

            FBSlist.IndentationLeft = 20f;

            ListItem li4 = new ListItem();
            li4.Add(new Chunk("Pay off all of the debts you selected in Step 1 in ", subtitleFont));
            li4.Add(new Chunk(model.FBSPlanData.PayOffTime.ToString(), text));
            li4.Add(new Chunk(" years.", subtitleFont));
            FBSlist.Add(li4);

            ListItem li5 = new ListItem();
            li5.Add(new Chunk("Save a total of ", subtitleFont));
            li5.Add(new Chunk(model.FBSPlanData.InterestSaved.ToString("C", CultureInfo.GetCultureInfo("en-US")), text));
            li5.Add(new Chunk(" in interest.", subtitleFont));
            FBSlist.Add(li5);

            ListItem li6 = new ListItem();
            li6.Add(new Chunk("Have a savings account with approximately ", subtitleFont));
            li6.Add(new Chunk(model.FBSPlanData.TotalAmountToSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")), text));
            li6.Add(new Chunk(" in it when you retire in ", subtitleFont));
            li6.Add(new Chunk(model.YearsUntilRetirement.ToString(), text));
            li6.Add(new Chunk(" years.", subtitleFont));
            FBSlist.Add(li6);
 

            Paragraph FBSpara1 = new Paragraph(FBSphrase1);
            FBSpara1.SpacingAfter = 20f;
            FBSpara1.SetLeading(0, 1.75f);

            Paragraph FBSpara2 = new Paragraph(FBSphrase2);
            FBSpara2.SpacingAfter = 20f;
            FBSpara2.SetLeading(0, 1.75f);

            Paragraph FBSpara3 = new Paragraph(FBSphrase3);
            FBSpara3.SpacingAfter = 10f;
            FBSpara3.SetLeading(0, 1.75f);

            Paragraph FBSpara4 = new Paragraph(FBSphrase4);
            FBSpara4.SpacingAfter = 10f;
            FBSpara4.SetLeading(0, 1.75f);

            Paragraph FBSpara5 = new Paragraph(new Chunk("The savings account value is estimated based on Current Savings Total, Monthly Savings/Retirement Contributions, 100% of the previous payments after all of your debts are paid off and any Cash Values of Life Insurance.", FontFactory.GetFont("Arial", 12, Font.NORMAL)));

            if (model.BudgetForFBS)
            {
                document.Add(paraFBS);
                document.Add(FBSpara1);
                document.Add(FBSpara2);
                document.Add(FBSpara3);
                document.Add(FBSpara4);
                document.Add(FBSlist);
                document.Add(img);
                document.Add(FBSpara5);
                document.NewPage();
            }

            #endregion

            string strbankingSystem = model.BudgetForFBS ? "Family Banking System" : "Accelerated Family Wealth";
            #region debt information
            Paragraph para9 = new Paragraph(new Chunk("Below is a list of the debts that you have selected to pay off using the "+ strbankingSystem + ".", subtitleFont));
            para9.SpacingAfter = 10f;
            para9.SetLeading(0, 1f);

            PdfPTable tableDebtInfo = new PdfPTable(5);

            PdfPCell cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Padding = 15f;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.Border = 1;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 1f;
            cell.BorderWidthTop = 1f;
            cell.BorderWidthRight = 1f;

            tableDebtInfo.WidthPercentage = 100;
            tableDebtInfo.SpacingBefore = 10f;
            tableDebtInfo.SpacingAfter = 12.5f;
            
            tableDebtInfo.AddCell(cell);
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Debt Information", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 5, BackgroundColor = new BaseColor(255, 0, 0),Padding=9f, BorderWidth = 2 });
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Creditor", text)) { HorizontalAlignment = Element.ALIGN_CENTER , Padding = 9f , BorderWidth =2});
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Balance", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Interest", text)) { HorizontalAlignment = Element.ALIGN_CENTER , Padding = 9f, BorderWidth = 2 });
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Payment", text)) { HorizontalAlignment = Element.ALIGN_CENTER , Padding = 9f, BorderWidth = 2 });
            tableDebtInfo.AddCell(new PdfPCell(new Phrase("Minimum Payment", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });

            foreach (var item in model.Debts)
            {
                tableDebtInfo.AddCell(new PdfPCell(new Phrase(item.Name, text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
                tableDebtInfo.AddCell(new PdfPCell(new Phrase(Math.Round(AccountingFormatter.CentsToDollars(Convert.ToInt32(item.BalanceInCents)), 2).ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0) , Padding = 9f, BorderWidth = 2 });
                tableDebtInfo.AddCell(new PdfPCell(new Phrase(Math.Round(item.InterestRatePerc, 2) + " %", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
                tableDebtInfo.AddCell(new PdfPCell(new Phrase(Math.Round(AccountingFormatter.CentsToDollars(Convert.ToInt32(item.ActualPayment)), 2).ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
                tableDebtInfo.AddCell(new PdfPCell(new Phrase(Math.Round(AccountingFormatter.CentsToDollars(Convert.ToInt32(item.MinMonthPaymentInCents)), 2).ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79) , Padding = 9f, BorderWidth = 2 });
            }

            document.Add(para9);
            document.Add(tableDebtInfo);
            #endregion

            #region debt comparision

            Paragraph para10= new Paragraph(new Chunk("Looking at the chart below you can see how all your debts will be paid off in "+model.PayTime+" years using the "+ strbankingSystem + " vs "+model.PayoffTime+" years.",subtitleFont));
            para10.SpacingAfter = 20f;
            para10.SpacingBefore = 20f;
            para10.SetLeading(0, 1f);

            PdfPTable tableComparisonDebt = new PdfPTable(5);

            PdfPCell cell1 = new PdfPCell();
            cell1.Colspan = 5;
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_CENTER;
            cell1.Padding = 15;
            tableComparisonDebt.WidthPercentage = 100;
            tableComparisonDebt.SpacingBefore = 10f;
            tableComparisonDebt.SpacingAfter = 12.5f;

            tableComparisonDebt.AddCell(cell1);
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Comparison of Payoff by Debt", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 5, BackgroundColor = new BaseColor(255, 0, 0),Padding= 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("")) { BorderWidth=2});
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Current Schedule", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 2, Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Accelerated Schedule", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 2, Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Creditor", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Months to PayOff", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Paid Off Date", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Months to PayOff", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
            tableComparisonDebt.AddCell(new PdfPCell(new Phrase("Paid Off Date", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            foreach (var item in model.ComparisonDebts)
            {
                tableComparisonDebt.AddCell(new PdfPCell(new Phrase(item.Creditor, text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
                tableComparisonDebt.AddCell(new PdfPCell(new Phrase(item.PayoffTime.ToString(), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
                tableComparisonDebt.AddCell(new PdfPCell(new Phrase(item.CurrentPayOffDate.ToString(), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
                tableComparisonDebt.AddCell(new PdfPCell(new Phrase(item.PayoffTimeViaPlan.ToString(), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
                tableComparisonDebt.AddCell(new PdfPCell(new Phrase(item.AcceleratedPayOffDate.ToString(), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
            }

            document.Add(para10);
            document.Add(tableComparisonDebt);
            document.NewPage();

            #endregion

            #region summary
            Paragraph para11 = new Paragraph(new Chunk("By following the "+ strbankingSystem + " you will save a total of " + model.SavingSummary.AcceleratedTotalInterestSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")) + " in interest and " + model.PayTime + " years.", subtitleFont));
            para11.SpacingAfter = 20f;
          //  para11.SetLeading(0, 1f); 

            Paragraph para12 = new Paragraph(new Chunk("You will also potentially will have an additional " + savingsDiff.ToString("C", CultureInfo.GetCultureInfo("en-US")) + " in savings when you retire in " + model.YearsUntilRetirement + " years.", subtitleFont));
            para12.SpacingAfter = 20f;
          //  para12.SetLeading(0, 1f);

            #endregion

            PdfPTable tableInterestTimeDebt = new PdfPTable(3);

            PdfPCell cell2 = new PdfPCell();
            cell2.Colspan = 3;
            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
            cell2.VerticalAlignment = Element.ALIGN_CENTER;
            cell2.Padding = 15;
            tableInterestTimeDebt.WidthPercentage = 100;
            tableInterestTimeDebt.SpacingBefore = 10f;
            tableInterestTimeDebt.SpacingAfter = 12.5f;

            tableInterestTimeDebt.AddCell(cell2);
            tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase("Interest & Time Saved Per Debt", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 3, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
            tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase("Creditor", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase("Interest Saved", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase("Time Saved", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });

            foreach (var item in model.SavingsDebts)
            {
                tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase(item.Creditor, text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
                tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase(Math.Round(item.MoneySaved).ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER , BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f,BorderWidth = 2 });
                tableInterestTimeDebt.AddCell(new PdfPCell(new Phrase(Math.Round(item.TimeSaved) + " months (" + Math.Round(item.TimeSaved / 12, 2) + " years )", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f , BorderWidth = 2 });
            }

            PdfPTable tableSummarySavings = new PdfPTable(3);

            PdfPCell cell3 = new PdfPCell();
            cell3.Colspan = 3;
            cell3.HorizontalAlignment = Element.ALIGN_CENTER;
            cell3.VerticalAlignment = Element.ALIGN_CENTER;
            cell3.Padding = 15;
            tableSummarySavings.WidthPercentage = 100;
            tableSummarySavings.SpacingBefore = 10f;
            tableSummarySavings.SpacingAfter = 12.5f;

            tableSummarySavings.AddCell(cell3);
            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Summary of Savings", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 3, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase("", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Current Schedule", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Accelerated Schedule", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Total Monthly Payments", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentTotalMonthlyPayments.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedTotalMonthlyPayments.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Debt Balance", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentDebtBalance.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER , BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedDebtBalance.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER , BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Interest Paid", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentInterestPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedInterestPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Real Total Debt", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentTotalAmountPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedTotalAmountPaid.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Debt Free", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentRetireDebt.ToString() + " months (" + Math.Round(model.SavingSummary.CurrentRetireDebt / 12, 2) + " years)", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedRetireDebt.ToString() + " months (" + Math.Round(model.SavingSummary.AcceleratedRetireDebt / 12, 2) + " years)", debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER , BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });

            tableSummarySavings.AddCell(new PdfPCell(new Phrase("Total Interest Savings", text)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.CurrentTotalInterestSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(255, 0, 0), Padding = 9f, BorderWidth = 2 });
            tableSummarySavings.AddCell(new PdfPCell(new Phrase(model.SavingSummary.AcceleratedTotalInterestSavings.ToString("C", CultureInfo.GetCultureInfo("en-US")), debtTableFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(106, 168, 79), Padding = 9f, BorderWidth = 2 });


            document.Add(para11);
            document.Add(para12);
            document.Add(tableSummarySavings);
            document.Add(tableInterestTimeDebt);
            
            
            document.Close();
            writer.Close();

            stream.Position = 0;
            return File(stream, "application/pdf");
        }


        #endregion
    }


    public class WealthDetailsItemShort
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public double Wealth { get; set; }

        public DateTime Date { get; set; }

    }
}