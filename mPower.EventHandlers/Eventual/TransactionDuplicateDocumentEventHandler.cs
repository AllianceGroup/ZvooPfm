using System.Linq;
using mPower.Documents;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class TransactionDuplicateDocumentEventHandler : IMessageHandler<Ledger_Account_AggregationStatus_UpdatedEvent>
    {
        private readonly DuplicateProcessor _duplicateProcessor;
        private readonly IEventService _eventService;
        private readonly TransactionDocumentService _transactionService;

        public TransactionDuplicateDocumentEventHandler(DuplicateProcessor duplicateProcessor, IEventService eventService, TransactionDocumentService transactionService)
        {
            _duplicateProcessor = duplicateProcessor;
            _eventService = eventService;
            _transactionService = transactionService;
        }

        public void Handle(Ledger_Account_AggregationStatus_UpdatedEvent message)
        {
            //TODO: Un comment when wanting to test duplicate logic
            //if (message.NewStatus != AggregatedAccountStatusEnum.PullingTransactions)
            //{
            //    var intuitTransactions = _transactionService.GetPotentialDuplicates(message.LedgerId);
            //    var duplicateEvents = _duplicateProcessor.GetInterAccountDuplicateCommands(intuitTransactions.Select(Map).ToList());

            //    if (duplicateEvents.Any())
            //        _eventService.Send(duplicateEvents.Cast<IEvent>().ToArray());
            //}
        }

        private static DuplicateProcessor.ImportedTransactionDto Map(TransactionDocument doc)
        {
            var baseEntry = doc.Entries.Single(x => x.AccountId == doc.BaseEntryAccountId);
            return new DuplicateProcessor.ImportedTransactionDto
            {
                LedgerId = doc.LedgerId,
                AccountId = doc.BaseEntryAccountId,
                LedgerTransactionId = doc.Id,
                AmountInCents = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountLabel(baseEntry.DebitAmountInCents, baseEntry.CreditAmountInCents, baseEntry.AccountLabel),
                Date = doc.BookedDate,
            };
        }
    }
}