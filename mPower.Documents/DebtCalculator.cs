using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;

namespace mPower.Documents
{
    public class DebtCalculator
    {

        public List<ProgramDetailsItemShort> CalcOriginaldDetails(DebtEliminationDocument debtProgram, bool isDebtCalculation = false, bool isQuickSavings = false)
        {
            return CalcRankedPayments(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount), 0, debtProgram.MaxLoans, isDebtCalculation, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, isQuickSavings);
        }

        public List<ProgramDetailsItemShort> CalcAcceleratedDetails(DebtEliminationDocument debtProgram, bool isDebtCalculation = false, bool isQuickSavings = false)
        {
            var extraPayment = (double)AccountingFormatter.CentsToDollars(debtProgram.MonthlyBudgetInCents);

            switch (debtProgram.PlanId)
            {
                case DebtEliminationPlanEnum.QuickWins:
                    return CalcRankedPayments(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount).OrderBy(d => d.Balance),
                        extraPayment, debtProgram.MaxLoans, isDebtCalculation, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, isQuickSavings);
                case DebtEliminationPlanEnum.HighestInterest:
                    return CalcRankedPayments(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount).OrderByDescending(d => d.Interest),
                        extraPayment, debtProgram.MaxLoans, isDebtCalculation, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, isQuickSavings);
                case DebtEliminationPlanEnum.Balanced:
                    return CalcEquivalentPayments(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount), (decimal)extraPayment,
                        debtProgram.MaxLoans, isDebtCalculation, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, isQuickSavings);
                #region Old Code
                //case DebtEliminationPlanEnum.SmallestDebtHighestPayment:
                //                   return CalcRankedPayments(
                //                       Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount)
                //                           .OrderByDescending(d => d.MinPayment)
                //                           .ThenBy(d => d.Balance), 
                //                       extraPayment, isDebtCalculation);
                #endregion
                case DebtEliminationPlanEnum.SmallestDebtHighestPayment:
                    return CalcRankedPayments(
                        Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount)
                            .OrderBy(d => d.Balance)
                            .ThenByDescending(d => d.MinPayment),
                        extraPayment, debtProgram.MaxLoans, isDebtCalculation, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, isQuickSavings);
                case DebtEliminationPlanEnum.NotInitialized:
                    throw new ArgumentException("Please, choose program.");
                default:
                    throw new NotImplementedException();
            }
        }
        public List<ProgramDetailsItemShort> CalcFBSDetails(DebtEliminationDocument debtProgram)
        {
            var extraPayment = (double)AccountingFormatter.CentsToDollars(debtProgram.MonthlyBudgetInCents);
            switch (debtProgram.PlanId)
            {
                case DebtEliminationPlanEnum.QuickWins:
                    return CalcRankedPaymentsForFBS(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount).OrderBy(d => d.Balance),
                         extraPayment,debtProgram.MaxLoans, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, debtProgram.BudgetForFBS);
                case DebtEliminationPlanEnum.HighestInterest:
                    return CalcRankedPaymentsForFBS(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount).OrderByDescending(d => d.Interest),
                        extraPayment, debtProgram.MaxLoans, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, debtProgram.BudgetForFBS);
                case DebtEliminationPlanEnum.Balanced:
                    return CalcEquivalentPaymentsFBS(Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount),
                        (decimal)extraPayment, debtProgram.MaxLoans, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, debtProgram.BudgetForFBS);

                case DebtEliminationPlanEnum.SmallestDebtHighestPayment:
                    return CalcRankedPaymentsForFBS(
                        Map(debtProgram.Debts, debtProgram.CurrentDebtMonth, debtProgram.NewLoanAmount)
                            .OrderBy(d => d.Balance)
                            .ThenByDescending(d => d.MinPayment),
                        extraPayment, debtProgram.MaxLoans, debtProgram.LumpSumAmount, debtProgram.AmountToSavings,
                        debtProgram.Term1, debtProgram.Term2, debtProgram.Term1Amount, debtProgram.Term2Amount, debtProgram.LoanInterestRate, debtProgram.BudgetForFBS);
                case DebtEliminationPlanEnum.NotInitialized:
                    throw new ArgumentException("Please, choose program.");
                default:
                    throw new NotImplementedException();
            }
        }
        //private List<ProgramDetailsItemShort> CalcRankedPayments(IEnumerable<LoanData> debts, double extraPayment, List<MaxLoan> MaxLoan, bool isDebtCalculation)
        //{
        //    var payments = new List<ProgramDetailsItemShort>();
        //    var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
        //    var totalDebts = notPayedDebts;
        //    var nextMonth = DateTime.Now.AddMonths(1);
        //    var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
        //    decimal LoanAmount = 0;
        //    int i = 1;
        //    decimal LastDebt = 0;
        //    decimal LastDebtAmt = 0;
        //    decimal remainingAmount = 0;
        //    bool IsLastDebtAmtUsed = false;
        //    extraPayment = isDebtCalculation ? extraPayment : 0;
        //    int Year = date.Year;
        //    decimal NewLoanAmount = 0;
        //    decimal addedLoanAmount = 0;
        //    List<MaxLoan> MaxLoans = MaxLoan;
        //    bool IsNewLoanAmountUsed = false;
        //    while (notPayedDebts.Count > 0)
        //    {
        //        var recentExtraMoney = (decimal)extraPayment;
        //        var TotalDebts = notPayedDebts.Count();
        //        IsLastDebtAmtUsed = true;
        //        int debtMonth = notPayedDebts.FirstOrDefault().CurrentDebtMonth;
        //        LoanAmount = MaxLoans.Count > 0 ? (decimal)MaxLoans.FirstOrDefault().MaxNewLoan : LoanAmount;
        //        LoanAmount = LoanAmount / 12;
        //        remainingAmount = 0;

        //        NewLoanAmount += LoanAmount;

        //        foreach (var loanData in notPayedDebts)
        //        {
        //            var requiredExtra = MaxRequiredExtraPayment(loanData);
        //            var loanExtraPayment = Math.Min(requiredExtra, recentExtraMoney);
        //            var interestPaid = loanData.Balance * (decimal)loanData.Interest;
        //            decimal remAmount = 0;
        //            if (remainingAmount > 0)
        //            {
        //                remAmount = remainingAmount;
        //                remainingAmount = 0;
        //            }
        //            if (debtMonth == 0 && isDebtCalculation)
        //            {
        //                //  NewLoanAmount += NewLoanAmount;
        //                addedLoanAmount = NewLoanAmount;
        //                IsNewLoanAmountUsed = true;
        //            }
        //            if (debtMonth == 3 && i % 3 == 0 && isDebtCalculation)
        //            {
        //                // NewLoanAmount += LoanAmount * 3;
        //                addedLoanAmount = NewLoanAmount - LoanAmount;
        //                IsNewLoanAmountUsed = true;
        //            }
        //            if (debtMonth == 4 && i % 4 == 0 && isDebtCalculation)
        //            {
        //                // NewLoanAmount += LoanAmount * 4;
        //                addedLoanAmount = NewLoanAmount - LoanAmount;
        //                IsNewLoanAmountUsed = true;
        //            }
        //            if (debtMonth == 6 && i % 6 == 0 && isDebtCalculation)
        //            {
        //                //  NewLoanAmount += LoanAmount * 6;
        //                addedLoanAmount = NewLoanAmount - LoanAmount;
        //                IsNewLoanAmountUsed = true;
        //            }
        //            var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment + addedLoanAmount + remAmount); // Old Code
        //            if (IsNewLoanAmountUsed)
        //            {
        //                NewLoanAmount = 0;
        //                addedLoanAmount = 0;
        //                IsNewLoanAmountUsed = false;
        //            }

        //            if (isDebtCalculation)
        //            {
        //                if (payment < loanData.MinPayment)
        //                {
        //                    remainingAmount = loanData.MinPayment - payment;
        //                }
        //                if (IsLastDebtAmtUsed)
        //                {
        //                    payment += LastDebtAmt;
        //                    if (loanData.Balance + interestPaid < payment)
        //                    {
        //                        decimal amt = payment - (loanData.Balance + interestPaid);
        //                        payment = payment - amt;
        //                        remainingAmount += amt;
        //                    }
        //                    IsLastDebtAmtUsed = false;
        //                }
        //            }
        //            var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid);
        //            if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
        //            {
        //                payments.Add(detailsItem);
        //                return payments;
        //            }

        //            payments.Add(detailsItem);
        //            loanData.Balance += interestPaid - payment;
        //            recentExtraMoney -= loanExtraPayment;
        //            //NewLoanAmount = 0;
        //        }
        //        notPayedDebts = notPayedDebts.Where(d => Math.Round(d.Balance, 2) > 0).ToList();
        //        if (notPayedDebts.Count() != TotalDebts)
        //        {
        //            totalDebts = totalDebts.Except(notPayedDebts).ToList();
        //            LastDebt = totalDebts[0].MinPayment;
        //            totalDebts = notPayedDebts;
        //            LastDebtAmt += LastDebt;
        //            IsLastDebtAmtUsed = true;
        //        }
        //        date = date.AddMonths(1);
        //        if (Year != date.Year)
        //        {
        //            Year = date.Year;
        //            if (MaxLoans.Count > 0 && isDebtCalculation)
        //            {
        //                MaxLoans.RemoveAt(0);
        //            }
        //        }
        //        i++;
        //    }
        //    return payments;
        //}

        private List<ProgramDetailsItemShort> CalcRankedPayments(IEnumerable<LoanData> debts, double extraPayment, List<MaxLoan> MaxLoan, bool isDebtCalculation, decimal LumpSumAmount,
            decimal AmountToSavings, int Term1, int Term2, long Term1Amount, long Term2Amount, decimal LoanInterestRate, bool isQuickSavings)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var totalDebts = notPayedDebts;
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            decimal LoanAmount = 0;
            int i = 1, j = 0;
            decimal LastDebt = 0;
            decimal LastDebtAmt = 0;
            decimal remainingAmount = 0;
            bool IsLastDebtAmtUsed = false;
            extraPayment = isDebtCalculation ? extraPayment : isQuickSavings ? extraPayment : 0;
            LumpSumAmount = AccountingFormatter.CentsToDollars(Convert.ToInt64(LumpSumAmount));
            decimal NewLoanAmount = 0;
            decimal LastLoanAmountUsed = 0;
            AmountToSavings = AccountingFormatter.CentsToDollars(Convert.ToInt64(AmountToSavings));

            List<MaxLoan> MaxLoans = MaxLoan;
            List<LoanData> LoanPayBack = new List<LoanData>();

            decimal lumpSumAmountValue = isDebtCalculation ? LumpSumAmount : 0;
            while (notPayedDebts.Count > 0)
            {
                var recentExtraMoney = (decimal)extraPayment;
                var TotalDebts = notPayedDebts.Count();
                IsLastDebtAmtUsed = true;
                int debtMonth = notPayedDebts.FirstOrDefault().CurrentDebtMonth;
                LoanAmount = MaxLoans.Count > 0 ? (decimal)MaxLoans.FirstOrDefault().MaxNewLoan : LoanAmount;
                LoanAmount = LoanAmount / 12;
                remainingAmount = 0;
                decimal AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                decimal AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);
               
                    LoanAmount = 0;
                    recentExtraMoney = recentExtraMoney - (Term1 > 0 ? recentExtraMoney >= AmountOfTerm1 ? AmountOfTerm1 : 0 : Term2 > 0 ? recentExtraMoney >= AmountOfTerm2 ? AmountOfTerm2 : 0 : 0);
         
                 
                foreach (var loanData in notPayedDebts)
                {
                    var requiredExtra = MaxRequiredExtraPayment(loanData);
                    var loanExtraPayment = Math.Min(requiredExtra, recentExtraMoney);
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    interestPaid = Math.Floor(interestPaid * 100) / 100;
                    decimal remAmount = 0;
                    if (remainingAmount > 0)
                    {
                        remAmount = remainingAmount;
                        remainingAmount = 0;
                    }

                    //  var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment + NewLoanAmount + remAmount); // Old Code
                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment + NewLoanAmount + remAmount);
                    if (lumpSumAmountValue > 0)
                    {
                        var remBal = (loanData.Balance + interestPaid - payment);
                        var takeLSAmt = lumpSumAmountValue <= remBal ? lumpSumAmountValue : remBal;
                        lumpSumAmountValue = takeLSAmt;
                        payment += takeLSAmt;
                    }

                    if (isDebtCalculation)
                    {
                        if (payment < loanData.MinPayment + NewLoanAmount)
                        {
                            remainingAmount = loanData.MinPayment + NewLoanAmount - payment;
                        }
                        if (IsLastDebtAmtUsed)
                        {
                            payment += LastDebtAmt;
                            if (loanData.Balance + interestPaid < payment)
                            {
                                decimal amt = payment - (loanData.Balance + interestPaid);
                                payment = payment - amt;
                                remainingAmount += amt;
                            }
                            IsLastDebtAmtUsed = false;
                        }
                    }
                    loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                    if (loanData.ClosingBalanceInCents < 0)
                    {
                        loanData.ClosingBalanceInCents = 0;
                    }
                    //     loanData.Balance = Math.Floor(loanData.Balance * 100) / 100;
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                    if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                    {
                        payments.Add(detailsItem);
                        return payments;
                    }

                    payments.Add(detailsItem);
                    loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                    recentExtraMoney -= loanExtraPayment;
                    if (NewLoanAmount > 0 && payment < NewLoanAmount)
                    {
                        LastLoanAmountUsed += payment - loanData.MinPayment;
                    }
                    else
                    {
                        LastLoanAmountUsed += NewLoanAmount;
                    }
                    NewLoanAmount = 0;
                    AmountOfTerm1 = 0;
                    AmountOfTerm2 = 0;

                    if (isDebtCalculation)
                    {
                        LumpSumAmount = LumpSumAmount > 0 && LumpSumAmount > lumpSumAmountValue ? LumpSumAmount - lumpSumAmountValue : 0;
                        lumpSumAmountValue = LumpSumAmount;
                    }
                }

                notPayedDebts = notPayedDebts.Where(d => Math.Round(d.Balance, 2) > 0).ToList();
                if (notPayedDebts.Count() != TotalDebts)
                {
                    totalDebts = totalDebts.Except(notPayedDebts).ToList();
                    foreach (var amt in totalDebts)
                    {
                        LastDebtAmt += amt.MinPayment;
                    }
                    totalDebts = notPayedDebts;
                    IsLastDebtAmtUsed = true;
                }
                date = date.AddMonths(1);
                if (extraPayment > 0)
                {
                    Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                    Term1 = Term1 > 0 ? Term1 - 1 : 0;
                }

                
                i++;
            }
            return payments;
        }

        private List<ProgramDetailsItemShort> CalcEquivalentPayments(IEnumerable<LoanData> debts, decimal extraPayment, List<MaxLoan> MaxLoan,
            bool isDebtCalculation, decimal LumpSumAmount, decimal AmountToSavings, int Term1, int Term2, long Term1Amount, long Term2Amount,
            decimal LoanInterestRate, bool isQuickSavings)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var totalDebts = notPayedDebts;
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            decimal LoanAmount = 0;
            int i = 1, j = 0;
            decimal LastDebt = 0;
            decimal LastDebtAmt = 0;
            bool IsLastDebtAmtUsed = false;
            decimal remainingAmount = 0;
            decimal NewLoanAmount = 0;
            decimal LastLoanAmountUsed = 0;
            AmountToSavings = AccountingFormatter.CentsToDollars(Convert.ToInt64(AmountToSavings));
            extraPayment = isDebtCalculation ? extraPayment : isQuickSavings ? extraPayment : 0;
            List<MaxLoan> MaxLoans = MaxLoan;
            List<LoanData> LoanPayBack = new List<LoanData>();
            LumpSumAmount = AccountingFormatter.CentsToDollars(Convert.ToInt64(LumpSumAmount));
            decimal lumpSumAmountValue = isDebtCalculation ? LumpSumAmount : 0;
            while (notPayedDebts.Count > 0)
            {
                int debtMonth = notPayedDebts.FirstOrDefault().CurrentDebtMonth;
                
                IsLastDebtAmtUsed = true;

                var TotalDebts = notPayedDebts.Count();

                remainingAmount = 0;

                 
                decimal AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                decimal AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);
                
                extraPayment = extraPayment - (Term1 > 0 ? extraPayment >= AmountOfTerm1 ? AmountOfTerm1 : 0 : Term2 > 0 ? extraPayment >= AmountOfTerm2 ? AmountOfTerm2 : 0 : 0);

                DevideExtraPaymentBetweenLoansEqually(notPayedDebts, extraPayment);
                foreach (var loanData in notPayedDebts)
                {
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    interestPaid = Math.Floor(interestPaid * 100) / 100;
                    decimal remAmount = 0;
                    if (remainingAmount > 0)
                    {
                        remAmount = remainingAmount;
                        remainingAmount = 0;
                    }


                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanData.ExtraPayment + NewLoanAmount + remAmount ?? 0);

                    if (lumpSumAmountValue > 0)
                    {
                        var remBal = (loanData.Balance + interestPaid - payment);
                        var takeLSAmt = lumpSumAmountValue <= remBal ? lumpSumAmountValue : remBal;
                        lumpSumAmountValue = takeLSAmt;
                        payment += takeLSAmt;
                    }
                    if (isDebtCalculation)
                    {
                        if (payment < loanData.MinPayment + NewLoanAmount)
                        {
                            remainingAmount = loanData.MinPayment + NewLoanAmount - payment;
                        }
                        if (IsLastDebtAmtUsed)
                        {
                            payment += LastDebtAmt;
                            if (loanData.Balance + interestPaid < payment)
                            {
                                decimal amt = payment - (loanData.Balance + interestPaid);
                                payment = payment - amt;
                                remainingAmount += amt;
                            }
                            IsLastDebtAmtUsed = false;
                        }
                    }
                    loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                    if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                    {
                        payments.Add(detailsItem);
                        return payments;
                    }
                    payments.Add(detailsItem);
                    loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                    if (NewLoanAmount > 0 && payment < NewLoanAmount)
                    {
                        LastLoanAmountUsed += payment - loanData.MinPayment;
                    }
                    else
                    {
                        LastLoanAmountUsed += NewLoanAmount;
                    }
                    NewLoanAmount = 0;
                    AmountOfTerm1 = 0;
                    AmountOfTerm2 = 0;
                    if (isDebtCalculation)
                    {
                        LumpSumAmount = LumpSumAmount > 0 && LumpSumAmount > lumpSumAmountValue ? LumpSumAmount - lumpSumAmountValue : 0;
                        lumpSumAmountValue = LumpSumAmount;
                    }
                }
                notPayedDebts.Each(d => d.ExtraPayment = null);
                notPayedDebts = notPayedDebts.Where(d => d.Balance > 0).ToList();
                if (notPayedDebts.Count() != TotalDebts)
                {
                    totalDebts = totalDebts.Except(notPayedDebts).ToList();
                    foreach (var amt in totalDebts)
                    {
                        LastDebtAmt += amt.MinPayment;
                    }
                    totalDebts = notPayedDebts;
                    IsLastDebtAmtUsed = true;
                }
                date = date.AddMonths(1);
                if (extraPayment > 0)
                {
                    Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                    Term1 = Term1 > 0 ? Term1 - 1 : 0;
                }

                
                 
                i++;
            }
            return payments;
        }

        private List<ProgramDetailsItemShort> CalcRankedPaymentsForFBS(IEnumerable<LoanData> debts, double extraPayment, List<MaxLoan> MaxLoan, decimal LumpSumAmount,
           decimal AmountToSavings, int Term1, int Term2, long Term1Amount, long Term2Amount, decimal LoanInterestRate, bool BudgetForFBS)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var totalDebts = notPayedDebts;
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            decimal LoanAmount = 0;
            int i = 1, j = 0;
            decimal LastDebt = 0;
            decimal LastDebtAmt = 0;
            decimal remainingAmount = 0;
            bool IsLastDebtAmtUsed = false;
            LumpSumAmount = AccountingFormatter.CentsToDollars(Convert.ToInt64(LumpSumAmount));
            decimal NewLoanAmount = 0;
            decimal LastLoanAmountUsed = 0;
            AmountToSavings = AccountingFormatter.CentsToDollars(Convert.ToInt64(AmountToSavings))- (decimal)extraPayment;
            extraPayment = 0;
            if (MaxLoan.Count == 0 || BudgetForFBS == false)
            {
                return payments;
            }

            List<MaxLoan> MaxLoans = MaxLoan;
            List<LoanData> LoanPayBack = new List<LoanData>();

            decimal lumpSumAmountValue = LumpSumAmount;
            while (notPayedDebts.Count > 0)
            {
                var recentExtraMoney = (decimal)extraPayment;
                var TotalDebts = notPayedDebts.Count();
                IsLastDebtAmtUsed = true;
                int debtMonth = notPayedDebts.FirstOrDefault().CurrentDebtMonth;
                LoanAmount = MaxLoans.Count > 0 ? (decimal)MaxLoans.FirstOrDefault().MaxNewLoan : LoanAmount;
                LoanAmount = LoanAmount / 12;
                remainingAmount = 0;
                decimal AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                decimal AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);

               // recentExtraMoney = recentExtraMoney - (Term1 > 0 ? recentExtraMoney > AmountOfTerm1 ? AmountOfTerm1 : 0 : Term2 > 0 ? recentExtraMoney > AmountOfTerm2 ? AmountOfTerm2 : 0 : 0);

                 
                if (debtMonth == 0)
                {
                    NewLoanAmount = LoanAmount;
                }
                if (debtMonth == 3 && i % 3 == 0)
                {
                    NewLoanAmount = LoanAmount * 3;
                    j++;
                }
                if (debtMonth == 4 && i % 4 == 0)
                {
                    NewLoanAmount = LoanAmount * 4;
                    j++;
                }
                if (debtMonth == 6 && i % 6 == 0)
                {
                    NewLoanAmount = LoanAmount * 6;
                    j++;
                }
                if (debtMonth == 12 && i % 12 == 0)
                {
                    NewLoanAmount = LoanAmount * 12;
                    j++;
                }

                foreach (var loanData in notPayedDebts)
                {
                     
                    var requiredExtra = MaxRequiredExtraPayment(loanData);
                    var loanExtraPayment = Math.Min(requiredExtra, recentExtraMoney);
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    interestPaid = Math.Floor(interestPaid * 100) / 100;
                    decimal remAmount = 0;
                    if (remainingAmount > 0)
                    {
                        remAmount = remainingAmount;
                        remainingAmount = 0;
                    }

                    //  var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment + NewLoanAmount + remAmount); // Old Code
                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment + NewLoanAmount + remAmount);
                    if (lumpSumAmountValue > 0)
                    {
                        var remBal = (loanData.Balance + interestPaid - payment);
                        var takeLSAmt = lumpSumAmountValue <= remBal ? lumpSumAmountValue : remBal;
                        lumpSumAmountValue = takeLSAmt;
                        payment += takeLSAmt;
                    }


                    if (payment < loanData.MinPayment + NewLoanAmount)
                    {
                        remainingAmount = loanData.MinPayment + NewLoanAmount - payment;
                    }
                    if (IsLastDebtAmtUsed)
                    {
                        payment += LastDebtAmt;
                        if (loanData.Balance + interestPaid < payment)
                        {
                            decimal amt = payment - (loanData.Balance + interestPaid);
                            payment = payment - amt;
                            remainingAmount += amt;
                        }
                        IsLastDebtAmtUsed = false;
                    }

                    loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                    if (loanData.ClosingBalanceInCents < 0)
                    {
                        loanData.ClosingBalanceInCents = 0;
                    }
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                    if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                    {
                        payments.Add(detailsItem);
                        return payments;
                    }

                    payments.Add(detailsItem);
                    loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                    recentExtraMoney -= loanExtraPayment;
                    if (NewLoanAmount > 0 && payment < NewLoanAmount)
                    {
                        LastLoanAmountUsed += payment - loanData.MinPayment;
                    }
                    else
                    {
                        LastLoanAmountUsed += NewLoanAmount;
                    }
                    NewLoanAmount = 0;
                    AmountOfTerm1 = 0;
                    AmountOfTerm2 = 0;

                    LumpSumAmount = LumpSumAmount > 0 && LumpSumAmount > lumpSumAmountValue ? LumpSumAmount - lumpSumAmountValue : 0;
                    lumpSumAmountValue = LumpSumAmount;

                }

                notPayedDebts = notPayedDebts.Where(d => Math.Round(d.Balance, 2) > 0).ToList();
                if (notPayedDebts.Count() != TotalDebts)
                {
                    totalDebts = totalDebts.Except(notPayedDebts).ToList();
                    foreach (var amt in totalDebts)
                    {
                        LastDebtAmt += amt.MinPayment;
                    }
                    totalDebts = notPayedDebts;
                    IsLastDebtAmtUsed = true;
                }
                date = date.AddMonths(1);

                //Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                //Term1 = Term1 > 0 ? Term1 - 1 : 0;

                decimal RemainingAmountAfterLastPayment = 0;

                if (debtMonth == 3 && i % 3 == 0 && j == 4)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 4 && i % 4 == 0 && j == 3)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 6 && i % 6 == 0 && j == 2)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 12 && i % 12 == 0 && j == 1)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                //}
                //else
                if (notPayedDebts.Count == 0)
                {
                   // AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                    //AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);
                    if (LastLoanAmountUsed > 0 && LoanPayBack.Count == 0)
                    {
                        LoanData newLoanData = new LoanData();
                        LastLoanAmountUsed = LastLoanAmountUsed + LastLoanAmountUsed * Convert.ToDecimal(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(LastLoanAmountUsed);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        LastLoanAmountUsed = 0;
                    }
                    while (LoanPayBack.Count > 0)
                    {

                        foreach (var loanData in LoanPayBack)
                        {
                            LastLoanAmountUsed = LastLoanAmountUsed + LastLoanAmountUsed * Convert.ToDecimal(LoanInterestRate / 100);
                            loanData.Balance += LastLoanAmountUsed;
                            loanData.ClosingBalanceInCents += LastLoanAmountUsed;
                            //decimal MinPayment = loanData.MinPayment - (Term1 > 0 ? AmountOfTerm1 : Term2 > 0 ? AmountOfTerm2 : 0);
                            decimal MinPayment = loanData.MinPayment;

                            var interestPaid = loanData.Balance * (Convert.ToDecimal(LoanInterestRate / 12 / 100));
                            interestPaid = Math.Floor(interestPaid * 100) / 100;

                            var payment = Math.Min(loanData.Balance + interestPaid, MinPayment);

                            loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                            if (loanData.ClosingBalanceInCents < 0)
                            {
                                loanData.ClosingBalanceInCents = 0;
                            }
                            var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                            if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                            {
                                payments.Add(detailsItem);
                                return payments;
                            }

                            payments.Add(detailsItem);
                            loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                            LastLoanAmountUsed = 0;

                        }
                        LoanPayBack = LoanPayBack.Where(d => Math.Round(d.Balance, 2) > 0).ToList();
                        date = date.AddMonths(1);
                        //Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                        //Term1 = Term1 > 0 ? Term1 - 1 : 0;

                    }
                }

                i++;
            }
            return payments;
        }

        private List<ProgramDetailsItemShort> CalcEquivalentPaymentsFBS(IEnumerable<LoanData> debts, decimal extraPayment, List<MaxLoan> MaxLoan,
             decimal LumpSumAmount, decimal AmountToSavings, int Term1, int Term2, long Term1Amount, long Term2Amount,
            decimal LoanInterestRate, bool BudgetForFBS)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var totalDebts = notPayedDebts;
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            decimal LoanAmount = 0;
            int i = 1, j = 0;
            decimal LastDebt = 0;
            decimal LastDebtAmt = 0;
            bool IsLastDebtAmtUsed = false;
            decimal remainingAmount = 0;
            decimal NewLoanAmount = 0;
            decimal LastLoanAmountUsed = 0;
            AmountToSavings = AccountingFormatter.CentsToDollars(Convert.ToInt64(AmountToSavings));


            if (MaxLoan.Count == 0 || BudgetForFBS == false)
            {
                return payments;
            }

            List<MaxLoan> MaxLoans = MaxLoan;
            List<LoanData> LoanPayBack = new List<LoanData>();
            LumpSumAmount = AccountingFormatter.CentsToDollars(Convert.ToInt64(LumpSumAmount));
            decimal lumpSumAmountValue = LumpSumAmount;
            while (notPayedDebts.Count > 0)
            {
                int debtMonth = notPayedDebts.FirstOrDefault().CurrentDebtMonth;
                LoanAmount = MaxLoans.Count > 0 ? (decimal)MaxLoans.FirstOrDefault().MaxNewLoan : LoanAmount;
                LoanAmount = LoanAmount / 12;
                IsLastDebtAmtUsed = true;

                var TotalDebts = notPayedDebts.Count();

                remainingAmount = 0;

                if (debtMonth == 0)
                {
                    NewLoanAmount = LoanAmount;
                }
                if (debtMonth == 3 && i % 3 == 0)
                {
                    NewLoanAmount = LoanAmount * 3;
                    j++;
                }
                if (debtMonth == 4 && i % 4 == 0)
                {
                    NewLoanAmount = LoanAmount * 4;
                    j++;
                }
                if (debtMonth == 6 && i % 6 == 0)
                {
                    NewLoanAmount = LoanAmount * 6;
                    j++;
                }
                if (debtMonth == 12 && i % 12 == 0)
                {
                    NewLoanAmount = LoanAmount * 12;
                    j++;
                }
                decimal AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                decimal AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);
               // AmountToSavings = AmountToSavings - (Term1 > 0 ? AmountOfTerm1 : Term2 > 0 ? AmountOfTerm2 : 0);
              
                DevideExtraPaymentBetweenLoansEqually(notPayedDebts, 0);
                foreach (var loanData in notPayedDebts)
                {
                    //var requiredExtra = MaxRequiredExtraPayment(loanData);
                    //var loanExtraPayment = Math.Min(requiredExtra, recentExtraMoney);
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    interestPaid = Math.Floor(interestPaid * 100) / 100;
                    decimal remAmount = 0;
                    if (remainingAmount > 0)
                    {
                        remAmount = remainingAmount;
                        remainingAmount = 0;
                    }


                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanData.ExtraPayment + NewLoanAmount + remAmount ?? 0);

                    if (lumpSumAmountValue > 0)
                    {
                        var remBal = (loanData.Balance + interestPaid - payment);
                        var takeLSAmt = lumpSumAmountValue <= remBal ? lumpSumAmountValue : remBal;
                        lumpSumAmountValue = takeLSAmt;
                        payment += takeLSAmt;
                    }

                    if (payment < loanData.MinPayment + NewLoanAmount)
                    {
                        remainingAmount = loanData.MinPayment + NewLoanAmount - payment;
                    }
                    if (IsLastDebtAmtUsed)
                    {
                        payment += LastDebtAmt;
                        if (loanData.Balance + interestPaid < payment)
                        {
                            decimal amt = payment - (loanData.Balance + interestPaid);
                            payment = payment - amt;
                            remainingAmount += amt;
                        }
                        IsLastDebtAmtUsed = false;
                    }

                    loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                    if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                    {
                        payments.Add(detailsItem);
                        return payments;
                    }
                    payments.Add(detailsItem);
                    loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                    if (NewLoanAmount > 0 && payment < NewLoanAmount)
                    {
                        LastLoanAmountUsed += payment - loanData.MinPayment;
                    }
                    else
                    {
                        LastLoanAmountUsed += NewLoanAmount;
                    }
                    NewLoanAmount = 0;
                    AmountOfTerm1 = 0;
                    AmountOfTerm2 = 0;

                    LumpSumAmount = LumpSumAmount > 0 && LumpSumAmount > lumpSumAmountValue ? LumpSumAmount - lumpSumAmountValue : 0;
                    lumpSumAmountValue = LumpSumAmount;

                }
                notPayedDebts.Each(d => d.ExtraPayment = null);
                notPayedDebts = notPayedDebts.Where(d => d.Balance > 0).ToList();
                if (notPayedDebts.Count() != TotalDebts)
                {
                    totalDebts = totalDebts.Except(notPayedDebts).ToList();
                    foreach (var amt in totalDebts)
                    {
                        LastDebtAmt += amt.MinPayment;
                    }
                    totalDebts = notPayedDebts;
                    IsLastDebtAmtUsed = true;
                }
                date = date.AddMonths(1);

                Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                Term1 = Term1 > 0 ? Term1 - 1 : 0;

                if (debtMonth == 3 && i % 3 == 0 && j == 4)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = 0;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 4 && i % 4 == 0 && j == 3)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 6 && i % 6 == 0 && j == 2)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (debtMonth == 12 && i % 12 == 0 && j == 1)
                {
                    if (MaxLoans.Count > 0)
                    {
                        var lastLoan = MaxLoans.FirstOrDefault();
                        LoanData newLoanData = new LoanData();
                        decimal lastamount = 0;
                        if (LoanPayBack.Count > 0)
                        {
                            lastamount = LoanPayBack.FirstOrDefault().Balance;
                        }
                        lastLoan.MaxNewLoan = Convert.ToDouble(lastamount) + lastLoan.MaxNewLoan + (lastLoan.MaxNewLoan + Convert.ToDouble(lastamount)) * Convert.ToDouble(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(lastLoan.MaxNewLoan);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        MaxLoans.RemoveAt(0);
                        LastLoanAmountUsed = 0;
                    }
                    j = 0;
                }
                if (notPayedDebts.Count == 0)
                {
                    AmountOfTerm1 = AccountingFormatter.CentsToDollars(Term1Amount);
                    AmountOfTerm2 = AccountingFormatter.CentsToDollars(Term2Amount);
                    if (LastLoanAmountUsed > 0 && LoanPayBack.Count == 0)
                    {
                        LoanData newLoanData = new LoanData();
                        LastLoanAmountUsed = LastLoanAmountUsed + LastLoanAmountUsed * Convert.ToDecimal(LoanInterestRate / 100);
                        newLoanData.Balance = Convert.ToDecimal(LastLoanAmountUsed);
                        newLoanData.ClosingBalanceInCents = newLoanData.Balance;
                        newLoanData.Name = "Policy Loan Payback";
                        newLoanData.MinPayment = AmountToSavings;
                        newLoanData.LoanInterestRate = LoanInterestRate;
                        LoanPayBack = new List<LoanData>();
                        LoanPayBack.Add(newLoanData);
                        LastLoanAmountUsed = 0;
                    }
                    while (LoanPayBack.Count > 0)
                    {

                        foreach (var loanData in LoanPayBack)
                        {
                            LastLoanAmountUsed = LastLoanAmountUsed + LastLoanAmountUsed * Convert.ToDecimal(LoanInterestRate / 100);
                            loanData.Balance += LastLoanAmountUsed;
                            loanData.ClosingBalanceInCents += LastLoanAmountUsed;
                           // decimal MinPayment = loanData.MinPayment - (Term1 > 0 ? AmountOfTerm1 : Term2 > 0 ? AmountOfTerm2 : 0);
                            decimal MinPayment = loanData.MinPayment;

                            var interestPaid = loanData.Balance * (Convert.ToDecimal(LoanInterestRate / 12 / 100));
                            interestPaid = Math.Floor(interestPaid * 100) / 100;

                            var payment = Math.Min(loanData.Balance + interestPaid, MinPayment);

                            loanData.ClosingBalanceInCents += Math.Floor((interestPaid - payment) * 100) / 100;
                            if (loanData.ClosingBalanceInCents < 0)
                            {
                                loanData.ClosingBalanceInCents = 0;
                            }
                            var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, MinPayment, payment, interestPaid, loanData.ClosingBalanceInCents);
                            if (!string.IsNullOrEmpty(detailsItem.ErrorMessage))
                            {
                                payments.Add(detailsItem);
                                return payments;
                            }

                            payments.Add(detailsItem);
                            loanData.Balance += Math.Floor((interestPaid - payment) * 100) / 100;
                            LastLoanAmountUsed = 0;

                        }
                        LoanPayBack = LoanPayBack.Where(d => Math.Round(d.Balance, 2) > 0).ToList();
                        date = date.AddMonths(1);
                        //Term2 = Term1 == 0 ? Term2 > 0 ? Term2 - 1 : 0 : Term2;
                        //Term1 = Term1 > 0 ? Term1 - 1 : 0;

                    }
                }
                i++;
            }
            return payments;
        }

        private static IEnumerable<LoanData> Map(IEnumerable<DebtItemData> debts, int debtMonth, double NewLoanAmount)
        {
            return debts.Select(d => new LoanData
            {
                Id = d.DebtId,
                Name = d.Name,
                Balance = AccountingFormatter.CentsToDollars(Convert.ToInt64(d.BalanceInCents)),//usha
                Interest = d.InterestRatePerc / 12 / 100,
                MinPayment = AccountingFormatter.CentsToDollars(Convert.ToInt64(d.MinMonthPaymentInCents)),//usha
                CurrentDebtMonth = debtMonth,
                NewLoanAmount = NewLoanAmount / 12,
                ClosingBalanceInCents = AccountingFormatter.CentsToDollars(Convert.ToInt64(d.BalanceInCents))
            });
        }

        private ProgramDetailsItemShort CreateProgramDetailsItem(string id, string loanName, DateTime date, decimal balance, decimal minPayment, decimal payment, decimal interestPaid, decimal ClosingBalance)
        {
            if (payment <= interestPaid)
                // throw new ArgumentException("Minimum monthly payment is too small - you'll never repay the loan.");
                return new ProgramDetailsItemShort
                {
                    ErrorMessage = "Your combined minimum monthly payments in "+ loanName+" are too small to pay off the combined loan balances. Please check your entries.",
                    Id = id,
                    Debt = loanName,
                    Date = date,
                    BalanceInCents = DollarsToCents(balance),
                    MinPaymentInCents = DollarsToCents(minPayment),
                    ActualPaymentInCents = DollarsToCents(payment),
                    PrincipalPaymentInCents = DollarsToCents(payment) - DollarsToCents(interestPaid),
                    InterestPaymentInCents = DollarsToCents(interestPaid),
                    ClosingBalanceInCents = DollarsToCents(ClosingBalance)
                };

            return new ProgramDetailsItemShort
            {
                Id = id,
                Debt = loanName,
                Date = date,
                BalanceInCents = DollarsToCents(balance),
                MinPaymentInCents = DollarsToCents(minPayment),
                ActualPaymentInCents = DollarsToCents(payment),
                PrincipalPaymentInCents = DollarsToCents(payment - interestPaid),
                InterestPaymentInCents = DollarsToCents(interestPaid),
                ClosingBalanceInCents = DollarsToCents(ClosingBalance)
            };
        }

        private void DevideExtraPaymentBetweenLoansEqually(List<LoanData> debts, decimal extraPayment)
        {
            if (debts.Sum(d => MaxRequiredExtraPayment(d)) <= extraPayment)
            {
                debts.Each(d => d.ExtraPayment = MaxRequiredExtraPayment(d));
            }
            else
            {
                var notPayedDebts = debts.Where(d => !d.ExtraPayment.HasValue).ToList();
                while (notPayedDebts.Count > 0)
                {
                    var eachLoanExtraMoney = extraPayment / notPayedDebts.Count;
                    var lessExtraReuiredDebts = notPayedDebts.Where(d => MaxRequiredExtraPayment(d) < eachLoanExtraMoney).ToList();
                    if (lessExtraReuiredDebts.Count > 0)
                    {
                        lessExtraReuiredDebts.Each(d => d.ExtraPayment = MaxRequiredExtraPayment(d));
                    }
                    else
                    {
                        notPayedDebts.Each(d => d.ExtraPayment = eachLoanExtraMoney);
                    }
                    notPayedDebts = notPayedDebts.Where(d => !d.ExtraPayment.HasValue).ToList();
                }
            }
        }

        private static decimal MaxRequiredExtraPayment(LoanData debt)
        {
            var maxExtraPayment = debt.Balance * (1 + (decimal)debt.Interest) - debt.MinPayment;
            return maxExtraPayment > 0 ? maxExtraPayment : 0;
        }

        private static long DollarsToCents(decimal dollars)
        {
            return AccountingFormatter.DollarsToCents(Math.Round(dollars, 2));
            // dollars= Math.Floor(dollars * 100) / 100;
            // return AccountingFormatter.DollarsToCents(dollars);
        }

        public class LoanData
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public decimal Balance { get; set; }

            public float Interest { get; set; }

            public decimal MinPayment { get; set; }

            public decimal? ExtraPayment { get; set; }
            public int CurrentDebtMonth { get; set; }
            public double NewLoanAmount { get; set; }
            public decimal LoanInterestRate { get; set; }
            public decimal ClosingBalanceInCents { get; set; }

        }
    }
}