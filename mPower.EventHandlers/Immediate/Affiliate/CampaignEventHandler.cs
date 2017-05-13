using System;
using System.Linq;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Events;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class CampaignEventHandler:
        IMessageHandler<Affiliate_Campaign_CreatedEvent>,
        IMessageHandler<Affiliate_Campaign_Offer_UpdatedEvent>,
        IMessageHandler<Affiliate_Campaign_Settings_UpdatedEvent>,
        IMessageHandler<Affiliate_Offer_AcceptedByUserEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;

        public CampaignEventHandler(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public void Handle(Affiliate_Campaign_CreatedEvent message)
        {
            var campaign = new CampaignDocument
            {
                Id = message.CampaignId,
                SegmentId = message.SegmentId,
                Offer = message.Offer,
            };

            Update(message.AffiliateId, affiliate => affiliate.Campaigns.Add(campaign));
        }

        public void Handle(Affiliate_Campaign_Offer_UpdatedEvent message)
        {
            Update(message.AffiliateId, affiliate => 
                affiliate.UpdateCampaign(message.CampaignId, campaign => 
                    campaign.Offer = message.Offer));
        }

        public void Handle(Affiliate_Campaign_Settings_UpdatedEvent message)
        {
            Update(message.AffiliateId, affiliate =>
                affiliate.UpdateCampaign(message.CampaignId, campaign =>
                    campaign.Settings = message.Settings));
        }

        public void Handle(Affiliate_Offer_AcceptedByUserEvent message)
        {
            Update(message.OfferAffiliateId, affiliate =>
            {
                var camp = affiliate.Campaigns.Single(x => x.Id == message.OfferId);
                if (!camp.Statistic.AcceptedByUsers.Contains(message.UserId))
                {
                    camp.Statistic.AcceptedByUsers.Add(message.UserId);
                }
            });
        }

        private void Update(string affiliateId, Action<AffiliateDocument> updater)
        {
            var affiliate = _affiliateService.GetById(affiliateId);
            if (affiliate != null)
            {
                updater(affiliate);
                _affiliateService.Save(affiliate);
            }
        }
    }
}