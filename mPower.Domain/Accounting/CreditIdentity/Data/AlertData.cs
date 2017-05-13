using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class AlertData
    {
        public DateTime Date { get; set; }

        public AlertTypeEnum Type { get; set; }

        public string Message { get; set; }
    }
}