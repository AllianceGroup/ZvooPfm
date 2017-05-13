using System;

namespace mPower.Domain.Accounting.Calendar.Data
{
    public class RepeatingEventEndOption
    {
        public bool Never { get; set; }
        public int? After { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
