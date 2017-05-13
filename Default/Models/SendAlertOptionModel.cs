using System.Collections.Generic;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace Default.Models
{
    public class SendAlertOptionModel
    {
        public AlertModeEnum Mode { get; set; }
        public int Count { get; set; }
        public SendAlertTimeRange TimeRange { get; set; }
        public IEnumerable<SelectListItem> Modes { get; set; }

        public SendAlertOptionModel()
        {
            Modes = new SelectList(
                new[]
                    {
                        new {Value = AlertModeEnum.Email, Text = "Email"},
                        new {Value = AlertModeEnum.NotSend, Text = "No alert"}
                    }, "Value", "Text");
        }
    }
}