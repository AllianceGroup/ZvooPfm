using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class BuyHomeEstimateModel: IEstimateModel
    {
        [Range(1, 100000000, ErrorMessage = "Please enter a valid annual insurance")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal AnnualInsurance { get; set; }

        [Range(1, 100000000, ErrorMessage = "Please enter a valid annual income")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal AnnualIncome { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid mortgage rate")]
        [PropertyBinder(typeof(PercentBinder))]
        public double MortgageRate { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid percent down payment")]
        [PropertyBinder(typeof(PercentBinder))]
        public double PercentDownPayment { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid annual property tax")]
        [PropertyBinder(typeof(PercentBinder))]
        public double AnnualPropertyTax { get; set; }

        [Range(1, 1000000, ErrorMessage = "Please enter a valid monthly payment")]
        public decimal MonthlyPayment 
        {
            get { return (decimal)LoanCalculator.HowMuchHousePaymentICanAfford((double)AnnualIncome, IsAgressive); }
        }

        public bool IsAgressive { get; set; }

        public decimal HomeCost 
        {
            
            get
            {
                try
                {
                    return (decimal)LoanCalculator.GetHouseCost((double)MonthlyPayment, MortgageRate, (double)AnnualInsurance, AnnualPropertyTax);
                }
                catch(OverflowException)
                {
                    return 0;
                }
               
            }
        }

        public decimal EstimatedValue
        {
            get { return (decimal)LoanCalculator.HomeLoanGoal((double)HomeCost, PercentDownPayment / 100); }
        }

   

        public string Title
        {
            get { return "Buy a Home"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.BuyHome; }
        }

        public BuyHomeEstimateModel()
        {
            IsAgressive = true;
        }

        [Range(0, 100000000, ErrorMessage = "Please enter a valid home cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal CustomHomeCost { get; set; }
    }
}