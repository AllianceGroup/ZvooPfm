using System.Linq;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework;
using mPower.Framework.Environment;

namespace mPower.EventHandlers.Eventual
{
    public class TransactionDocumentEventHandler :
        IMessageHandler<Transaction_CreatedEvent>,
        IMessageHandler<Transaction_ModifiedEvent>,
        IMessageHandler<Transaction_DeletedEvent>,
        IMessageHandler<Ledger_DeletedEvent>
    {
        private readonly TransactionDocumentService _transactionDocumentService;
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransactionDocumentEventHandler(TransactionDocumentService transactionDocumentService,
                                               IEventService eventService)
        {
            _transactionDocumentService = transactionDocumentService;
            _eventService = eventService;
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            var transaction = new TransactionDocument
            {
                Id = message.TransactionId,
                LedgerId = message.LedgerId,
                Type = message.Type,
                Memo = message.Memo,
                BookedDate = message.Entries[0].BookedDate,
                ReferenceNumber = message.ReferenceNumber,
                BaseEntryAccountId = message.BaseEntryAccountId,
                BaseEntryAccountType = message.BaseEntryAccountType,
                Imported = message.Imported
            };

            foreach (var entry in message.Entries)
            {
                transaction.Entries.Add(ToTransactionEntryDocument(entry));
            }

            _transactionDocumentService.Insert(transaction);
        }

        public void Handle(Transaction_ModifiedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.TransactionId),
                Query.EQ("LedgerId", message.LedgerId));

            var update = Update<TransactionDocument>.Set(x=> x.Type, message.Type)
                .Set(x => x.ReferenceNumber, message.ReferenceNumber)
                .Set(x => x.BaseEntryAccountId, message.BaseEntryAccountId)
				.Set(x => x.BookedDate, message.Entries.First().BookedDate)
                .Set(x => x.Memo, message.Memo)
				.Set(x => x.Entries, message.Entries.Select(ToTransactionEntryDocument).ToList());
				
            _transactionDocumentService.Update(query, update);
        }

        public void Handle(Transaction_DeletedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.TransactionId),
                                  Query.EQ("LedgerId", message.LedgerId));


            _transactionDocumentService.Remove(query);
        }

        private static TransactionEntryDocument ToTransactionEntryDocument(ExpandedEntryData entry)
        {
            return new TransactionEntryDocument
            {
                AccountId = entry.AccountId,
                BookedDate = entry.BookedDate,
                CreditAmountInCents = entry.CreditAmountInCents,
                DebitAmountInCents = entry.DebitAmountInCents,
                BookedDateString = entry.BookedDate.ToString("MM-dd-yyyy"),
                Payee = entry.Payee,
                Memo = entry.Memo,
                AccountLabel = entry.AccountLabel
            };
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var filter = new TransactionFilter { LedgerId = message.LedgerId };
            _transactionDocumentService.Remove(filter);
        }


        //public void Handle(Transaction_ConfirmedNotDuplicatedEvent message)
        //{
        //    var query = Query.And(Query.EQ("_id", message.TransactionId),
        //        Query.EQ("LedgerId", message.LedgerId));

        //    var update = Update.Set(x => x.ConfirmedNotDuplicate, true);

        //    _transactionDocumentService.Update(query, update);
        //}
    }
}
