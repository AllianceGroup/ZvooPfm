using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Events;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate.Affiliate
{
   public class FaqEventHandler :
        IMessageHandler<Affiliate_Faq_AddedEvent>,
        IMessageHandler<Affiliate_Faq_UpdatedEvent>,
        IMessageHandler<Affiliate_Faq_DeletedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;

        public FaqEventHandler(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public void Handle(Affiliate_Faq_AddedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var faq = new FaqDocument
            {
                Id = message.Id,
                Name = message.Name,
                Html = message.Html,
                IsActive = message.IsActive,
                CreationDate = message.CreationDate,
            };
            var update = Update.PushWrapped("FaqDocuments", faq);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Faq_UpdatedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.AffiliateId), Query.EQ("FaqDocuments._id", message.Id));
            var update = Update.Set("FaqDocuments.$.Name", BsonValue.Create(message.Name) ?? BsonNull.Value)
                .Set("FaqDocuments.$.Html", BsonValue.Create(message.Html) ?? BsonNull.Value)
                .Set("FaqDocuments.$.IsActive", message.IsActive);

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Faq_DeletedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var innerQuery = Query.EQ("_id", message.Id);
            var update = Update.Pull("FaqDocuments", innerQuery);

            _affiliateService.Update(query, update);
        }
    }
}