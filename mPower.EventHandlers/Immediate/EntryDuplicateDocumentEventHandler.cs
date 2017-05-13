using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework.Environment;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class EntryDuplicateDocumentEventHandler :
        IMessageHandler<Transaction_Entry_DuplicateCreatedEvent>
    {
        private readonly EntryDuplicateDocumentService _entries;
        private readonly IIdGenerator _idGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EntryDuplicateDocumentEventHandler(EntryDuplicateDocumentService entries, IIdGenerator idGenerator)
        {
            _entries = entries;
            _idGenerator = idGenerator;
        }

        public void Handle(Transaction_Entry_DuplicateCreatedEvent message)
        {
            var duplicate = new EntryDuplicateDocument()
                                {
                                    Id = message.DuplicateId,
                                    ManualEntry = Map(message.ManualEntry),
                                    PotentialDuplicates = message.PotentialDuplicates.Select(Map)
                                };

            _entries.Save(duplicate);
        }

        private EntryDocument Map(ExpandedEntryData entry)
        {
            return new EntryDocument()
                       {
                           Id = _idGenerator.Generate(),
                           AccountId = entry.AccountId,
                           BookedDate = entry.BookedDate,
                           CreditAmountInCents = entry.CreditAmountInCents,
                           DebitAmountInCents = entry.DebitAmountInCents,
                           BookedDateString = entry.BookedDate.ToString("MM-dd-yyyy"),
                           Payee = entry.Payee,
                           Memo = entry.Memo,
                           TransactionId = entry.TransactionId,
                           LedgerId = entry.LedgerId,
                           FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(entry.DebitAmountInCents - entry.CreditAmountInCents, true),
                           AccountLabel = entry.AccountLabel,
                           AccountType = entry.AccountType,
                           AccountName = entry.AccountName,
                           OffsetAccountId = entry.OffsetAccountId,
                           OffsetAccountName = entry.OffsetAccountName,
                           Imported = entry.TransactionImported
                       };
        }
    }
}
