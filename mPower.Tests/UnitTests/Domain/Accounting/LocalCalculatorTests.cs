using System;
using NUnit.Framework;
using mPower.Domain.Accounting;

namespace mPower.Tests.UnitTests.Domain.Accounting
{
    [TestFixture]
    public class LocalCalculatorTests
    {
        double amount = 100000;
        double payment = 1028.61;
        double interestRate = 1;
       
        [Test]
        public void loan_payments_returns_correct_number_of_payments_required_to_pay_off_a_loan()
        {
            var calc = LoanCalculator.LoanPayments(amount, payment, interestRate);
            Assert.AreEqual(360, calc);
        }

        [Test]
        public void total_interest_paid_returns_total_interest_paid_on_a_loan_with_equal_monthly_payments()
        {
            var calc = LoanCalculator.TotalInterestPaid(amount, payment, interestRate);
            Assert.AreEqual(270308.72, calc);
        }

        [Test]
        public void investment_balance_no_beginning_amount()
        {
            var calc = LoanCalculator.InvestmentBalance(2, 100, 1);
            Assert.AreEqual(201.00, calc);
        }


        [Test]
        public void investment_balance_100_beginning_amount()
        {
            var calc = LoanCalculator.InvestmentBalance(1, 0, 1,100);
            Assert.AreEqual(101.00, calc);
        }


        [Test]
        public void payment_size()
        {
            var calc = LoanCalculator.PaymentSize(150000, 360, 0.666666666666667);
            Assert.AreEqual(calc, 1100.65);
        }

        [Test]
        public void loan_payment_formula()
        {
            var calc = LoanCalculator.LoanAmountBasedOnMonthlyPayment(8, 360, 1100.65);
            Assert.AreEqual(150000, calc);
        }


        [Test]
        public void how_much_can_i_afford()
        {
            var calc = LoanCalculator.HowMuchHousePaymentICanAfford(100000);
            Assert.AreEqual(2583.33, calc);
        }

        [Test]
        
        public void home_loan_goal()
        {
            var calc = LoanCalculator.HomeLoanGoal(100000, 6, 485, 1, .20);
            Assert.AreEqual(77646, calc);
        }


        [Test]
        public void car_savings_goal()
        {
            var calc = LoanCalculator.PrincipalPaid(500, 48,2.99);
            Assert.AreEqual(calc, 1406.16);
        }

        [Test]
        public void compound_interest()
        {
            var calc = LoanCalculator.CompoundInterest(1000, .05, 3);
            Assert.AreEqual(1157.63, calc);
        }

        [Test]
        public void total_cost_of_college()
        {
            const int costPerYear = 15000;
            const double inflationRate = .03;
            const int yearsInCollege = 4;
            const int currentAge = 12;
            const int collegeAge = 18;

            var calc = LoanCalculator.TotalCostOfCollecge(yearsInCollege, costPerYear, inflationRate, currentAge,
                                                          collegeAge);
         
            Assert.AreEqual(74932, Math.Round(calc));
        }
       
        [Test]
        public void annuity_withdraw_calculated_correctly()
        {
            const double annualPayment = 100000.00;
            const double rate = .05;
            const int yearsInRetirement = 40;

            var calc = LoanCalculator.AnnuitySize(annualPayment, rate, yearsInRetirement);

            Assert.AreEqual(1715909, Math.Round(calc));

        }

        [Test]
        public void PaymentAmountForAnInvestmentGoal()
        {
            var calc = LoanCalculator.PaymentAmountForAnInvestmentGoal(27500, .004585, 60);

            Assert.AreEqual(399.22, Math.Round(calc, 2));

        }
    }
}
