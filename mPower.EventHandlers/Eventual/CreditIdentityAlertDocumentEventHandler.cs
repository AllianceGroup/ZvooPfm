using System.Linq;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Framework.Environment;

namespace mPower.EventHandlers.Eventual
{
    public class CreditIdentityAlertDocumentEventHandler : IMessageHandler<CreditIdentity_Alerts_AddedEvent>
    {
        private readonly CreditIdentityDocumentService _creditIdentityService;
        private readonly IIdGenerator _idGenerator;

        public CreditIdentityAlertDocumentEventHandler(
            CreditIdentityDocumentService creditIdentityService, 
            IIdGenerator idGenerator)
        {
            _creditIdentityService = creditIdentityService;
            _idGenerator = idGenerator;
        }

        public void Handle(CreditIdentity_Alerts_AddedEvent message)
        {
            var alerts = Enumerable.ToList<AlertDocument>(message.Alerts.Select(ad => 
                                                                   new AlertDocument
                                                                       {
                                                                           Id = _idGenerator.Generate(),
                                                                           Date = ad.Date,
                                                                           Type = ad.Type,
                                                                           Message = ad.Message,
                                                                       }));

            _creditIdentityService.AddAlerts(message.ClientKey, alerts);
        }
    }
}
