using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.TempDocuments.Server.Notifications.Documents.Triggers
{
    public class NewAggregatedAccountTriggerNotification : BaseNotification
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }
    }
}
