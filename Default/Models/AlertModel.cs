using System.Collections.Generic;
using mPower.Domain.Application.Enums;

namespace Default.Models
{
    public class AlertModel
    {
        public EmailTypeData TypeData { get; set; }

        public EmailTypeEnum Type { get; set; }

        public bool SendEmail { get; set; }

        public bool SendText { get; set; }

        public int BorderValue { get; set; }
    }
}