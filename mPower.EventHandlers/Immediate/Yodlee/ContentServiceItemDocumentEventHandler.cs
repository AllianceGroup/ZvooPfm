using System;
using MongoDB.Driver.Builders;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Yodlee.Storage.Documents;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate.Yodlee
{
    public class ContentServiceItemDocumentEventHandler 
        //: IMessageHandler<Ledger_Account_RemovedEvent>
    {
        private readonly ContentServiceItemDocumentService _documentService;

        public ContentServiceItemDocumentEventHandler(ContentServiceItemDocumentService documentService)
        {
            _documentService = documentService;
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var query =  Query.EQ("Accounts.LedgerAccountId", message.AccountId);
            
            var update = Update.Set("Accounts.$.LedgerId", String.Empty)
                .Set("Accounts.$.LedgerAccountId", String.Empty)
                .Set("Accounts.$.IsMapped", false);

            _documentService.Update(query, update);
        }
    }
}
