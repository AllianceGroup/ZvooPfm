using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class ImproveHomeEstimateModel: IEstimateModel
    {
        [Range(1, 1000000, ErrorMessage = "Please enter a valid cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Cost { get; set; }

        public decimal EstimatedValue
        {
            get { return IsMultipleSources ? Cost - Amount : Cost; }
        }

        public string Title
        {
            get { return "Improve home"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.ImproveHome; }
        }

        [Range(1, 1000000, ErrorMessage = "Please enter a valid amount to be financed")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Amount { get; set; }

        public bool IsMultipleSources { get; set; }
    }
}