using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Application.Affiliate.Events;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class EmailContentEventHandler :
        IMessageHandler<Affiliate_Email_Content_AddedEvent>,
        IMessageHandler<Affiliate_Email_Content_ChangedEvent>,
        IMessageHandler<Affiliate_Email_Content_DeletedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;

        public EmailContentEventHandler(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public void Handle(Affiliate_Email_Content_AddedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var content = new EmailContentDocument
            {
                Id = message.Id,
                TemplateId = message.TemplateId,
                Name = message.Name,
                Subject = message.Subject,
                Html = message.Html,
                IsDefaultForEmailType = message.IsDefaultForEmailType,
                CreationDate = message.CreationDate,
                Status = message.Status,
            };
            var update = Update.PushWrapped("EmailContents", content);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Email_Content_ChangedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.AffiliateId), Query.EQ("EmailContents._id", message.Id));
            var update = Update.Set("EmailContents.$.TemplateId", BsonValue.Create(message.TemplateId) ?? BsonNull.Value)
                .Set("EmailContents.$.Name", BsonValue.Create(message.Name) ?? BsonNull.Value)
                .Set("EmailContents.$.Subject", BsonValue.Create(message.Subject) ?? BsonNull.Value)
                .Set("EmailContents.$.Html", BsonValue.Create(message.Html) ?? BsonNull.Value)
                .Set("EmailContents.$.Status", message.Status);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Email_Content_DeletedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var innerQuery = Query.EQ("_id", message.Id);
            var update = Update.Pull("EmailContents", innerQuery);

            _affiliateService.Update(query, update);
        }
    }
}