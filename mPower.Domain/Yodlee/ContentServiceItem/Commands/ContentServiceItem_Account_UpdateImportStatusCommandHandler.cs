using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Yodlee.Storage.Documents;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_Account_UpdateImportStatusCommandHandler : IMessageHandler<ContentServiceItem_Account_UpdateImportStatusCommand>
    {
        private readonly IRepository _repository;
        private readonly ContentServiceItemDocumentService _docService;

        public ContentServiceItem_Account_UpdateImportStatusCommandHandler(IRepository repository, ContentServiceItemDocumentService docService)
        {
            _repository = repository;
            _docService = docService;
        }

        #region IMessageHandler<ContentServiceItem_UpdateCommand> Members

        public void Handle(ContentServiceItem_Account_UpdateImportStatusCommand message)
        {
            var query = Query.And(Query.EQ("_id", message.ItemId), Query.EQ("Accounts.AccountId", message.ContentServiceItemAccountId));

            var update = Update.Set("Accounts.$.ImportStatus", message.ImportStatus)
                .Set("Accounts.$.LedgerId", BsonValue.Create(message.LedgerId) ?? BsonNull.Value)
                .Set("Accounts.$.LedgerAccountId", BsonValue.Create(message.LedgerAccountId) ?? BsonNull.Value)
                .Set("Accounts.$.IsMapped", true);

            _docService.Update(query, update);
        }

        #endregion
    }
}