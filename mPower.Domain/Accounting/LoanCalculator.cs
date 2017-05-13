using System;

namespace mPower.Domain.Accounting
{
    public class LoanCalculator
    {
        /// <summary>
        /// Calculates the number of payments required to pay off a loan
        /// </summary>
        /// <param name="amount">Principal balance of the Loan</param>
        /// <param name="Payment">Payment amount per period (usually months)</param>
        /// <param name="InterestRate">Annual interest rate</param>
        /// <returns></returns>
        public static double LoanPayments(double amount, double Payment, double InterestRate, bool round = true)
        {
            // Number of Payments Formula
            //N = −log(1−iA/P) / log(1+i)

            InterestRate = InterestRate/100;

            var exactAmount = -Math.Log10(1 - (InterestRate*amount)/Payment)/Math.Log10(1 + InterestRate);

            return round ? Math.Round(exactAmount, 0) : exactAmount;
        }

        /// <summary>
        /// Calculates the total interest paid on a loan with equal monthly payments
        /// </summary>
        /// <param name="amount">Principal balance of the loan</param>
        /// <param name="monthlyPayment">Monthly payment amount</param>
        /// <param name="interestRate">Annual Interest Rate</param>
        /// <returns></returns>
        public static double TotalInterestPaid(double amount, double monthlyPayment, double interestRate)
        {
            // Total Interest Paid Forumla
            //I = cN - P
            var numberOfPayments = LoanPayments(amount, monthlyPayment, interestRate, false);

            var exactAmount = monthlyPayment * numberOfPayments - amount;

            return Math.Round(exactAmount, 2);
        } 

        /// <summary>
        /// Calculates the balance of an investment over a period of time
        /// </summary>
        /// <param name="Periods">How many periods of time (usually months or years)</param>
        /// <param name="Payment">Payment amount per period</param>
        /// <param name="InterestRatePerPeriod">Interest Rate Per Period</param>
        /// <param name="Amount">Principal balance of the investment</param>
        /// <returns></returns>
        public static double InvestmentBalance(double Periods, double Payment, double InterestRatePerPeriod,
                                               double Amount = 0)
        {
            // Balanance after n Payments forumula
            // B_n = A(1+i)^N + (P/i)[(1+i)^N -1)]

            InterestRatePerPeriod = InterestRatePerPeriod/100;

            var exactAmount = Amount*Math.Pow((1 + InterestRatePerPeriod), Periods) +
                              (Payment/InterestRatePerPeriod)*(Math.Pow((1 + InterestRatePerPeriod), Periods) - 1);

            return Math.Round(exactAmount, 2);
        }

        /// <summary>
        /// Calculate size of payment for one period
        /// </summary>
        /// <param name="Amount">Principal balance of the Loan</param>
        /// <param name="Periods">How many periods of time (usually months or years)</param>
        /// <param name="InterestRatePerPeriod">Interest Rate Per Period</param>
        /// <returns></returns>
        public static double PaymentSize(double Amount, double Periods, double InterestRatePerPeriod, bool round = true)
        {
            // Size of Payment Formula
            // P = iA / (1 - (1 + i)^(-N))

            InterestRatePerPeriod = InterestRatePerPeriod/100;

            var exactAmount = InterestRatePerPeriod*Amount/(1 - Math.Pow(1 + InterestRatePerPeriod, -Periods));

            return round ? Math.Round(exactAmount, 2) : exactAmount;
        }

        
        /// <summary>
        /// Calculates the amount of a loan based on the monthly payment and term on the loan
        /// </summary>
        /// <param name="apr">Annual Percentage Rate of the loan</param>
        /// <param name="months">Term of the loan in months</param>
        /// <param name="monthlyPayment">Monthly Payment</param>
        /// <param name="round">Flags if the amount should be rounded to not include cents</param>
        /// <returns></returns>
        public static double LoanAmountBasedOnMonthlyPayment(double apr, int months, double monthlyPayment, bool round = true)
        {
            // Loan Payment Formula
            // rate = apr / 1200
            // MonthlyPayment = (rate + (rate / (1+rate)^months - 1)) x principal
            //http://www.1728.org/loanform.htm
            
            var rate = Convert.ToDouble(apr/1200);
            
            var core = rate + (rate / (Math.Pow((1 + rate), months) - 1));
            
            var exactAmount = monthlyPayment/core;

            return round ? Math.Round(exactAmount, 0) : exactAmount;

        }

        public static double AnnuitySize(double annualPayment, double interestRate, int years)
        {
            //V = P(1-(1+r)^-n)/r

            return (annualPayment * (1 - Math.Pow(1 + interestRate, -years))/interestRate);

        }

     
        public static double HowMuchHousePaymentICanAfford(double annualIncome, bool aggresive = true, bool round = true)
        {
            var dti = aggresive ? .31 : .28;

            var exactAmount = (annualIncome / 12) * dti;

            return round ? Math.Round(exactAmount, 2) : exactAmount;
        }


