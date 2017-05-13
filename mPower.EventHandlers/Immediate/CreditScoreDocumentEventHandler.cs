using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.CreditIdentity.Events;

namespace mPower.EventHandlers.Immediate
{
    public class CreditScoreDocumentEventHandler : IMessageHandler<CreditIdentity_Report_AddedEvent>
    {
        private readonly CreditIdentityDocumentService _creditIdentityService;

        public CreditScoreDocumentEventHandler(CreditIdentityDocumentService creditIdentityService)
        {
            _creditIdentityService = creditIdentityService;
        }

        public void Handle(CreditIdentity_Report_AddedEvent message)
        {
            var reportScore = ToScoreDocument(message.CreditScoreId, message.Data);

            _creditIdentityService.AddCreditScore(message.ClientKey, reportScore);
        }

        private static CreditScoreDocument ToScoreDocument(string id, CreditReportData data)
        {
            return new CreditScoreDocument
                       {
                           Id = id,
                           Score = data.Score,
                           ScoreDate = data.ScoreDate,
                       };
        }
    }
}
