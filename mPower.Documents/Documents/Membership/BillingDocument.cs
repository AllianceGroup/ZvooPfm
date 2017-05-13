using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.Documents.Documents.Membership
{
    public class BillingDocument
    {
        public DateTime BillDate { get; set; }

        public string MaskedCreditCardNumber { get; set; }

        public long AmountInCents { get; set; }

        public string Status { get; set; }

        public string ProductId { get; set; }

        public string ProductDescription { get; set; }

        public string SubscriptionId { get; set; }
    }
}
