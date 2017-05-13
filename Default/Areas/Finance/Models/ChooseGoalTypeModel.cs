using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace Default.Areas.Finance.Models
{
    public class ChooseGoalTypeModel
    {
        private IEnumerable<SelectListItem> GoalTypes { get; set; }

        public ChooseGoalTypeModel()
        {
            GoalTypes = BuildGoalTypesList();
        }

        private IEnumerable<SelectListItem> BuildGoalTypesList()
        {
            var names = Enum.GetNames(typeof (GoalTypeEnum)).ToList();
            var values = Enum.GetValues(typeof (GoalTypeEnum)).Cast<List<object>>();
            return names.Zip(values, (name, value) => CreateSelectListItem(name, value.ToString()));
        }

        private SelectListItem CreateSelectListItem(String name, String value, bool selected = false)
        {
            return new SelectListItem() {Selected = selected, Text = name, Value = value};
        }
    }
}