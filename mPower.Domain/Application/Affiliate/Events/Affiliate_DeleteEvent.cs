using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_DeleteEvent : Event
    {
        public string Id { get; set; }
    }
}
