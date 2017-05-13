using System.Linq;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class TransactionDuplicateDocumentEventHandler :
        IMessageHandler<Transaction_DuplicateCreatedEvent>,
        IMessageHandler<Transaction_DuplicateDeletedEvent>,
        IMessageHandler<Ledger_DeletedEvent>
    {
        private readonly TransactionDuplicateDocumentService _transactionDocumentService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransactionDuplicateDocumentEventHandler(TransactionDuplicateDocumentService transactionDocumentService)
        {
            _transactionDocumentService = transactionDocumentService;
        }

        public void Handle(Transaction_DuplicateCreatedEvent message)
        {
            var doc = new TransactionDuplicateDocument()
                          {
                              BaseTransactionId = message.BaseTransaction.TransactionId,
                              LedgerId = message.BaseTransaction.LedgerId,
                              BaseTransaction = Map(message.BaseTransaction),
                              PotentialDuplicates = message.PotentialDuplicates.Select(Map).ToList()
                          };

            _transactionDocumentService.Save(doc);
        }


        public void Handle(Transaction_DuplicateDeletedEvent message)
        {
            var query = Query.And(Query.EQ("LedgerId", message.LedgerId), Query.EQ("_id", message.Id));

            _transactionDocumentService.Remove(query);
        }

        private TransactionDuplicateDataDocument Map(ExpandedEntryData data)
        {
           return  new TransactionDuplicateDataDocument()
                       {
                           Date = data.BookedDate,
                           TransactionId = data.TransactionId,
                           AccountName = data.AccountName,
                           Memo = data.Memo,
                           Payee = data.Payee,
                           OffsetAccountName = data.OffsetAccountName,
                           OffsetAccountId = data.OffsetAccountId,
                           FormattedAmount = AccountingFormatter.ConvertToDollarsThenFormat(data.DebitAmountInCents - data.CreditAmountInCents, true)
                       };
        }


        public void Handle(Ledger_DeletedEvent message)
        {
            _transactionDocumentService.Remove(Query.EQ("LedgerId", message.LedgerId));
        }
    }
}
