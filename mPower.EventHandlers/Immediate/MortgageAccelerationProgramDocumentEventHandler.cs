using System;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.DebtElimination.Events;
using mPower.Domain.Accounting.Enums;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class MortgageAccelerationProgramDocumentEventHandler :
        IMessageHandler<DebtElimination_MortgageProgram_AddedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_UpdatedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_DeletedEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_AddedToCalendarEvent>
    {
        private readonly DebtEliminationDocumentService _debtEliminationService;

        public MortgageAccelerationProgramDocumentEventHandler(DebtEliminationDocumentService debtEliminationService)
        {
            _debtEliminationService = debtEliminationService;
        }

        public void Handle(DebtElimination_MortgageProgram_AddedEvent message)
        {
            var mortgageProgram = new MortgageAccelerationProgramDocument
            {
                Id = message.Id,
                Title = message.Title,
                LoanAmountInCents = message.LoanAmountInCents,
                InterestRatePerYear = message.InterestRatePerYear,
                LoanTermInYears = message.LoanTermInYears,
                PaymentPeriod = message.PaymentPeriod,
                ExtraPaymentInCentsPerPeriod = message.ExtraPaymentInCentsPerPeriod,
                DisplayResolution = message.DisplayResolution,
            };
            PerformDebtEliminationCalculations(mortgageProgram);

            var query = Query.EQ("_id", message.DebtEliminationId);
            var update = Update.Push("MortgagePrograms", mortgageProgram.ToBsonDocument());
            _debtEliminationService.Update(query, update);

            SetCurrentMortgageProgram(message.DebtEliminationId, message.Id);
        }

        public void Handle(DebtElimination_MortgageProgram_UpdatedEvent message)
        {
            var mortgageProgram = new MortgageAccelerationProgramDocument
            {
                Id = message.Id,
                Title = message.Title,
                LoanAmountInCents = message.LoanAmountInCents,
                InterestRatePerYear = message.InterestRatePerYear,
                LoanTermInYears = message.LoanTermInYears,
                PaymentPeriod = message.PaymentPeriod,
                ExtraPaymentInCentsPerPeriod = message.ExtraPaymentInCentsPerPeriod,
                DisplayResolution = message.DisplayResolution,
                AddedToCalendar = message.AddedToCalendar,
            };
            PerformDebtEliminationCalculations(mortgageProgram);

            var query = Query.And(Query.EQ("_id", message.DebtEliminationId), Query.EQ("MortgagePrograms._id", message.Id));
            var update = Update.Set("MortgagePrograms.$", mortgageProgram.ToBsonDocument());
            _debtEliminationService.Update(query, update);

            SetCurrentMortgageProgram(message.DebtEliminationId, message.Id);
        }

        public void Handle(DebtElimination_MortgageProgram_DeletedEvent message)
        {
            var query = Query.EQ("_id", message.DebtEliminationId);
            var innerQuery = Query.EQ("_id", message.Id);
            var update = Update.Pull("MortgagePrograms", innerQuery);
            _debtEliminationService.Update(query, update);

            // set new current program
            var debt = _debtEliminationService.GetById(message.DebtEliminationId);
            if (debt.CurrentMortgageProgramId == message.Id)
            {
                var newCurrMortgageProgram = debt.MortgagePrograms.Find(mp => mp.Id != message.Id);
                if (newCurrMortgageProgram != null)
                {
                    SetCurrentMortgageProgram(message.DebtEliminationId, newCurrMortgageProgram.Id);
                }
            }
        }

        public void Handle(DebtElimination_MortgageProgram_AddedToCalendarEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.DebtEliminationId), Query.EQ("MortgagePrograms._id", message.MortgageProgramId));
            var update = Update.Set("MortgagePrograms.$.AddedToCalendar", true);
            _debtEliminationService.Update(query, update);
        }

        private void SetCurrentMortgageProgram(string debtId, string programId)
        {
            var query = Query.And(Query.EQ("_id", debtId));
            var update = Update.Set("CurrentMortgageProgramId", programId);
            _debtEliminationService.UpdateMany(query, update);
        }

        private void PerformDebtEliminationCalculations(MortgageAccelerationProgramDocument program)
        {
            // USE 'DECIMAL' WHERE IT POSSIBLE TO MINIMISE CALCULATION MISTAKE

            var coeffYear = (double)GetCoeffYear(program.PaymentPeriod);
            // PaymentPeriod for original data is always Monthly
            var coeffMonth = coeffYear * 12;
            var loanAmount =(double) AccountingFormatter.CentsToDollars(program.LoanAmountInCents);
            var monthlyInterestRate = program.InterestRatePerYear / 12;
            var interestRateViaPlan = program.InterestRatePerYear * (float)coeffYear;
            var loanTermInMonths = program.LoanTermInYears * 12;
            var monthlyPayment = LoanCalculator.PaymentSize((double)loanAmount, (double)loanTermInMonths, (double)monthlyInterestRate, false);
            var monthlyPayoffTime = LoanCalculator.LoanPayments((double)loanAmount, (double)monthlyPayment, (double)monthlyInterestRate, false);

            var paymentViaPlan = monthlyPayment / ((int)program.PaymentPeriod) + (double)AccountingFormatter.CentsToDollars(program.ExtraPaymentInCentsPerPeriod);
            var payoffTimeViaPlan = LoanCalculator.LoanPayments((double)loanAmount, (double)paymentViaPlan, (double)interestRateViaPlan, false);

            var interestPaid = LoanCalculator.TotalInterestPaid((double)loanAmount, (double)monthlyPayment, (double)monthlyInterestRate);
            var interestPaidViaPlan = LoanCalculator.TotalInterestPaid((double)loanAmount, (double)paymentViaPlan, (double)interestRateViaPlan);

            // check for incorrect input
            if (double.IsNaN(monthlyPayment) || double.IsNaN(monthlyPayoffTime) || double.IsNaN(paymentViaPlan) || double.IsNaN(payoffTimeViaPlan) || double.IsNaN(interestPaid) || double.IsNaN(interestPaidViaPlan))
            {
                return;
            }

            // comparison data

            program.OriginalParams.PaymentInCents = program.OriginalParams.MonthlyPaymentEquivalentInCents = AccountingFormatter.DollarsToCents(monthlyPayment);
            program.OriginalParams.TotalInterestPaidInCents = AccountingFormatter.DollarsToCents(interestPaid);
            program.OriginalParams.TotalPaymentAmountInCents = program.LoanAmountInCents + program.OriginalParams.TotalInterestPaidInCents;
            program.OriginalParams.TotalSavingsInCents = 0;
            program.OriginalParams.PayoffTime = program.LoanTermInYears;

            program.AcceleratedParams.PaymentInCents = AccountingFormatter.DollarsToCents(paymentViaPlan);
            program.AcceleratedParams.MonthlyPaymentEquivalentInCents = AccountingFormatter.DollarsToCents(Math.Round(paymentViaPlan / coeffMonth, 2));
            program.AcceleratedParams.TotalInterestPaidInCents = AccountingFormatter.DollarsToCents(interestPaidViaPlan);
            program.AcceleratedParams.TotalPaymentAmountInCents = program.LoanAmountInCents + program.AcceleratedParams.TotalInterestPaidInCents;

            program.AcceleratedParams.TotalSavingsInCents = program.OriginalParams.TotalPaymentAmountInCents -
                                                            program.AcceleratedParams.TotalPaymentAmountInCents;

            program.AcceleratedParams.PayoffTime = Math.Round(payoffTimeViaPlan * coeffYear, 1);


            // details
            var nextMonth = DateTime.Now.AddMonths(1);
            var date = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            var periodInDays = coeffYear * (52 * 7); // 52 weeks in year
            var origLastDate = date.AddMonths((int)monthlyPayoffTime + 1);
            var accLastDate = date.AddDays((double)(payoffTimeViaPlan * periodInDays));
            var lastDate = origLastDate > accLastDate ? origLastDate : accLastDate;
            var step = 1;
            double lastMonthNumber = 0;
            double lastYearNumber = 0;
            var shownPeriodMod = GetPeriodsInStep(program.DisplayResolution);
            var origBalance = loanAmount;
            var accBalance = loanAmount;
            double accEquity = 0;
            double origEquity = 0;
            double accTotalInterest = 0;
            double origTotalInterest = 0;
            while (accBalance > 0 || origBalance > 0)
            {
                var accInterest = accBalance * interestRateViaPlan / 100;
                var accPayment = Math.Min(paymentViaPlan, accBalance + accInterest);
                var accPrincipal = accPayment - accInterest;
                accEquity += accPrincipal;
                accTotalInterest += accInterest;

                // original paramenters calculating
                var isNewMonth = step * coeffMonth >= lastMonthNumber;
                var isNewYear = false;
                double origPrincipal = 0;
                if (isNewMonth)
                {
                    lastMonthNumber = (step * coeffMonth) + 1;
                    isNewYear |= step * coeffYear > lastYearNumber;
                    if (isNewYear)
                    {
                        lastYearNumber = (lastMonthNumber - 1) / 12 + 1;
                    }

                    var monthlyInterest = origBalance * monthlyInterestRate / 100;
                    var origPayment = Math.Min(monthlyPayment, origBalance + monthlyInterest);
                    origPrincipal = origPayment - monthlyInterest;
                    origEquity += origPrincipal;
                    origTotalInterest += monthlyInterest;
                }

                var item = new ProgramDetailsItem
                {
                    Step = step,
                    Date = date,
                    ShowInGrid = (shownPeriodMod == 1 || step % shownPeriodMod == 1 || paymentViaPlan > accPayment) && accPayment > 0, // requered step OR last step
                    ShowInGraph = isNewYear,

                    OrigBalanceInCents = AccountingFormatter.DollarsToCents(origBalance),
                    OrigTotalInterestInCents = AccountingFormatter.DollarsToCents(origTotalInterest),
                    OrigTotalPaymentsInCents = AccountingFormatter.DollarsToCents(origEquity + origTotalInterest),

                    AccBalanceInCents = AccountingFormatter.DollarsToCents(accBalance),
                    AccPaymentInCents = AccountingFormatter.DollarsToCents(accPayment),
                    AccPrincipalInCents = AccountingFormatter.DollarsToCents(accPrincipal),
                    AccInterestInCents = AccountingFormatter.DollarsToCents(accInterest),
                    AccEquityInCents = AccountingFormatter.DollarsToCents(accEquity),
                    AccTotalInterestInCents = AccountingFormatter.DollarsToCents(accTotalInterest),
                    AccTotalPaymentsInCents = AccountingFormatter.DollarsToCents(accEquity + accTotalInterest),
                };
                program.Details.Add(item);

                if (isNewMonth)
                {
                    origBalance -= Math.Min(origPrincipal, origBalance);
                }

                accBalance -= Math.Min(accPrincipal, accBalance);
                date = GetNextDate(date, program.PaymentPeriod, lastDate);
                step++;
            }
        }

        private static decimal GetCoeffYear(PaymentPeriodEnum paymentPeriod)
        {
            switch (paymentPeriod)
            {
                case PaymentPeriodEnum.Weekly:
                    return 1 / 52m;
                case PaymentPeriodEnum.Biweekly:
                    return 1 / 26m;
                case PaymentPeriodEnum.Monthly:
                    return 1 / 12m;

                default:
                    throw new NotImplementedException();
            }
        }

        private static int GetPeriodsInStep(DisplayResolutionEnum displayResolution)
        {
            switch (displayResolution)
            {
                // for providing graph with data at list one value for each year is required (month is biggest payment period)
                case DisplayResolutionEnum.Low:
                    return 12;
                case DisplayResolutionEnum.Medium:
                    return 12;
                case DisplayResolutionEnum.High:
                    return 3;
                case DisplayResolutionEnum.Maximum:
                    return 1;

                default:
                    throw new NotImplementedException();
            }
        }

        private static DateTime GetNextDate(DateTime date, PaymentPeriodEnum period, DateTime lastDate)
        {
            DateTime newDate;
            switch (period)
            {
                case PaymentPeriodEnum.Weekly:
                    newDate = date.AddDays(7);
                    break;
                case PaymentPeriodEnum.Biweekly:
                    newDate = date.AddDays(14);
                    break;
                case PaymentPeriodEnum.Monthly:
                    newDate = date.AddMonths(1);
                    break;

                default:
                    newDate = date.AddDays(1);
                    break;
            }
            return newDate > lastDate ? lastDate : newDate;
        }
    }
}