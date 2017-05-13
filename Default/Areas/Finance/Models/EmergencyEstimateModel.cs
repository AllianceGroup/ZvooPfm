using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class EmergencyEstimateModel: IEstimateModel
    {
        [Range(1, 50000, ErrorMessage = "Please enter a valid average monthly spending")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal AverageMonthlySpending { get; set; }

        [Range(1, 12)]
        public int MonthsCount { get; set; }

        public decimal EstimatedValue
        {
            get { return AverageMonthlySpending*MonthsCount; }
        }

        public string Title
        {
            get { return "Save for an Emergency"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.Emergency; }
        }

        public SelectList MonthsList
        {
            get
            {
                var dic = new Dictionary<int, string>();
                for (var i = 1; i <= 12; i++)
                {
                    dic.Add(i, MonthCountToString(i));
                }
                return new SelectList(dic, "Key", "Value", MonthsCount);
            }
        }

        private static string MonthCountToString(int num)
        {
            return string.Format("{0} {1}", num, num == 1 ? "month" : "months");
        }
    }
}