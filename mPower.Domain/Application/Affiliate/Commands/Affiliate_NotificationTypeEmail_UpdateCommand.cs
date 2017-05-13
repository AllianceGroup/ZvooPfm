using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_NotificationTypeEmail_UpdateCommand : Command
    {
        public string AffiliateId { get; set; }

        public EmailTypeEnum EmailType { get; set; }

        public string EmailContentId { get; set; }

        public TriggerStatusEnum Status { get; set; }
    }
}