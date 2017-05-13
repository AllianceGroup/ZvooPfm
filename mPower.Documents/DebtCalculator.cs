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
          
        public List<ProgramDetailsItemShort> CalcOriginaldDetails(DebtEliminationDocument debtProgram)
        {           
            return CalcRankedPayments(Map(debtProgram.Debts), 0);     
        }

        public List<ProgramDetailsItemShort> CalcAcceleratedDetails(DebtEliminationDocument debtProgram)
        {
            var extraPayment = (double) AccountingFormatter.CentsToDollars(debtProgram.MonthlyBudgetInCents);

            switch (debtProgram.PlanId)
            {
                case DebtEliminationPlanEnum.QuickWins:
                    return CalcRankedPayments(Map(debtProgram.Debts).OrderBy(d => d.Balance), extraPayment);
                case DebtEliminationPlanEnum.HighestInterest:
                    return CalcRankedPayments(Map(debtProgram.Debts).OrderByDescending(d => d.Interest), extraPayment);
                case DebtEliminationPlanEnum.Balanced:
                    return CalcEquivalentPayments(Map(debtProgram.Debts), (decimal)extraPayment);
                case DebtEliminationPlanEnum.SmallestDebtHighestPayment:
                    return CalcRankedPayments(
                        Map(debtProgram.Debts)
                            .OrderByDescending(d => d.MinPayment)
                            .ThenBy(d => d.Balance), 
                        extraPayment);
                case DebtEliminationPlanEnum.NotInitialized:
                    throw new ArgumentException("Please, choose program.");
                default:
                    throw new NotImplementedException();
            }
        }

        private List<ProgramDetailsItemShort> CalcRankedPayments(IEnumerable<LoanData> debts, double extraPayment)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            while (notPayedDebts.Count > 0)
            {
                var recentExtraMoney = (decimal)extraPayment;
                foreach (var loanData in notPayedDebts)
                {
                    var requiredExtra = MaxRequiredExtraPayment(loanData);
                    var loanExtraPayment = Math.Min(requiredExtra, recentExtraMoney);
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanExtraPayment);
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid);
                    payments.Add(detailsItem);
                    loanData.Balance += interestPaid - payment;
                    recentExtraMoney -= loanExtraPayment;
                }
                notPayedDebts = notPayedDebts.Where(d => d.Balance > 0).ToList();
                date = date.AddMonths(1);
            }
            return payments;
        }

        private List<ProgramDetailsItemShort> CalcEquivalentPayments(IEnumerable<LoanData> debts, decimal extraPayment)
        {
            var payments = new List<ProgramDetailsItemShort>();
            var notPayedDebts = debts.Where(d => d.Balance > 0).ToList();
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            while (notPayedDebts.Count > 0)
            {
                DevideExtraPaymentBetweenLoansEqually(notPayedDebts, extraPayment);
                foreach (var loanData in notPayedDebts)
                {
                    var interestPaid = loanData.Balance * (decimal)loanData.Interest;
                    var payment = Math.Min(loanData.Balance + interestPaid, loanData.MinPayment + loanData.ExtraPayment ?? 0);
                    var detailsItem = CreateProgramDetailsItem(loanData.Id, loanData.Name, date, loanData.Balance, loanData.MinPayment, payment, interestPaid);
                    payments.Add(detailsItem);
                    loanData.Balance += interestPaid - payment;
                }
                notPayedDebts.Each(d => d.ExtraPayment = null);
                notPayedDebts = notPayedDebts.Where(d => d.Balance > 0).ToList();
                date = date.AddMonths(1);
            }
            return payments;
        }

        private static IEnumerable<LoanData> Map(IEnumerable<DebtItemData> debts)
        {
            return debts.Select(d => new LoanData
            {
                Id = d.DebtId,
                Name = d.Name,
                Balance = AccountingFormatter.CentsToDollars(d.BalanceInCents),
                Interest = d.InterestRatePerc / 12 / 100,
                MinPayment = AccountingFormatter.CentsToDollars(d.MinMonthPaymentInCents),
            });
        }

        private ProgramDetailsItemShort CreateProgramDetailsItem(string id, string loanName, DateTime date, decimal balance, decimal minPayment, decimal payment, decimal interestPaid)
        {
            if (payment <= interestPaid)
                throw new ArgumentException("Minimum monthly payment is too small - you'll never repay the loan.");

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
        }

        public class LoanData
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public decimal Balance { get; set; }

            public float Interest { get; set; }

            public decimal MinPayment { get; set; }

            public decimal? ExtraPayment { get; set; }
        }
    }
}