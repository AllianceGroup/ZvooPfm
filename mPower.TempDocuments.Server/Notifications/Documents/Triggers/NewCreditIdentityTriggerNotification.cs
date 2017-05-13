using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.TempDocuments.Server.Notifications.Documents.Triggers
{
    public class NewCreditIdentityTriggerNotification : BaseNotification
    {
        public string CreditIdentityId { get; set; }

        public string CreditIdentitySocial { get; set; }
    }
}
