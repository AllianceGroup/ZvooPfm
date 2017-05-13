using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace Default.Areas.Finance.Models
{
    public class SteppedSliderModel
    {
        public string Id { get; set; }

        public SelectList SelectList { get; set; }

        public bool IsVertical { get; set; }

        public GoalTypeEnum? GoalType { get; set; }
    }
}