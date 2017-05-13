using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Application.Enums;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class NotificationTypeEmailEventHandler :
        IMessageHandler<Affiliate_NotificationTypeEmail_AddedEvent>,
        IMessageHandler<Affiliate_NotificationTypeEmail_ChangedEvent>,
        IMessageHandler<Affiliate_Email_Content_ChangedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;

        public NotificationTypeEmailEventHandler(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public void Handle(Affiliate_NotificationTypeEmail_AddedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var notificationTypeEmail = new NotificationTypeEmailDocument
            {
                Name = message.Name,
                EmailType = message.EmailType,
                EmailContentId = message.EmailContentId,
                Status = message.Status,
            };
            var update = Update.PushWrapped("NotificationTypeEmails", notificationTypeEmail);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_NotificationTypeEmail_ChangedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.AffiliateId),
                                  Query.EQ("NotificationTypeEmails._id", message.EmailType));
            var update = Update.Set("NotificationTypeEmails.$.EmailContentId", BsonValue.Create(message.EmailContentId) ?? BsonNull.Value)
                .Set("NotificationTypeEmails.$.Status", message.Status);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Email_Content_ChangedEvent message)
        {
            if (message.Status != TemplateStatusEnum.Active)
            {
                var query = Query.And(Query.EQ("_id", message.AffiliateId), Query.EQ("NotificationTypeEmails.EmailContentId", message.Id));
                var update = Update.Set("NotificationTypeEmails.$.Status", TriggerStatusEnum.Inactive);

                _affiliateService.Update(query, update);
            }
        }
    }
}