        public static double GetHouseCost(double monthlyPayment, double apr, double annualInsurance, double annualPropertyTaxRate)
        {
            var adjustedMonthlyPayment = monthlyPayment - (annualInsurance / 12) - (monthlyPayment * (annualPropertyTaxRate / 12));

            return LoanAmountBasedOnMonthlyPayment(apr, 360, adjustedMonthlyPayment);
        }


        /// <summary>
        /// Calculates the down payment required to purchase a home that someone can afford.
        /// </summary>
        /// <param name="annualIncome"></param>
        /// <param name="apr"></param>
        /// <param name="annualInsurance"></param>
        /// <param name="annualPropertyTaxRate"></param>
        /// <param name="percDownPayment"></param>
        /// <param name="aggressive"></param>
        /// <returns></returns>
        public static double HomeLoanGoal(double annualIncome, double apr, double annualInsurance, double annualPropertyTaxRate, double percDownPayment, bool aggressive = true)
        {
            var monthlyPayment = HowMuchHousePaymentICanAfford(annualIncome, aggressive);

            var principal = GetHouseCost(monthlyPayment, apr, annualInsurance, annualPropertyTaxRate);

            return HomeLoanGoal(principal, percDownPayment);
        }

        public static double HomeLoanGoal(double principal, double percDownPayment)
        {
            return principal * percDownPayment;
        }

        /// <summary>
        /// Calculates the total principal that will be paid over the life of a loan
        /// </summary>
        /// <param name="purchasePrice"></param>
        /// <param name="monthlyPayment"></param>
        /// <param name="loanTermInMonths"></param>
        /// <param name="loanApr"></param>
        /// <param name="tradeInPrice"></param>
        /// <returns></returns>
        public static double PrincipalPaid(double monthlyPayment, int loanTermInMonths, double loanApr)
        {
            var loanAmount = LoanAmountBasedOnMonthlyPayment(loanApr, loanTermInMonths, monthlyPayment);
            var principalPaid = TotalInterestPaid(loanAmount, monthlyPayment, (loanApr / 12));

            return principalPaid;
        }


        /// <summary>
        /// Simple compound interest formula
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="apr"></param>
        /// <param name="years"></param>
        /// <returns></returns>
        public static double CompoundInterest(double principal, double apr, double years)
        {
            //Computnd Interest
            //M = P(1 + i)^n

            var exactAmount = principal * Math.Pow((1 + apr), years);

            return Math.Round(exactAmount, 2);
        }

        public static double PaymentAmountForAnInvestmentGoal(double desiredAmount, double interestRate, int years)
        {
            interestRate = ScrubInterestRate(interestRate);

            // P = iF / (i+i_^n -1

            return (interestRate*desiredAmount)/(Math.Pow(1 + interestRate, years) - 1);


        }

        private static double ScrubInterestRate(double interestRate)
        {
            if (interestRate < 1)
                return interestRate;
            else
            {
                return interestRate/100;
            }
        }

        /// <summary>
        /// Returns the total cost of going to college at at a future time and takes inflation into consideration
        /// </summary>
        /// <param name="yearsInCollege"></param>
        /// <param name="costPerYear"></param>
        /// <param name="inflationRate"></param>
        /// <param name="currentAge"></param>
        /// <param name="collegeAge"></param>
        /// <returns></returns>
        public static double TotalCostOfCollecge(int yearsInCollege, double costPerYear, double inflationRate, int currentAge, int collegeAge)
        {
            double totalCost = 0;

            for (int i = 0; i < yearsInCollege; i++)
            {
                totalCost = totalCost + CompoundInterest(costPerYear, inflationRate, collegeAge - currentAge + i);
            }

            return totalCost;
        }

        public static double TotalCostOfRetirement(int yearsInRetirement, double costPerYear, double inflationRate, int currentAge, int retirementAge)
        {
            return TotalCostOfCollecge(yearsInRetirement, costPerYear, inflationRate, currentAge, retirementAge);

        }

        public static double PriceWithTax(double price, double taxRate)
        {
            return price * (1 + taxRate);
        }

        public static double CarLoanGoal(double vehiclePrice, double taxRate, double monthlyPayment, int loanTermInMonths, double loanApr, double tradeInValue)
        {
            var priceWithTax = PriceWithTax(vehiclePrice, taxRate);
            var loan = PrincipalPaid(monthlyPayment, loanTermInMonths, loanApr);
            return CarLoanGoal(priceWithTax, loan, tradeInValue);
        }

        public static double CarLoanGoal(double vehiclePriceWithTax, double loan, double tradeInValue)
        {
            return vehiclePriceWithTax - loan - tradeInValue;
        }
    }

}
