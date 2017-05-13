using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Default.Areas.Finance.Controllers;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class TripEstimateModel: IEstimateModel
    {
        [Range(0, 10000, ErrorMessage = "Please enter a valid activities cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Activities { get; set; }

        [Range(0, 10000, ErrorMessage = "Please enter a valid car cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Car { get; set; }

        [Range(0, 10000, ErrorMessage = "Please enter a valid hotel cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Hotel { get; set; }

        [Range(0, 10000, ErrorMessage = "Please enter a valid food cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Food { get; set; }

        [Range(0, 10000, ErrorMessage = "Please enter a valid flight cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Flight { get; set; }

        public decimal EstimatedValue { get
        {
            return Flight*NumberOfTravelers +
                   Car*Duration +
                   Hotel*Duration +
                   Food*Duration +
                   Activities*Duration;
        }}

        public string Title
        {
            get { return "Trip"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.Trip; }
        }

        public string Destination { get; set; }

        [Range(1, 100, ErrorMessage = "Please enter a valid duration")]
        public int Duration { get; set; }

        [Range(1, 100, ErrorMessage = "Please enter a valid number of travelers")]
        public int NumberOfTravelers { get; set; }

    }
}