using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Events;

namespace mPower.EventHandlers.Immediate
{
    public class ImportedTransactionDocumentEventHandler :
        IMessageHandler<Transaction_CreatedEvent>,
        IMessageHandler<Ledger_DeletedEvent>
    {
        private readonly ImportedTransactionDocumentService _importedTransactions;

        public ImportedTransactionDocumentEventHandler(ImportedTransactionDocumentService importedTransactions)
        {
            _importedTransactions = importedTransactions;
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            if (message.Imported == false || string.IsNullOrEmpty(message.ImportedTransactionId))
                return;

            _importedTransactions.Insert(new ImportedTransactionDocument()
            {
                LedgerId = message.LedgerId,
                TransactionId = message.TransactionId,
                ImportedTransactionId = message.ImportedTransactionId
            });
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            _importedTransactions.Remove(Query.EQ("LedgerId", message.LedgerId));
        }
    }
}
