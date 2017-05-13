using System;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;

namespace mPower.EventHandlers.Eventual
{
    public class UserDocumentEventHandler :
        IMessageHandler<User_LoggedInEvent>,
        IMessageHandler<Ledger_Account_AddedEvent>,
        IMessageHandler<Affiliate_Offer_AcceptedByUserEvent>
    {
        private readonly UserDocumentService _userService;
        private readonly LedgerDocumentService _ledgerService;
        private readonly AffiliateDocumentService _affiliateService;

        public UserDocumentEventHandler(UserDocumentService userService, LedgerDocumentService ledgerService, AffiliateDocumentService affiliateService)
        {
            _userService = userService;
            _ledgerService = ledgerService;
            _affiliateService = affiliateService;
        }

        public void Handle(User_LoggedInEvent message)
        {
            _userService.SetLastLogin(message.UserId, message.Date, message.AuthToken);
        }

        public void Handle(Ledger_Account_AddedEvent message)
        {
            if (message.Aggregated)
            {
                var ledger = _ledgerService.GetById(message.LedgerId);
                foreach (var user in ledger.Users)
                {
                    _userService.IncreaseAggregatedAccountsCounter(user.Id);
                }
            }
        }

        public void Handle(Affiliate_Offer_AcceptedByUserEvent message)
        {
            var affiliate = _affiliateService.GetById(message.OfferAffiliateId);
            var offer = affiliate.Campaigns.Find(x => x.Id == message.OfferId);
            Update(message.UserId,
                   user =>
                   user.AcceptOffer(message.OfferId, message.Date, message.OfferAffiliateId, offer.Offer.OfferType,
                                    offer.Offer.OfferValueInCents, offer.Offer.OfferValueInPerc));
        }

        private void Update(string affiliateId, Action<UserDocument> updater)
        {
            var affiliate = _userService.GetById(affiliateId);
            if (affiliate != null)
            {
                updater(affiliate);
                _userService.Save(affiliate);
            }
        }
    }
}