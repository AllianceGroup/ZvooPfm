using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Application.Affiliate.Events;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class EmailTemplateEventHandler : 
        IMessageHandler<Affiliate_Email_Template_AddedEvent>,
        IMessageHandler<Affiliate_Email_Template_ChangedEvent>,
        IMessageHandler<Affiliate_Email_Template_DeletedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;

        public EmailTemplateEventHandler(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public void Handle(Affiliate_Email_Template_AddedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var template = new EmailTemplateDocument
            {
                Id = message.Id, 
                Name = message.Name, 
                Html = message.Html, 
                IsDefault = message.IsDefault,
                CreationDate = message.CreationDate,
                Status = message.Status,
            };
            var update = Update.PushWrapped("EmailTemplates", template);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Email_Template_ChangedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.AffiliateId), Query.EQ("EmailTemplates._id", message.Id));
            var update = Update.Set("EmailTemplates.$.Name", BsonValue.Create(message.Name) ?? BsonNull.Value)
                .Set("EmailTemplates.$.Html", BsonValue.Create(message.Html) ?? BsonNull.Value)
                .Set("EmailTemplates.$.Status", message.Status);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Email_Template_DeletedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var innerQuery = Query.EQ("_id", message.Id);
            var update = Update.Pull("EmailTemplates",  innerQuery);

            _affiliateService.Update(query, update);
        }
    }
}