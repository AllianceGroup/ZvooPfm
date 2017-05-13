using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;
using mPower.Framework.Utils.Extensions;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class RetirementEstimateModel: IEstimateModel
    {        
        [Range(1, 200, ErrorMessage = "Please enter a valid cuurent age")]
        public int CurrentAge { get; set; }

        [Range(1, 200, ErrorMessage = "Please enter a valid retirement age")]
        public int RetirementAge { get; set; }

        [Range(1, 100000000, ErrorMessage = "Please enter a valid annual income")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal AnnualIncome { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid inflation rate")]
        [PropertyBinder(typeof(PercentBinder))]
        public double InflationRate { get; set; }

        [Range(1, 200, ErrorMessage = "Please enter a valid life expectancy")]
        public int LifeExpectancy { get; set; }

        public InvestmentStyleEnum InvestmentStyle { get; set; }

        public decimal TotalMoney
        {
            get
            {
                try
                {

                    var annualIncome = (double) AnnualIncome;
                    var monthlyIncomeDesired = LoanCalculator.CompoundInterest(annualIncome, InflationRate/100, RetirementAge - CurrentAge);
                    var growthRate = (double) InvestmentStyle;
                    var estimatedValue =   (decimal)
                        LoanCalculator.AnnuitySize(monthlyIncomeDesired, growthRate / 100, LifeExpectancy - RetirementAge);

                    return estimatedValue;


                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public decimal TotalContribution
        {
            get
            {
                try
                {
                    return (decimal)LoanCalculator.PaymentAmountForAnInvestmentGoal((double) TotalMoney, InflationRate/100, RetirementAge - CurrentAge)*(RetirementAge - CurrentAge);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public Decimal Growth
        {
            get { return TotalMoney - TotalContribution; }
        }

        public decimal EstimatedValue { get { return TotalContribution; }}

        public string Title
        {
            get { return "Save for Retirement"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.Retirement; }
        }

        public SelectList InvestmentStylesList
        {
            get
            {
                var dic = Enum.GetValues(typeof (InvestmentStyleEnum)).Cast<InvestmentStyleEnum>().ToDictionary(x => (int)x, x => x.GetDescription());
                var list = new  SelectList(dic, "Key", "Value", (int)InvestmentStyle);
                return list;
            }
        }

        public RetirementEstimateModel()
        {
            RetirementAge = 65;
            InflationRate = 3;
            LifeExpectancy = 79;
        }
    }

    public enum InvestmentStyleEnum
    {
        [Description("Short-Term")]
        ShortTerm = 3,

        Conservative = 4,

        Balanced = 5,

        Growth = 7,

        [Description("Aggresive Growth")]
        AggresiveGrowth = 9,

        [Description("Most Aggresive")]
        MostAggresive = 11,
    }
}