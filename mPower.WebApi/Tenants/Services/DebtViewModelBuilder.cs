using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Default.ViewModel.Areas.Finance.DebtElimninationProgramController;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using Default.ViewModel.Areas.Finance.MortgageAcceleration;
using mPower.Documents;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Services
{
    public class DebtViewModelBuilder
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly BusinessReportDocumentService _businessService;
        private readonly DebtCalculator _calculator;

        public DebtViewModelBuilder(LedgerDocumentService ledgerService, 
            BusinessReportDocumentService businessService,
            DebtCalculator calculator)
        {
            _ledgerService = ledgerService;
            _businessService = businessService;
            _calculator = calculator;
        }

        #region Debt Elimination

        public DebtEliminationShortModel GetDebtEliminationShortModel(DebtEliminationDocument debt)
        {
            var model = new DebtEliminationShortModel();
            try
            {
                var origDetails = _calculator.CalcOriginaldDetails(debt);
                var accDetails = _calculator.CalcAcceleratedDetails(debt);

                model.MonthlyBudget = debt.MonthlyBudgetInCents;
                model.TotalPayed = origDetails.Sum(d => d.ActualPaymentInCents);
                model.TotalPayedViaPlan = accDetails.Sum(d => d.ActualPaymentInCents);
                model.PayoffTime = origDetails.Count > 0 ? Math.Round((origDetails.Last().Date - origDetails.First().Date).TotalDays/365, 1) : 0;
                model.PayoffTimeViaPlan = accDetails.Count > 0 ? Math.Round((accDetails.Last().Date - accDetails.First().Date).TotalDays/365, 1) : 0;
            }
            catch (ArgumentException)
            {
                model.WrongProgramParams = true;
            }

            return model;
        }

        public Step3Model GetDebtEliminationStep3Model(DebtEliminationDocument debt)
        {
            var origDetails = _calculator.CalcOriginaldDetails(debt);
            var accDetails = _calculator.CalcAcceleratedDetails(debt);

            var model = new Step3Model
            {
                TotalPayed = (double) AccountingFormatter.CentsToDollars(origDetails.Sum(d => d.ActualPaymentInCents)),
                TotalPayedViaPlan = (double) AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.ActualPaymentInCents)),
                PayoffTime = origDetails.Count > 0 ? Math.Round((origDetails.Last().Date - origDetails.First().Date).TotalDays/365, 1) : 0,
                PayoffTimeViaPlan = accDetails.Count > 0 ? Math.Round((accDetails.Last().Date - accDetails.First().Date).TotalDays/365, 1) : 0,
                DisplayMode = debt.DisplayMode
            };

            var displayModes = ((DebtEliminationDisplayModeEnum[])Enum.GetValues(typeof(DebtEliminationDisplayModeEnum))).ToDictionary(dm => (int)dm, dm => dm.GetDescription());
            model.DisplayModes = new SelectList(displayModes, "Key", "Value", (int)debt.DisplayMode);
            model.AddedToCalendar = debt.AddedToCalendar;

            switch (debt.DisplayMode)
            {
                case DebtEliminationDisplayModeEnum.Program:
                    model.Details = accDetails;
                    model.Chart = GenerateChart(model.Details);
                    break;
                case DebtEliminationDisplayModeEnum.Minimums:
                    model.Details = origDetails;
                    model.Chart = GenerateChart(model.Details);
                    break;
                case DebtEliminationDisplayModeEnum.ProgramAndMinimums:
                    model.Details = accDetails;
                    model.Chart = GenerateChart(origDetails, accDetails);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return model;
        }

        private ChartViewModel GenerateChart(IList<ProgramDetailsItemShort> details1,
            IList<ProgramDetailsItemShort> details2 = null)
        {
            var postfix1 = string.Empty;
            var postfix2 = string.Empty;
            var minDate = details1.Count > 0 ? details1[0].Date : DateTime.Now;
            var maxDate = details1.Count > 0 ? details1.Last().Date : DateTime.Now;
            if (details2 != null)
            {
                postfix1 = " (Minimums)";
                postfix2 = " (Program)";

                if (details2.Count > 0)
                {
                    var minDate2 = details2[0].Date;
                    var maxDate2 = details2.Last().Date;

                    minDate = minDate < minDate2 ? minDate : minDate2;
                    maxDate = maxDate > maxDate2 ? maxDate : maxDate2;
                }

            }

            var dates = new Dictionary<DateTime, string>();
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
            AddSeries(chartViewModel, details1, postfix1, dates);
            if (details2 != null)
            {
                AddSeries(chartViewModel, details2, postfix2, dates, details1.Count);
            }


            var j = 0;
            foreach (var date in dates.Keys)
            {
                ((IDictionary<string, object>)chartViewModel.Data[j]).Add("y", date.ToString("yyyy-MM"));
                j++;
            }

            return chartViewModel;
        }

        private static void AddSeries(ChartViewModel chartViewModel, IEnumerable<ProgramDetailsItemShort> details, string postfix, Dictionary<DateTime, string> dates, int j = 0)
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

        #endregion

        #region Debt To Income Ratio

        public DebtToIncomeRatioModel GetDebtToIncomeRatioModel(DebtEliminationDocument debt, bool forDashboard = false)
        {
            var model = new DebtToIncomeRatioModel();
            model.IsDebtToIncomeCalculatedBefore = debt.IsDebtToIncomeCalculatedBefore;
            if (!debt.IsDebtToIncomeCalculatedBefore)
            {
                var ledger = _ledgerService.GetById(debt.LedgerId);

                var lastMonthIncome = _businessService.GetIncomeForLastMonth(ledger);
                if (lastMonthIncome > 0) {model.MonthlyGrossIncome = AccountingFormatter.CentsToDollars(lastMonthIncome);}

                var lastMonthDebt = _businessService.GetLedgerDebtsBalanceForLastMonth(ledger);
                if (lastMonthDebt > 0) {model.TotalMonthlyDebt = AccountingFormatter.CentsToDollars(lastMonthDebt);}
            }
            else
            {
                model.MonthlyGrossIncome = AccountingFormatter.CentsToDollars(debt.DebtToIncomeRatio.MonthlyGrossIncomeInCents);
                model.TotalMonthlyDebt = AccountingFormatter.CentsToDollars(debt.DebtToIncomeRatio.TotalMonthlyDebtInCents);
                model.TotalMonthlyPitia = AccountingFormatter.CentsToDollars(debt.DebtToIncomeRatio.TotalMonthlyPitiaInCents);
                model.TotalMonthlyRent = AccountingFormatter.CentsToDollars(debt.DebtToIncomeRatio.TotalMonthlyRentInCents);
                model.DebtToIncomeRatio = debt.DebtToIncomeRatio.DebtToIncomeRatioString;
                BuildCharts(model, forDashboard);
            }

            return model;
        }

        private static void BuildCharts(DebtToIncomeRatioModel model, bool forDashboard)
        {
            var leftChartDebt = (double)(model.TotalMonthlyRent + model.TotalMonthlyPitia);
            model.LeftChart = new List<ChartItem>
            {
                new ChartItem(leftChartDebt, "Debt"),
                new ChartItem(((double)model.MonthlyGrossIncome - leftChartDebt), "")   
            };

            var rightChartDebt = (double)(model.TotalMonthlyRent + model.TotalMonthlyPitia + model.TotalMonthlyDebt);
            model.RightChart = new List<ChartItem>
            {
                new ChartItem(rightChartDebt, "Debt"),
                new ChartItem(((double)model.MonthlyGrossIncome - rightChartDebt), "")  
            };
        }

        #endregion

        #region Mortgage Acceleration Program

        public MortgageProgramModel GetMortgageProgramModel(DebtEliminationDocument debt, string programId, bool forDashboard = false)
        {
            var program = debt.MortgagePrograms.Find(mp => mp.Id == programId);
            if (program != null)
            {
                var programModel = new MortgageProgramModel
                {
                    Id = programId,
                    Title = program.Title,
                    LoanAmountInDollars = AccountingFormatter.CentsToDollars(program.LoanAmountInCents),
                    InterestRatePerYear = program.InterestRatePerYear,
                    LoanTermInYears = program.LoanTermInYears,
                    PaymentPeriod = program.PaymentPeriod,
                    ExtraPaymentInDollarsPerPeriod = AccountingFormatter.CentsToDollars(program.ExtraPaymentInCentsPerPeriod),
                    DisplayResolution = program.DisplayResolution,
                    OriginalParams = program.OriginalParams,
                    AcceleratedParams = program.AcceleratedParams,
                    Details = BuildProgramDetailsModel(program, forDashboard),
                    AddedToCalendar = program.AddedToCalendar,
                };

                return programModel;
            }



            return new MortgageProgramModel();
        }

        private ProgramDetailsModel BuildProgramDetailsModel(MortgageAccelerationProgramDocument program, bool forDashboard)
        {
            var detalizationModel = new ProgramDetailsModel();
            var detailsForGrid = program.Details.Where(d => d.ShowInGrid).ToList();
            if (detailsForGrid.Count > 0)
            {
                if (program.DisplayResolution == DisplayResolutionEnum.Low)
                {
                    detalizationModel.Items.Add(program.Details[0]);
                    if (detailsForGrid.Count > 1)
                    {
                        detalizationModel.Items.Add(detailsForGrid.Last());
                    }
                }
                else
                {
                    detalizationModel.Items = detailsForGrid;
                }

                detalizationModel.Chart = BuildChartViewModel(program);
            }

            return detalizationModel;
        }

        private ChartViewModel BuildChartViewModel(MortgageAccelerationProgramDocument program)
        {
            var chartViewModel = new ChartViewModel
            {
                Data = new List<dynamic>()
            };
            var graphedPayments = program.Details.Where(d => d.ShowInGraph);
            foreach (var payment in graphedPayments)
            {
                chartViewModel.Data.Add(new
                {
                    x_a = (double)AccountingFormatter.CentsToDollars(payment.AccBalanceInCents),
                    x_b = (double)AccountingFormatter.CentsToDollars(payment.AccTotalPaymentsInCents),
                    x_c = (double)AccountingFormatter.CentsToDollars(payment.AccTotalInterestInCents),
                    x_d = (double)AccountingFormatter.CentsToDollars(payment.OrigBalanceInCents),
                    x_e = (double)AccountingFormatter.CentsToDollars(payment.OrigTotalPaymentsInCents),
                    x_f = (double)AccountingFormatter.CentsToDollars(payment.OrigTotalInterestInCents),
                    y = payment.Date.ToString("yyyy-MM")
                });
            }
            chartViewModel.Labels = new List<string>
            {
                "Balance (Orig)",
                "Total Payment (Orig)",
                "Total Interest (Orig)",
                "Balance (Accel)",
                "Total Payment (Accel)",
                "Total Interest (Accel)"
            };
            chartViewModel.YKeys = new List<string> {"x_a", "x_b", "x_c", "x_d", "x_e", "x_f"};
            chartViewModel.XKey = "y";
            return chartViewModel;
        }

        #endregion
    }
}