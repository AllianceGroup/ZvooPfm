using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;

namespace mPower.Documents
{
    public class TransactionDto
    {
        public string BaseEntryAccountId { get; set; }
        public AccountTypeEnum BaseEntryAccountType { get; set; }
        public string ReferenceNumber { get; set; }
        public String LedgerId { get; set; }
        public String TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public List<EntryData> Entries { get; set; }
        public bool Imported { get; set; }
        public string ImportedTransactionId { get; set; }

        public TransactionDto()
        {
            Entries = new List<EntryData>();
            BaseEntryAccountType =
            AccountTypeEnum.Asset;
        }
    }

    public class TransactionEntryChangeAccountDto
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public string PreviousAccountId { get; set; }
        public string NewAccountId { get; set; }
    }

    public class TransactionCreateDuplicateDto
    {
        public string DuplicateId { get; set; }
        public TransactionData BaseTransaction { get; set; }
        public List<TransactionData> PotentialDuplicates { get; set; }
    }

    public class TransactionGenerator
    {
        private readonly LedgerDocumentService _ledgerService;

        public TransactionGenerator(LedgerDocumentService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        public Transaction_DuplicateCreatedEvent GenerateDuplicateCreatedEvent(TransactionCreateDuplicateDto dto, LedgerDocument ledger = null)
        {
            if (ledger == null)
                ledger = _ledgerService.GetById(dto.BaseTransaction.LedgerId);

            var command = new Transaction_DuplicateCreatedEvent
            {
                BaseTransaction = TransactionToExpandedEntryData(dto.BaseTransaction, ledger),
                PotentialDuplicates = new List<ExpandedEntryData>(),
            };

            foreach (var item in dto.PotentialDuplicates)
            {
                command.PotentialDuplicates.Add(TransactionToExpandedEntryData(item, ledger));
            }

            return command;
        }

        public static bool IsValidEntry(EntryDocument entry)
        {
            if (entry == null)
                return false;

            var obe = BaseAccounts.OpeningBalanceEquity.ToLower();

            //we need only these types of events
            return (entry.AccountLabel == AccountLabelEnum.Bank ||
                   entry.AccountLabel == AccountLabelEnum.CreditCard ||
                   entry.AccountLabel == AccountLabelEnum.Loan) &&
                   (entry.OffsetAccountId != null && entry.OffsetAccountId.ToLower() != obe) &&
                   entry.AccountId.ToLower() != obe &&
                   (entry.Memo != "Beginning Balance Adjustment");
        }

        private ExpandedEntryData TransactionToExpandedEntryData(TransactionData data, LedgerDocument ledger)
        {
            var entry = data.Entries.Single(x => x.AccountId == data.BaseEntryAccountId);

            var account = ledger.Accounts.Single(x => x.Id == entry.AccountId);
            var expEntry = new ExpandedEntryData
            {
                AccountId = entry.AccountId,
                AccountLabel = account.LabelEnum,
                AccountType = account.TypeEnum,
                AccountName = account.Name,
                BookedDate = data.BookedDate,
                CreditAmountInCents = entry.CreditAmountInCents,
                DebitAmountInCents = entry.DebitAmountInCents,
                Memo = entry.Memo,
                Payee = entry.Payee,
                LedgerId = data.LedgerId,
                TransactionId = data.Id,
                TransactionImported = data.Imported,
            };


            if (data.Entries.Count > 2)
            {
                expEntry.OffsetAccountId = "Split";
                expEntry.OffsetAccountName = "Split";
            }
            else
            {
                expEntry.OffsetAccountId = data.Entries.Single(x => x != entry).AccountId;
                expEntry.OffsetAccountName = ledger.Accounts.Single(x => x.Id == expEntry.OffsetAccountId).Name;
            }

            return expEntry;
        }

        public static List<ExpandedEntryData> ExpandEntryData(LedgerDocument ledger, params EntryData[] entries)
        {
            var expandedEntries = new List<ExpandedEntryData>();

            foreach (var entry in entries)
            {
                var account = ledger.Accounts.SingleOrDefault(x => x.Id == entry.AccountId);

                if (account == null) // Andrew: why continue? seems should be error
                    continue;

                var expEntry = new ExpandedEntryData
                {
                    AccountId = entry.AccountId,
                    AccountLabel = account.LabelEnum,
                    AccountType = account.TypeEnum,
                    AccountName = account.Name,
                    BookedDate = entry.BookedDate,
                    CreditAmountInCents = entry.CreditAmountInCents,
                    DebitAmountInCents = entry.DebitAmountInCents,
                    Memo = entry.Memo,
                    Payee = entry.Payee,
                    LedgerId = ledger.Id,

                };

                if (entries.Length > 2)
                {
                    expEntry.OffsetAccountId = "Split";
                    expEntry.OffsetAccountName = "Split";
                }
                else
                {

                    var offsetEntry = entries.SingleOrDefault(x => x != entry);

                    if (offsetEntry != null)
                    {
                        expEntry.OffsetAccountId = entries.Single(x => x != entry).AccountId;
                        expEntry.OffsetAccountName =
                            ledger.Accounts.Single(x => x.Id == expEntry.OffsetAccountId).Name;
                    }

                }

                expandedEntries.Add(expEntry);
            }

            return expandedEntries;
        }
    }
}
