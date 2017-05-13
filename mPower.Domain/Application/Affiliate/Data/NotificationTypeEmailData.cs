using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class NotificationTypeEmailData
    {
        public string Name { get; set; }

        public EmailTypeEnum EmailType { get; set; }

        public string EmailContentId { get; set; }

        public TriggerStatusEnum Status { get; set; }
    }
}