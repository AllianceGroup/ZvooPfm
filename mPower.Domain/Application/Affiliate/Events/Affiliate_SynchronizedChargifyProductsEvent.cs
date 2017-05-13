using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_SynchronizedChargifyProductsEvent : Event
    {
        public string AffiliateId { get; set; }

        public List<ChargifyProductData> Products { get; set; }
    }
}
