using System;
using System.Linq;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.ExternalServices;
using mPower.Domain.Membership.User.Events;
using NLog;
using Paralect.ServiceBus;
using TransUnionWrapper;
using mPower.Framework;

namespace mPower.TempDocuments.Server.Handlers
{
    public class ChargifyEventHandler : IMessageHandler<User_Subscription_DeletedEvent>
    {
        private static readonly Logger _logger = MPowerLogManager.CurrentLogger;
        private readonly UserDocumentService _userService;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly ChargifyService _chargifyService;
        private readonly ITransUnionService _transUnionService;
        private readonly CreditIdentityDocumentService _creditIdentityService;

        public ChargifyEventHandler(UserDocumentService userService,
            AffiliateDocumentService affiliateService,
            ChargifyService chargifyService,
            ITransUnionService transUnionService,
            CreditIdentityDocumentService creditIdentityService)
        {
            _userService = userService;
            _affiliateService = affiliateService;
            _chargifyService = chargifyService;
            _transUnionService = transUnionService;
            _creditIdentityService = creditIdentityService;
        }

        public void Handle(User_Subscription_DeletedEvent message)
        {
            try
            {
                var user = _userService.GetById(message.UserId);
                var subscription = user.Subscriptions.Single(x => x.Id == message.SubscriptionId);
                var affiliate = _affiliateService.GetById(user.ApplicationId);
                _chargifyService.Connect(affiliate.ChargifyUrl, affiliate.ChargifySharedKey, affiliate.ChargifyApiKey);
                if (subscription.ChargifySubscriptionId != 0)
                {
                    _chargifyService.CancelSubscription(subscription.ChargifySubscriptionId, message.CancelMessage);
                }

                if (String.IsNullOrEmpty(message.CreditIdentityId)) // unsubscribe from main product -- unsubscribe from all alerts
                {
                    var identities = _creditIdentityService.GetCreditIdentitiesByUserId(message.UserId);

                    foreach (var identity in identities)
                    {
                        _transUnionService.UnSubscribeToAlerts(identity.ClientKey);
                    }
                }
                else
                {
                    _transUnionService.UnSubscribeToAlerts(message.CreditIdentityId);
                }
            }
            catch (Exception e)
            {

                _logger.LogException(LogLevel.Error, "Error cancelling subscription", e);
            }
        }
    }
}
