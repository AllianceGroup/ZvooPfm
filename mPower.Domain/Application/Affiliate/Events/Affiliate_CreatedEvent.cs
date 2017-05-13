using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_CreatedEvent : Event
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
