using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Application.Affiliate.Events;
using Paralect.ServiceBus;
using System;
using System.Linq;

namespace mPower.EventHandlers.Eventual
{
    public class CampaignEventHandler : 
        IMessageHandler<Affiliate_Offers_ShownToUserEvent>,
        IMessageHandler<Transaction_CreatedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly UserDocumentService _userService;

        public CampaignEventHandler(AffiliateDocumentService affiliateService, UserDocumentService userService)
        {
            _affiliateService = affiliateService;
            _userService = userService;
        }

        public void Handle(Affiliate_Offers_ShownToUserEvent message)
        {
            foreach (var affiliateId in message.ShownAffiliateOffers.Keys)
            {
                Update(affiliateId, affiliate =>
                {
                    foreach (var offersId in message.ShownAffiliateOffers[affiliate.ApplicationId])
                    {
                        var camp = affiliate.Campaigns.Single(x => x.Id == offersId);
                        camp.Statistic.UsersImpressions++;
                    }
                });
            }
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            var user = _userService.GetById(message.UserId);
            if (user != null)
            {
                Update(user.ApplicationId, affiliate =>
                {
                    var transactionOffers = affiliate.Campaigns.Where(cmp => 
                        cmp.Settings != null 
                        && user.IsOfferAccepted(cmp.Id,user.ApplicationId) 
                        && message.Entries.Any(ent => cmp.MatchesMerchantName(ent.Payee)));

                    foreach (var offer in transactionOffers)
                    {
                        offer.Statistic.Purchases++;
                    }
                });
            }
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