using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class GoalViewModel
    {
        #region Properties

        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime PlannedDate
        {
            get { return new DateTime(PlannedYear, PlannedMonth, 1); }
            set
            {
                PlannedYear = value.Year;
                PlannedMonth = value.Month;
            }
        }
        
        public DateTime StartDate
        {
            get { return new DateTime(StartYear, StartMonth, 1); }
            set
            {
                StartYear = value.Year;
                StartMonth = value.Month;
            }
        }

        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Monthly Contribution should be greater than 0")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal MonthlyContribution { get; set; }

        [Range(1, (double)decimal.MaxValue)]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal Amount { get; set; }

        [Range(1, (double)decimal.MaxValue)]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal StartingBalance { get; set; }

        public decimal Contributed { get; set; }

        [Range(1, 12)]
        public int PlannedMonth { get; set; }

        public IEnumerable<SelectListItem> PlannedMonthes { get; set; }

        public int PlannedYear { get; set; }

        public IEnumerable<SelectListItem> PlannedYears { get; set; }

        public GoalTypeEnum Type { get; set; }

        public String BackLinkAction { get; set; }
        
        public String EstimateAction
        {
            get
            {
                switch (Type)
                {
                    case GoalTypeEnum.Emergency:
                        return "Emergency";
                    case GoalTypeEnum.Retirement:
                        return "Retirement";
                    case GoalTypeEnum.BuyHome:
                        return "BuyHome";
                    case GoalTypeEnum.BuyCar:
                        return "Car";
                    case GoalTypeEnum.College:
                        return "College";
                    case GoalTypeEnum.Trip:
                        return "Trip";
                    case GoalTypeEnum.ImproveHome:
                        return "ImproveHome";
                    case GoalTypeEnum.Custom:
                        return "Custom";
                    default:
                        return null;
                }
            }
        }

        public String StandartImage
        {
            get
            {
                switch (Type)
                {
                    case GoalTypeEnum.Emergency:
                        return "goal-emergency.png";
                    case GoalTypeEnum.Retirement:
                        return "goal-retirement.png";
                    case GoalTypeEnum.BuyHome:
                        return "goal-house.png";
                    case GoalTypeEnum.BuyCar:
                        return "goal-car.png";
                    case GoalTypeEnum.College:
                        return "goal-college.png";
                    case GoalTypeEnum.Trip:
                        return "goal-trip.png";
                    case GoalTypeEnum.ImproveHome:
                        return "goal-home-improvement.png";

                    default:
                        return "goal-avatar-example.png";
                }
            }
        }

        public int StartYear { get; set; }

        public int StartMonth { get; set; }

        public DateTime? ProjectedDate 
        {
            get { return MonthlyContribution > 0 ? PlannedDate.AddDays((double)(Amount / MonthlyContribution - PeriodInMonths) * 365 / 12) : (DateTime?)null; }
        }

        public decimal RoundedMonthlyContribution { get { return Math.Round(MonthlyContribution, 2); } }

        public int PeriodInMonths { get { return GetMonthsDuration(StartDate, PlannedDate); } }

        public string Summary
        {
            get
            {
                var projDate = ProjectedDate;
                if (!projDate.HasValue)
                {
                    return string.Empty;
                }

                string template;
                var diff = GetMonthsDuration(projDate.Value, PlannedDate);
                if (diff > 0)
                {
                    template = "You are ahead of schedule! By saving <strong>{0} per month</strong>, we project that you will reach your goal <strong>{1}</strong> before your planned date of <strong>{2}</strong>";
                }
                else if (diff < 0)
                {
                    template = "You aren’t saving enough money each month in order to reach your goal on time! By saving <strong>{0} per month</strong>, we project that you will reach your goal <strong>{1}</strong> after your planned date of <strong>{2}</strong>";
                }
                else
                {
                    template = "Nice! By saving <strong>{0} per month</strong>, we project that you will reach your goal by your planned date of <strong>{2}</strong>";
                }
                return string.Format(template, MonthlyContribution.ToString("C0"), MonthCountToLongString((uint)Math.Abs(diff)), PlannedDate.ToString("MMMM, yyyy"));
            }
        }

        public string DateAway
        {
            get { return PeriodInMonths < 0 ? string.Empty : MonthCountToLongString((uint)PeriodInMonths) + " away"; }
        }

        #endregion

        #region Constructor

        public GoalViewModel()
        {
            var now = DateTime.Now;
            StartDate = now;
            PlannedDate = now.AddYears(1);

            var list = new List<SelectListItem>();
            for (var i = 1; i < 13; i++)
            {
                list.Add(CreateSelectListItem(GetMonthName(i), i.ToString(CultureInfo.InvariantCulture)));
            }
            PlannedMonthes = list;

            list = new List<SelectListItem>();
            for (var i = 0; i < 100; i++)
            {
                list.Add(CreateSelectListItem((StartYear + i).ToString(CultureInfo.InvariantCulture)));
            }
            PlannedYears = list;
        }

        #endregion

        #region Helper Methods

        private static SelectListItem CreateSelectListItem(string nameValue)
        {
            return CreateSelectListItem(nameValue, nameValue);
        }

        private static SelectListItem CreateSelectListItem(string name, string value)
        {
            return new SelectListItem { Text = name, Value = value };
        }

        private static string GetMonthName(int month)
        {
            return new DateTime(1900, month, 1).ToString("MMMM");
        }

        private static string MonthCountToLongString(uint count)
        {
            var yearsCount = count/12;
            var monthCount = count%12;

            if (yearsCount > 0 && monthCount > 0)
            {
                return string.Format("{0}, {1}", YearCountToString(yearsCount), MonthCountToString(monthCount));
            }
            if (yearsCount > 0)
            {
                return YearCountToString(yearsCount);
            }
            return MonthCountToString(monthCount);
        }

        private static string MonthCountToString(uint count)
        {
            return string.Format("{0} {1}", count, Math.Abs(count) == 1 ? "month" : "months");
        }

        private static string YearCountToString(uint count)
        {
            return string.Format("{0} {1}", count, Math.Abs(count) == 1 ? "year" : "years");
        }

        private static int GetMonthsDuration(DateTime start, DateTime end)
        {
            const double monthLenthInDays = 365/12.0;
            return (int)Math.Round((end - start).TotalDays / monthLenthInDays);
        }

        #endregion
    }
}