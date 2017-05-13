using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segment_DeleteCommand: Command
    {
        public string AffiliateId { get; set; }

        public string Id { get; set; }
    }
}