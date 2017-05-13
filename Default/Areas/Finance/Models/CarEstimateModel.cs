using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class CarEstimateModel : IEstimateModel
    {
        public bool IsFinancing { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid tax rate")]
        [PropertyBinder(typeof(PercentBinder))]
        public double TaxRate { get; set; }

        [Range(1, 10000000, ErrorMessage = "Please enter a valid cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Cost { get; set; }

        [Range(1, 1000000, ErrorMessage = "Please enter a valid monthly payment")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal MonthlyPayment { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a valid credit duration")]
        public int CreditDuration { get; set; }

        public SelectList CreditDurations
        {
            get
            {
                var periodsInMonths = new[] { 36, 48, 60, 72, 84 };
                var listItems = periodsInMonths.ToDictionary(x => x, x => string.Format("{0} months", x));
                return new SelectList(listItems, "Key", "Value", CreditDuration);
            }
        }

        [Range(1, 100, ErrorMessage = "Please enter a valid loan annual rate")]
        [PropertyBinder(typeof(PercentBinder))]
        public double LoanAnnualPercRate { get; set; }

        private decimal _tradingVehicleCost;

        [Range(1, 10000000, ErrorMessage = "Please enter a valid trading vehicle cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal TradingVehicleCost
        {
            get { return IsTradingVehicleCostInputVisible ? _tradingVehicleCost : 0; }
            set { _tradingVehicleCost = value; }
        }

        public string TradingVehicle { get; set; }

        public SelectList TradingVehicles
        {
            get
            {
                var items = new Dictionary<string, string>
                {
                    {"worth", "a vehicle worth"},
                    {"none", "none"},
                };
                return new SelectList(items, "Key", "Value");
            }
        }

        public bool IsTradingVehicleCostInputVisible {get { return String.IsNullOrEmpty(TradingVehicle) || TradingVehicle.ToLower() != "none"; }}

        public decimal EstimatedValue
        {
            get { return (decimal)LoanCalculator.CarLoanGoal((double)CostWithTaxs, (double)Loan, (double)TradingVehicleCost); }
        }

        public string Title
        {
            get { return "Buy a Car"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.BuyCar; }
        }

        public decimal CostWithTaxs { get { return Cost*(decimal) (100 + TaxRate)/100; } }

        public decimal Loan
        {
            get
            {
                return IsFinancing && MonthlyPayment > 0 && CreditDuration > 0 && LoanAnnualPercRate > 0
                    ? (decimal)LoanCalculator.LoanAmountBasedOnMonthlyPayment(LoanAnnualPercRate, CreditDuration, (double)MonthlyPayment)
                    : 0;
            }
        }

        public CarEstimateModel()
        {
            TaxRate = 4.7;
            CreditDuration = 48;
            LoanAnnualPercRate = 2.99;
        }
    }
}