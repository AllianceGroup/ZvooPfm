using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Content_DeleteCommand : Command
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }
    }
}