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
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.WebApi.Tenants.Controllers;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;

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
                var origDetails = _calculator.CalcOriginaldDetails(debt,true);
                var accDetails = _calculator.CalcAcceleratedDetails(debt,true);

                model.MonthlyBudget = Convert.ToInt32(AccountingFormatter.CentsToDollars(debt.MonthlyBudgetInCents)); //debt.MonthlyBudgetInCents;
                model.TotalPayed = origDetails.Sum(d => d.ActualPaymentInCents);
                model.TotalPayedViaPlan = accDetails.Sum(d => d.ActualPaymentInCents);
                model.PayoffTime = origDetails.Count > 0 ? Math.Round((origDetails.Last().Date - origDetails.First().Date).TotalDays / 365, 1) : 0;
                model.PayoffTimeViaPlan = accDetails.Count > 0 ? Math.Round((accDetails.Last().Date - accDetails.First().Date).TotalDays / 365, 1) : 0;
            }
            catch (ArgumentException)
            {
                model.WrongProgramParams = true;
            }

            return model;
        }


        public Step3Model GetDebtEliminationStep3Model(DebtEliminationDocument debt)
        {
         //   debt.MonthlyBudgetInCents = debt.MonthlyBudgetInCents - debt.AmountToSavings + debt.LumpSumAmount;   // Debt Elimination Changes

            int PlanID = (int)Enum.Parse(typeof(DebtEliminationPlanEnum), debt.PlanId.ToString());
            double deathBenefit= debt.MaxLoans.Count() > 0 ? debt.MaxLoans.LastOrDefault().OriginalDeathBenefit : 0;
            
            var origDetails = _calculator.CalcOriginaldDetails(debt, false);
            var accDetails = _calculator.CalcAcceleratedDetails(debt,true);
            var FBSDetails = _calculator.CalcFBSDetails(debt);

            var ErrorMessage = origDetails.Where(x => x.ErrorMessage != null).FirstOrDefault();
            if(ErrorMessage != null)
            {
               var modelError = new Step3Model();
                modelError.ErrorMessage = ErrorMessage.ErrorMessage;
                return modelError;
            }

            
             var model = new Step3Model
            {
                TotalPayed = origDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(origDetails.Sum(d => d.ActualPaymentInCents)) : 0,
                TotalPayedViaPlan = accDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.ActualPaymentInCents)): 0,
                 PayoffTime = origDetails.Count > 0 ? Math.Round(((((origDetails.Last().Date - origDetails.First().Date).TotalDays) + 30) / 365)*12, 0) : 0,
                 PayoffTimeViaPlan = accDetails.Count > 0 ? Math.Round(((((accDetails.Last().Date - accDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0,
                 DisplayMode = debt.DisplayMode,
                InterestPaid= origDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(origDetails.Sum(d => d.InterestPaymentInCents)): 0,
                InterestPaidViaPlan= accDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.InterestPaymentInCents)): 0,
                 TotalPayedViaFBS = FBSDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(FBSDetails.Sum(d => d.ActualPaymentInCents)) : 0,
                 InterestPaidViaFBS = FBSDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(FBSDetails.Sum(d => d.InterestPaymentInCents)) : 0,
                 PayoffTimeViaFBS = FBSDetails.Count > 0 ? Math.Round(((((FBSDetails.Last().Date - FBSDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0,
             };
            model.PayoffTime = Math.Round(model.PayoffTime / 12,2);
            model.PayoffTimeViaPlan = Math.Round(model.PayoffTimeViaPlan / 12 ,2);
            model.PayoffTimeViaFBS = Math.Round(model.PayoffTimeViaFBS / 12, 2);
            model.DeathBenefitTerminatesAge = debt.DeathBenefitTerminatesAge;
            model.MonthlySavingsContribution = (long)AccountingFormatter.CentsToDollars(debt.MonthlySavingsContribution); 
            model.CurrentDeathBenefit = (long)AccountingFormatter.CentsToDollars(debt.CurrentDeathBenefit);
            model.FBSDeathBenefit = debt.BudgetForFBS == true ? (long)deathBenefit: 0;
            model.BudgetForFBS = debt.BudgetForFBS;

            AFWPlanData objAFWPlan = new AFWPlanData();

            objAFWPlan.TotalPayed = accDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.ActualPaymentInCents)) : 0;
            objAFWPlan.PayOffTime = accDetails.Count > 0 ? Math.Round(((((accDetails.Last().Date - accDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
            objAFWPlan.AmountToSavings = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.AmountToSavings/2), 0);
            objAFWPlan.InterestPaid = accDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(accDetails.Sum(d => d.InterestPaymentInCents)) : 0;
            objAFWPlan.MoneySaved = model.TotalPayed - objAFWPlan.TotalPayed;
            objAFWPlan.YearsSaved = model.PayoffTime - objAFWPlan.PayOffTime;
            objAFWPlan.PayOffTime = Math.Round(objAFWPlan.PayOffTime / 12, 2);
            objAFWPlan.InterestSaved = model.InterestPaid - objAFWPlan.InterestPaid;

            FBSPlanData objFBSPlan = new FBSPlanData();

            if (FBSDetails.Count > 0)
            {
                objFBSPlan.TotalPayed = FBSDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(FBSDetails.Sum(d => d.ActualPaymentInCents)) : 0;
                objFBSPlan.PayOffTime = FBSDetails.Count > 0 ? Math.Round(((((FBSDetails.Last().Date - FBSDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                objFBSPlan.AmountToSavings = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.AmountToSavings - debt.MonthlyBudgetInCents), 0);
                objFBSPlan.InterestPaid = FBSDetails.Count > 0 ? (double)AccountingFormatter.CentsToDollars(FBSDetails.Sum(d => d.InterestPaymentInCents)) : 0;
                objFBSPlan.MoneySaved = model.TotalPayed - objFBSPlan.TotalPayed;
                objFBSPlan.YearsSaved = model.PayoffTime - objFBSPlan.PayOffTime;
                objFBSPlan.PayOffTime = Math.Round(objFBSPlan.PayOffTime / 12, 2);
                objFBSPlan.InterestSaved = model.InterestPaid - objFBSPlan.InterestPaid;
            }


            List<DebtEliminationComparisionModel> AFSDebts = new List<DebtEliminationComparisionModel>();

            foreach (var a in debt.Debts)
            {
                DebtEliminationComparisionModel debtData = new DebtEliminationComparisionModel();
                DebtEliminationComparisionModel AFSData = new DebtEliminationComparisionModel();
                debtData.Creditor = a.Name;
                debtData.PayoffTime = origDetails.Where(x => x.Id == a.DebtId).Count(x => x.Id == a.DebtId) > 0 ? Math.Round(((((origDetails.Where(x => x.Id == a.DebtId).Last().Date - origDetails.Where(x => x.Id == a.DebtId).First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                debtData.TotalPayed = (double)AccountingFormatter.CentsToDollars(origDetails.Where(x => x.Id == a.DebtId).Sum(d => d.ActualPaymentInCents));
                debtData.CurrentPayOffDate = origDetails.Where(x => x.Id == a.DebtId).Last().Date.ToString("MM/yyyy");
                debtData.InterestPaid = (double)AccountingFormatter.CentsToDollars(origDetails.Where(x => x.Id == a.DebtId).Sum(d => d.InterestPaymentInCents));
                debtData.MinMonthPayment = (double)AccountingFormatter.CentsToDollars(Convert.ToInt64(a.MinMonthPaymentInCents));

                AFSData.Creditor = a.Name;
                AFSData.PayoffTime = origDetails.Where(x => x.Id == a.DebtId).Count(x => x.Id == a.DebtId) > 0 ? Math.Round(((((origDetails.Where(x => x.Id == a.DebtId).Last().Date - origDetails.Where(x => x.Id == a.DebtId).First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                AFSData.TotalPayed = (double)AccountingFormatter.CentsToDollars(origDetails.Where(x => x.Id == a.DebtId).Sum(d => d.ActualPaymentInCents));
                AFSData.CurrentPayOffDate = origDetails.Where(x => x.Id == a.DebtId).Last().Date.ToString("MM/yyyy");
                AFSData.InterestPaid = (double)AccountingFormatter.CentsToDollars(origDetails.Where(x => x.Id == a.DebtId).Sum(d => d.InterestPaymentInCents));
                AFSData.MinMonthPayment = (double)AccountingFormatter.CentsToDollars(Convert.ToInt64(a.MinMonthPaymentInCents));
                AFSData.PayoffTimeViaPlan = accDetails.Where(x => x.Id == a.DebtId).Count() > 0 ? Math.Round(((((accDetails.Where(x => x.Id == a.DebtId).Last().Date - accDetails.Where(x => x.Id == a.DebtId).First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                AFSData.TotalPayedViaPlan = (double)AccountingFormatter.CentsToDollars(accDetails.Where(x => x.Id == a.DebtId).Sum(d => d.ActualPaymentInCents));
                AFSData.AcceleratedPayOffDate = accDetails.Where(x => x.Id == a.DebtId).Last().Date.ToString("MM/yyyy");
                AFSData.InterestPaidViaPlan = (double)AccountingFormatter.CentsToDollars(accDetails.Where(x => x.Id == a.DebtId).Sum(d => d.InterestPaymentInCents));
                AFSDebts.Add(AFSData);

                if (FBSDetails.Count > 0)
                {
                   
                    debtData.PayoffTimeViaPlan = FBSDetails.Where(x => x.Id == a.DebtId).Count() > 0 ? Math.Round(((((FBSDetails.Where(x => x.Id == a.DebtId).Last().Date - accDetails.Where(x => x.Id == a.DebtId).First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                    debtData.TotalPayedViaPlan = (double)AccountingFormatter.CentsToDollars(FBSDetails.Where(x => x.Id == a.DebtId).Sum(d => d.ActualPaymentInCents));
                    debtData.AcceleratedPayOffDate = FBSDetails.Where(x => x.Id == a.DebtId).Last().Date.ToString("MM/yyyy");
                    debtData.InterestPaidViaPlan = (double)AccountingFormatter.CentsToDollars(FBSDetails.Where(x => x.Id == a.DebtId).Sum(d => d.InterestPaymentInCents));
                    model.ComparisonDebts.Add(debtData);
                    a.ActualPayment = FBSDetails.Where(x => x.Id == a.DebtId).FirstOrDefault().ActualPaymentInCents;
                }
                else
                {
                    
                    debtData.PayoffTimeViaPlan = accDetails.Where(x => x.Id == a.DebtId).Count() > 0 ? Math.Round(((((accDetails.Where(x => x.Id == a.DebtId).Last().Date - accDetails.Where(x => x.Id == a.DebtId).First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;
                    debtData.TotalPayedViaPlan = (double)AccountingFormatter.CentsToDollars(accDetails.Where(x => x.Id == a.DebtId).Sum(d => d.ActualPaymentInCents));
                    debtData.AcceleratedPayOffDate = accDetails.Where(x => x.Id == a.DebtId).Last().Date.ToString("MM/yyyy");
                    debtData.InterestPaidViaPlan = (double)AccountingFormatter.CentsToDollars(accDetails.Where(x => x.Id == a.DebtId).Sum(d => d.InterestPaymentInCents));
                    model.ComparisonDebts.Add(debtData);
                    a.ActualPayment = accDetails.Where(x => x.Id == a.DebtId).FirstOrDefault().ActualPaymentInCents;
                }
                
            }
            model.SavingsDebts = model.ComparisonDebts.OrderByDescending(x => x.TimeSaved).ToList();
            model.ComparisonDebts = model.ComparisonDebts.OrderBy(x => DateTime.Parse(x.AcceleratedPayOffDate)).ToList();
            
            model.Debts = debt.Debts;
            if(PlanID == 1)
            {
                model.Debts = model.Debts.OrderBy(x => x.BalanceInCents).ToList();
            }
            else if(PlanID == 2)
            {
                model.Debts = model.Debts.OrderByDescending(x => x.InterestRatePerc).ToList();
            }
            else if (PlanID == 4)
            {
                model.Debts = model.Debts.OrderBy(d => d.BalanceInCents)
                            .ThenByDescending(d => d.ActualPayment).ToList();
            }


            System.Reflection.FieldInfo fi = debt.PlanId.GetType().GetField(debt.PlanId.ToString());

            System.ComponentModel.DescriptionAttribute[] attributes =
                (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(System.ComponentModel.DescriptionAttribute),
                false);
            model.PlanName = attributes[0].Description;
          
            model.SavingSummary = new DebtEliminationSavingsSummaryModel();
            model.SavingSummary.CurrentTotalAmountPaid = model.TotalPayed;
            model.SavingSummary.AcceleratedTotalAmountPaid = model.TotalPayedViaPlan;
            model.SavingSummary.CurrentRetireDebt = origDetails.Count > 0 ? Math.Round(((((origDetails.Last().Date - origDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;

            if (FBSDetails.Count > 0)
            {
                model.SavingSummary.AcceleratedRetireDebt = FBSDetails.Count > 0 ? Math.Round(((((FBSDetails.Last().Date - FBSDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;

            }
            else
            {
                model.SavingSummary.AcceleratedRetireDebt = accDetails.Count > 0 ? Math.Round(((((accDetails.Last().Date - accDetails.First().Date).TotalDays) + 30) / 365) * 12, 0) : 0;

            }
            model.SavingSummary.CurrentTotalMonthlyPayments = AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Debts.Sum(x => x.MinMonthPaymentInCents)));
            model.SavingSummary.AcceleratedTotalMonthlyPayments = AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Debts.Sum(x => x.MinMonthPaymentInCents)));
            model.SavingSummary.CurrentDebtBalance = AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Debts.Sum(x => x.BalanceInCents)));
            model.SavingSummary.AcceleratedDebtBalance = AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Debts.Sum(x => x.BalanceInCents)));
            
            if(FBSDetails.Count > 0)
            {
                model.SavingSummary.AcceleratedInterestPaid = model.InterestPaidViaFBS;
            }
            else
            {
                model.SavingSummary.AcceleratedInterestPaid = model.InterestPaidViaPlan;
            }
            model.SavingSummary.CurrentInterestPaid = model.InterestPaid;
            //model.DeathBenefit = deathBenefit;
            model.SavingSummary.CurrentTotalInterestSavings = 0;

            EstimatedInvestment estimatedInvestment = new EstimatedInvestment();
            
            foreach (var item in debt.Debts)
            {
                estimatedInvestment.MonthlyDepositInDollars += Convert.ToInt64(AccountingFormatter.CentsToDollars(Convert.ToInt64(item.MinMonthPaymentInCents)));
               
            }
            estimatedInvestment.MonthlyDepositInDollars = estimatedInvestment.MonthlyDepositInDollars + Convert.ToInt64(AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.MonthlyBudgetInCents)));

            // To Calculate the Accelarate Wealth after elimination of all debts.
            estimatedInvestment.EarningsRate = Convert.ToDouble(debt.EstimatedInvestmentEarningsRate / 100);
            estimatedInvestment.InvestmentTimeInYears = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) - model.PayoffTimeViaPlan),2);
            estimatedInvestment.CompoundingInterval = Convert.ToInt32(QuickSavingCompoundEnum.AnnualCompounded);
            model.EstimatedInvestmentEarningsRate = debt.EstimatedInvestmentEarningsRate;
            model.AmountToSavings =(long)Math.Round(AccountingFormatter.CentsToDollars(debt.AmountToSavings),0);
            debt.CurrentSavingsTotal = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.CurrentSavingsTotal), 2);
            debt.MonthlySavingsContribution = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.MonthlySavingsContribution), 2);

            double TotalAccelarteWealth = 0;
            double investedTimeInMonths = Math.Round((estimatedInvestment.InvestmentTimeInYears * 12), MidpointRounding.AwayFromZero);
            double InvestmentTimeInYearsAFW = estimatedInvestment.InvestmentTimeInYears;
            double investedTimeInMonthsAFW = investedTimeInMonths;
            var wealthId = 1;
            var wealthName = "Wealth";

            List<WealthDetailsItemShort> lstWealthDetailsItemShort = new List<Controllers.WealthDetailsItemShort>();

            WealthDetailsItemShort objWealthDetailsItemShort = new WealthDetailsItemShort();

            objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
            objWealthDetailsItemShort.Name = wealthName + wealthId;
            objWealthDetailsItemShort.Wealth = estimatedInvestment.MonthlyDepositInDollars;
            var investementStartDate = DateTime.Now.AddMonths(Convert.ToInt32(Math.Round(model.PayoffTimeViaPlan * 12)));
            objWealthDetailsItemShort.Date = investementStartDate;

            var nextInvestementCalulationDateNew = investementStartDate;
            lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);

            var nextInvestementCalulationDate = nextInvestementCalulationDateNew;

            var previoursInventmentValue = investementStartDate;

         //   int LastDebtPayoffMonth = Convert.ToInt32(model.ComparisonDebts.OrderByDescending(x => x.PayoffTimeViaPlan).FirstOrDefault().PayoffTimeViaPlan);

            int LastDebtPayoffMonth = Convert.ToInt32(AFSDebts.OrderByDescending(x => x.PayoffTimeViaPlan).FirstOrDefault().PayoffTimeViaPlan);

            long Term1AmountDEB = debt.Term1Amount, Term2AmountDEB = debt.Term2Amount;
            int Term1DEB = debt.Term1, Term2DEB = debt.Term2;


            if (debt.MonthlyBudgetInCents == 0)
            {
                LastDebtPayoffMonth = Convert.ToInt32(Math.Round(model.PayoffTimeViaPlan * 12, 0)) - LastDebtPayoffMonth;
                debt.Term1Amount = 0;
                debt.Term2Amount = 0;
            }

            if (debt.Term1 > LastDebtPayoffMonth)
            {
                debt.Term1 = debt.Term1 - LastDebtPayoffMonth;
            }
            if (debt.Term2 > LastDebtPayoffMonth && debt.Term1 == 0)
            {
                debt.Term2 = debt.Term2 - LastDebtPayoffMonth;
            }

            debt.AmountToSavings = Convert.ToInt64(AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.AmountToSavings)));
            debt.Term1Amount = Convert.ToInt64(AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Term1Amount)));
            debt.Term2Amount = Convert.ToInt64(AccountingFormatter.CentsToDollars(Convert.ToInt64(debt.Term2Amount)));
            long Term1Amount = debt.Term1Amount, Term2Amount = debt.Term2Amount;
            int Term1 = debt.Term1, Term2 = debt.Term2;
            var SavingsStartDate = accDetails.Last().Date.AddMonths(1);

            while (investedTimeInMonths > 0 && debt.AmountToSavings > 0)
            {
                
                long amtToSavings = debt.AmountToSavings - (debt.Term1 > 0 ? debt.AmountToSavings >= debt.Term1Amount ? debt.Term1Amount : 0 : debt.Term2 > 0 ? debt.AmountToSavings >= debt.Term2Amount ? debt.Term2Amount : 0: 0);
                TotalAccelarteWealth += Math.Round(amtToSavings * (Math.Pow((1 + (Math.Round(estimatedInvestment.EarningsRate,2) / estimatedInvestment.CompoundingInterval)), (estimatedInvestment.InvestmentTimeInYears * estimatedInvestment.CompoundingInterval))), 2);
               
                investedTimeInMonths--;
                estimatedInvestment.InvestmentTimeInYears = Math.Round((investedTimeInMonths / 12), 2);
               // SavingsStartDate = SavingsStartDate.AddMonths(1);

                objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                objWealthDetailsItemShort.Name = wealthName + wealthId;

                var dateDiff = AccountingFormatter.GetMonthsBetween(SavingsStartDate, previoursInventmentValue);
                if (dateDiff == 12)
                {
                    objWealthDetailsItemShort = new WealthDetailsItemShort();
                    objWealthDetailsItemShort.Id = Convert.ToString(wealthId);
                    objWealthDetailsItemShort.Name = wealthName + wealthId;
                    objWealthDetailsItemShort.Wealth = TotalAccelarteWealth;
                    objWealthDetailsItemShort.Date = SavingsStartDate;
                    lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);
                    previoursInventmentValue = SavingsStartDate;
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
                        objWealthDetailsItemShort.Date = SavingsStartDate;
                        lstWealthDetailsItemShort.Add(objWealthDetailsItemShort);
                    }
                }
                debt.Term2 = debt.Term1 == 0 ? debt.Term2 > 0 ? debt.Term2 - 1 : 0 : debt.Term2;
                debt.Term1 = debt.Term1 > 0 ? debt.Term1 - 1 : 0;
                
            }

            // For AFW
           
            double TotalAccelarteWealthAFW = 0;
            while (investedTimeInMonthsAFW > 0 && objAFWPlan.AmountToSavings > 0)
            {
                long amtToSavings = objAFWPlan.AmountToSavings - (Term1 > 0 ? objAFWPlan.AmountToSavings >= Term1Amount ? Term1Amount : 0 : Term2 > 0 ? objAFWPlan.AmountToSavings >= Term2Amount ? Term2Amount : 0 : 0);
                TotalAccelarteWealthAFW += Math.Round(amtToSavings * (Math.Pow((1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval)), (InvestmentTimeInYearsAFW * estimatedInvestment.CompoundingInterval))), 2);

                investedTimeInMonthsAFW--;
                InvestmentTimeInYearsAFW = Math.Round((investedTimeInMonthsAFW / 12), 2);
                Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                Term1 = Term1 > 0 ? Term1 - 1 : 0;
            }


            //For FBS
            double investedTimeInYearsFBS = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) - model.PayoffTimeViaFBS), 2);
            double investedTimeInMonthsFBS = Math.Round((investedTimeInYearsFBS * 12), MidpointRounding.AwayFromZero);
            double TotalAccelarteWealthFBS = 0;
            while (investedTimeInMonthsFBS > 0 && debt.AmountToSavings > 0)
            {
                long monthlyBudget = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.MonthlyBudgetInCents), 0);

                long amtToSavings = debt.AmountToSavings - monthlyBudget;
                TotalAccelarteWealthFBS += Math.Round(amtToSavings * (Math.Pow((1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval)), (investedTimeInYearsFBS * estimatedInvestment.CompoundingInterval))), 2);

                investedTimeInMonthsFBS--;
                investedTimeInYearsFBS = Math.Round((investedTimeInMonthsFBS / 12), 2);
                  
            }

            

            //according to yearly calculation
            //double TotalCurrentSavings = 0;
            //if (debt.CurrentSavingsTotal > 0)
            //{
            //    TotalCurrentSavings = Math.Round(debt.CurrentSavingsTotal * (Math.Pow((1 + (Math.Round(estimatedInvestment.EarningsRate, 2))), (debt.YearsUntilRetirement))), 2);
            //}

            //according to monthly calculation
            double CSTimeInYears = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement)), 2);
            double CSTimeInMonths = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) * 12), MidpointRounding.AwayFromZero);
            double TotalCurrentSavings = 0;
           
            var power = estimatedInvestment.EarningsRate > 0 ? (Math.Pow(1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval), (CSTimeInMonths)) - 1) / (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval) : 0;
            var b = 1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval);

            TotalCurrentSavings = debt.CurrentSavingsTotal * (Math.Pow((1 + (estimatedInvestment.EarningsRate / estimatedInvestment.CompoundingInterval)), (debt.YearsUntilRetirement * estimatedInvestment.CompoundingInterval)));

            double TotalAmountForMSC = 0;
            TotalAmountForMSC = debt.MonthlySavingsContribution * power * b;


            // For DEB
            double DebtEliminationBudgetTotal = 0;
            var xDebtEliminationBudgetTotal = (long)Math.Round(AccountingFormatter.CentsToDollars(debt.MonthlyBudgetInCents), 2) * power * b;
            double InvestmentTimeInYearsDEB = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement)), 2);
            double investedTimeInMonthsDEB= Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) * 12), MidpointRounding.AwayFromZero);
            long DEBAmount = debt.MonthlyBudgetInCents;
            if(DEBAmount > 0)
            {
                double term1Month = investedTimeInMonthsDEB <= Term1DEB? investedTimeInMonthsDEB: Term1DEB;
                double term2Month = investedTimeInMonthsDEB - term1Month <= 0 ? 0 : investedTimeInMonthsDEB - term1Month >= Term2DEB ? Term2DEB : investedTimeInMonthsDEB - term1Month;

                var Term1power = estimatedInvestment.EarningsRate > 0 ? (Math.Pow(1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval), (term1Month)) - 1) / (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval) : 0;
                var Term2power = estimatedInvestment.EarningsRate > 0 ? (Math.Pow(1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval), (term2Month)) - 1) / (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval) : 0;

                long term1Amount = DEBAmount >= Term1AmountDEB ? DEBAmount - Term1AmountDEB : 0;
               
                DebtEliminationBudgetTotal += term1Amount * Term1power * b;

                long term2Amount = DEBAmount >= Term2AmountDEB ? DEBAmount - Term2AmountDEB : 0;
                DebtEliminationBudgetTotal += term2Amount * Term2power * b;

                double remainingMonthsDEB = investedTimeInMonthsDEB - term1Month - term2Month;
                if (remainingMonthsDEB > 0)
                {
                    var remainingpower = estimatedInvestment.EarningsRate > 0 ? (Math.Pow(1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval), (remainingMonthsDEB)) - 1) / (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval) : 0;
                    DebtEliminationBudgetTotal += DEBAmount * remainingpower * b;
                }

            }

            DebtEliminationBudgetTotal = Math.Round(DebtEliminationBudgetTotal/100, 2);
            var CurrentSavingsTotal = TotalCurrentSavings;

            var LastCashSurrenderValue = debt.MaxLoans.Count > 0 ? debt.MaxLoans.OrderByDescending(x=> x.Year).FirstOrDefault().CashSurrenderValue : 0;
            
           // model.TotalEstimatedInvestment = Math.Round(TotalAccelarteWealth, 2) + CurrentSavingsTotal + TotalAmountForMSC;
            model.TotalEstimatedInvestment = TotalAccelarteWealthAFW + CurrentSavingsTotal + TotalAmountForMSC;
            model.TotalEstimatedInvestment = model.TotalEstimatedInvestment> 0 ? Math.Round(model.TotalEstimatedInvestment, 0): 0;

            objAFWPlan.TotalAmountToSavings = TotalAccelarteWealthAFW + CurrentSavingsTotal + TotalAmountForMSC;
            objAFWPlan.MaxAmountToSavings = TotalAccelarteWealth +CurrentSavingsTotal + TotalAmountForMSC;


            model.YearsUntilRetirement = debt.YearsUntilRetirement;

            List<double> savingsFromCurrentDebt = new List<double>();
                
            foreach (var Currentdebts in model.ComparisonDebts)
            {
                var minPayment = Math.Round(Currentdebts.MinMonthPayment,0);
                double TimeInMonths = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) * 12), MidpointRounding.AwayFromZero);
                double months = TimeInMonths - Currentdebts.PayoffTime;
                double TotalAmount = 0;
                
                var powerValue = estimatedInvestment.EarningsRate > 0 ? (Math.Pow(1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval), (months)) - 1) / (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval) : 0;
                var b1 = 1 + (Math.Round(estimatedInvestment.EarningsRate, 2) / estimatedInvestment.CompoundingInterval);
                TotalAmount = minPayment * powerValue * b1;
                TotalAmount = TotalAmount > 0 ?  TotalAmount: 0;


                savingsFromCurrentDebt.Add(TotalAmount);
            }

            double InvestmentsTimeInMonths = Math.Round((Convert.ToDouble(debt.YearsUntilRetirement) * 12), MidpointRounding.AwayFromZero);

            //model.CurrentSavingsTotal = Math.Round(savingsFromCurrentDebt.Sum(),2) + CurrentSavingsTotal+ TotalAmountForMSC + DebtEliminationBudgetTotal;
            model.CPPaymentSavings = Math.Round(savingsFromCurrentDebt.Sum(), 2);
            model.CurrentSavingsTotal = CurrentSavingsTotal + TotalAmountForMSC + DebtEliminationBudgetTotal;
            model.CurrentSavingsTotal = model.CurrentSavingsTotal> 0 ? Math.Round(model.CurrentSavingsTotal, 0): 0;

            model.FBSSavings = debt.BudgetForFBS == true ?  (Math.Round(TotalAccelarteWealthFBS, 2) + CurrentSavingsTotal + (debt.MonthlyContributionFBS == true ? 0: TotalAmountForMSC) + LastCashSurrenderValue) : 0;
            model.FBSSavings = model.FBSSavings > 0 ? Math.Round(model.FBSSavings, 0): 0;
            objFBSPlan.TotalAmountToSavings = model.FBSSavings;

            model.PayTime = FBSDetails.Count > 0 ? model.PayoffTimeViaFBS : model.PayoffTimeViaPlan;
            model.FinalInterestSaved = FBSDetails.Count > 0 ? model.InterestSavedFBS > 0 ? model.InterestSavedFBS : 0 : model.InterestSaved > 0 ? model.InterestSaved : 0;
            model.FinalSavings = FBSDetails.Count > 0 ? model.FBSSavings : model.TotalEstimatedInvestment;

            model.AFWPlanData = objAFWPlan;
            model.FBSPlanData = objFBSPlan;

            if (FBSDetails.Count(x => x.Debt == "Policy Loan Payback") > 0)
            {
                var policyPayback = FBSDetails.Where(x => x.Debt == "Policy Loan Payback").FirstOrDefault();
                DebtItemData debtData = new DebtItemData();
                debtData.Name = policyPayback.Debt;
                debtData.MinMonthPaymentInCents = policyPayback.MinPaymentInCents;
                debtData.InterestRatePerc =(float)debt.LoanInterestRate;
                debtData.BalanceInCents = policyPayback.BalanceInCents;
                debtData.ActualPayment = policyPayback.MinPaymentInCents;
                model.Debts.Add(debtData);
            }

            var displayModes = ((DebtEliminationDisplayModeEnum[])Enum.GetValues(typeof(DebtEliminationDisplayModeEnum))).ToDictionary(dm => (int)dm, dm => dm.GetDescription());
            model.DisplayModes = new SelectList(displayModes, "Key", "Value", (int)debt.DisplayMode);
            model.AddedToCalendar = debt.AddedToCalendar;
            switch (debt.DisplayMode)
            {
                case DebtEliminationDisplayModeEnum.Program:
                    if (FBSDetails.Count > 0)
                    {
                        model.Details = FBSDetails;
                    }
                    else
                    {
                        model.Details = accDetails;
                    }
                    //  model.Chart = GenerateChart(model.Details, null, lstWealthDetailsItemShort);
                    //  model.InterestPaidChart = ChartInterestPaid(model);
                    //  model.DebtFreeChart = ChartDebtFree(model);
                    break;
                case DebtEliminationDisplayModeEnum.Minimums:
                        model.Details = origDetails;
                    //model.Chart = GenerateChart(model.Details, null, lstWealthDetailsItemShort);
                    //model.InterestPaidChart = ChartInterestPaid(model);
                    //model.DebtFreeChart = ChartDebtFree(model);
                    break;
                case DebtEliminationDisplayModeEnum.ProgramAndMinimums:
                    if (FBSDetails.Count > 0)
                    {
                        model.Details = FBSDetails;
                    }
                    else
                    {
                        model.Details = accDetails;
                    }
                    //model.Chart = GenerateChart(origDetails, accDetails, lstWealthDetailsItemShort);
                    //model.InterestPaidChart = ChartInterestPaid(model);
                    //model.DebtFreeChart = ChartDebtFree(model);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return model;
        }

        private List<ChartItems> ChartDebtFree(Step3Model model)
        {
            model.DebtFreeChart.Add(new ChartItems(model.PayoffTimeViaPlan ,"Debt", "#6AA84F"));
            model.DebtFreeChart.Add(new ChartItems(model.PayoffTime, "Debt","#FF0000"));

            return model.DebtFreeChart;
        }

    
        private List<ChartItems> ChartInterestPaid(Step3Model model)
        {
            model.InterestPaidChart.Add(new ChartItems(model.InterestPaidViaPlan, model.InterestPaidViaPlan.ToString(), "#6AA84F"));
            model.InterestPaidChart.Add(new ChartItems(model.InterestPaid, model.InterestPaid.ToString(), "#FF0000"));

            model.InterestPaidChart.Add(new ChartItems(model.SavingSummary.AcceleratedTotalInterestSavings, model.SavingSummary.AcceleratedTotalInterestSavings.ToString(), "#6AA84F"));
            model.InterestPaidChart.Add(new ChartItems(model.SavingSummary.CurrentTotalInterestSavings, model.SavingSummary.CurrentTotalInterestSavings.ToString(), "#FF0000"));

            return model.InterestPaidChart;
        }
        private ChartViewModel GenerateChartNew(Step3Model model)
        {

            var chartViewModel = new ChartViewModel
            {
                XKey = "y",
                Data = new List<dynamic>(),
                Labels = new List<string>(),
                YKeys = new List<string>(),
                gridLines = false
            };

  
          
                chartViewModel.Data.Add(new ExpandoObject());
            
                    ((IDictionary<string, object>)chartViewModel.Data[0]).Add("y", "Debt");
                

            AddSeriesNew(chartViewModel, model);
            return chartViewModel;
        }

        private static void AddSeriesNew(ChartViewModel chartViewModel,Step3Model model, int j = 0)
        {
            for (; j < 2; j++)
            {
   
                    ((IDictionary<string, object>)chartViewModel.Data[0]).Add($"x_{j}", model.PayoffTimeViaPlan);
                chartViewModel.YKeys.Add($"x_{j}");
                ((IDictionary<string, object>)chartViewModel.Data[0]).Add($"x_{++j}", model.PayoffTime);
                chartViewModel.YKeys.Add($"x_{j}");

                //((IDictionary<string, object>)chartViewModel.Data[0]).Add($"x_{++j}", model.InterestPaidViaPlan);
                //chartViewModel.YKeys.Add($"x_{j}");
                //((IDictionary<string, object>)chartViewModel.Data[1]).Add($"x_{++j}", model.InterestPaid);
                //chartViewModel.YKeys.Add($"x_{j}");

                chartViewModel.Labels.Add("");
                 j++;
            }
        }
        private ChartViewModel GenerateChart(IList<ProgramDetailsItemShort> details1,
            IList<ProgramDetailsItemShort> details2 = null, List<WealthDetailsItemShort> details3 = null)
        {
            var postfix1 = string.Empty;
            var postfix2 = string.Empty;
            var postfix3 = string.Empty;
            var minDate = details1.Count > 0 ? details1[0].Date : DateTime.Now;
            var maxDate = details1.Count > 0 ? details1.Last().Date : DateTime.Now;

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

            if (details3 != null)
            {
                var minDate3 = details3.Count > 0 ? details3[0].Date : DateTime.Now;
                var maxDate3 = details3.Count > 0 ? details3.Last().Date : DateTime.Now;

                for (var i = minDate3; i <= maxDate3; i = i.AddYears(1))
                {
                    dates3.Add(i, i.ToString("MM/yy"));
                    chartViewModel.Data.Add(new ExpandoObject());
                }
                if (details3.Count > 1)
                {
                    dates3.Add(maxDate3, maxDate3.ToString("MM/yy"));
                    chartViewModel.Data.Add(new ExpandoObject());
                }
            }
            string result = "";
            result = AddSeries(chartViewModel, details1, postfix1, dates);
            if (details2 != null)
            {
                result = AddSeries(chartViewModel, details2, postfix2, dates, details1.Count);
            }
            var j = 0;
            if (result == "debts")
            {
                foreach (var date in dates.Keys)
                {
                    ((IDictionary<string, object>)chartViewModel.Data[j]).Add("y", date.ToString("yyyy-MM"));
                    j++;
                }
            }
            if (details3 != null)
            {
                result = AddSeries(chartViewModel, details3, postfix3, dates3, j);
            }

            if (result == "wealth")
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
            return "debts";
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
            return "wealth";
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
                if (lastMonthIncome > 0) { model.MonthlyGrossIncome = AccountingFormatter.CentsToDollars(lastMonthIncome); }

                var lastMonthDebt = _businessService.GetLedgerDebtsBalanceForLastMonth(ledger);
                if (lastMonthDebt > 0) { model.TotalMonthlyDebt = AccountingFormatter.CentsToDollars(lastMonthDebt); }
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
            chartViewModel.YKeys = new List<string> { "x_a", "x_b", "x_c", "x_d", "x_e", "x_f" };
            chartViewModel.XKey = "y";
            return chartViewModel;
        }

        #endregion
    }
}