using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.EventHandlers.Eventual;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class LuceneEntryDocumentEventHandler :
        IMessageHandler<Transaction_CreatedEvent>,
        IMessageHandler<Transaction_ModifiedEvent>,
        IMessageHandler<Transaction_DeletedEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>,
        IMessageHandler<Transaction_DeleteMultipleMessage>,
        IMessageHandler<Transaction_CreateMultipleMessage>
    {
        private readonly EntryDocumentService _entryDocumentService;
        private readonly TransactionDocumentService _transactionDocumentService;
        private readonly TransactionLuceneService _transactionLuceneService;

        private bool IsReadModelRegeneration
        {
            get { return !_transactionLuceneService.IsImmediateFlush; }
        }

        public LuceneEntryDocumentEventHandler(
            EntryDocumentService entryDocumentService,
            TransactionDocumentService transactionDocumentService,
            TransactionLuceneService transactionLuceneService)
        {
            _entryDocumentService = entryDocumentService;
            _transactionDocumentService = transactionDocumentService;
            _transactionLuceneService = transactionLuceneService;
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            if (!message.IsMultipleInsert || IsReadModelRegeneration)
                SaveEntries(message.Entries, message.TransactionId, message.LedgerId);
        }

        public void Handle(Transaction_CreateMultipleMessage message)
        {
            if (_transactionLuceneService.IsImmediateFlush)
                _transactionLuceneService.WillDoManualFlush();

            foreach (var dto in message.Transactions)
            {
                SaveEntries(dto.Entries, dto.TransactionId, dto.LedgerId);
            }

            if (_transactionLuceneService.IsManualFlush)
                _transactionLuceneService.Flush(true);
        }

        public void Handle(Transaction_ModifiedEvent message)
        {
            RemoveEntries(message.TransactionId);
            SaveEntries(message.Entries, message.TransactionId, message.LedgerId);
        }

        public void Handle(Transaction_DeletedEvent message)
        {
            if (!message.IsMultipleDelete || IsReadModelRegeneration) // we'll handle this via Transaction_DeleteMultipleMessage
                RemoveEntries(message.TransactionId);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            var updated1 = _entryDocumentService.GetByFilter(new EntryFilter { AccountId = message.AccountId });
            _transactionLuceneService.Update(updated1.ToArray());
            var updated2 = _entryDocumentService.GetByFilter(new EntryFilter { OffsetAccountId = message.AccountId });
            _transactionLuceneService.Update(updated2.ToArray());
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var transactions = _transactionDocumentService.GetByFilter(new TransactionFilter() { LedgerId = message.LedgerId });

            _transactionLuceneService.Delete(transactions.Select(x => x.Id).ToArray());
        }

        private void SaveEntries(List<ExpandedEntryData> rawEntries, string transactionId, string ledgerId)
        {
            if (rawEntries == null || rawEntries.Count == 0)
            {
                return;
            }

            var entries = EntryDocumentEventHandler.Map(rawEntries, transactionId, ledgerId);
            _transactionLuceneService.Insert(entries.ToArray());
        }

        private void RemoveEntries(string transactionId)
        {
            _transactionLuceneService.Delete(new[] { transactionId });
        }

        public void Handle(Transaction_DeleteMultipleMessage message)
        {
            //during application work lucene flush data to disc immedeately as it come, and it's is very expensive operation
            //but during read model regeneration we collecting a lot of data in ram and flush at once to optimize process
            //So we are cheking if message come in normal application work we will flush all data at once, if regeneration we no need do this
            //because we can break lucene service configuration and make regeneration slow

            if (_transactionLuceneService.IsImmediateFlush)
                _transactionLuceneService.WillDoManualFlush();

            foreach (var id in message.TransactionIds)
                RemoveEntries(id);

            if (_transactionLuceneService.IsManualFlush)
                _transactionLuceneService.Flush(true);
        }
    }
}
