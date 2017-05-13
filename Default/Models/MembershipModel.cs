using System.Collections.Generic;
using mPower.Documents.Documents.Membership;

namespace Default.Models
{
    public class MembershipModel
    {
        public List<SubscriptionDocument> Subscriptions { get; set; }

        public List<BillingDocument> Bills { get; set; }
    }
}