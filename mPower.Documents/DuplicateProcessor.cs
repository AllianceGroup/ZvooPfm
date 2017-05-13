using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework.Environment;

namespace mPower.Documents
{
    public class DuplicateProcessor
    {
        public class ImportedTransactionDto
        {
            public string LedgerId { get; set; }
            public string LedgerTransactionId { get; set; }
            public string AccountId { get; set; }
            public long AmountInCents { get; set; }
            public DateTime Date { get; set; }
        }

        private readonly TransactionGenerator _transactionGenerator;
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly TransactionDocumentService _transactions;
        private readonly IIdGenerator _idGenerator;
        

        public DuplicateProcessor(TransactionDocumentService transactions, 
                                  IIdGenerator idGenerator,
                                  TransactionGenerator transactionGenerator,
                                  LedgerDocumentService ledgerDocumentService)
        {
            _transactionGenerator = transactionGenerator;
            _ledgerDocumentService = ledgerDocumentService;
            _transactions = transactions;
            _idGenerator = idGenerator;
        }


        public List<Transaction_DuplicateCreatedEvent> GetInterAccountDuplicateCommands(List<ImportedTransactionDto> intuitTransactions)
        {
            var duplicateCommands = new List<Transaction_DuplicateCreatedEvent>();

            if (intuitTransactions != null && intuitTransactions.Any())
            {
                var ledgerId = intuitTransactions.First().LedgerId;
                var ledger = _ledgerDocumentService.GetById(ledgerId);
                var ledgerTransactions = _transactions.GetByFilter(new TransactionFilter {LedgerId = ledgerId});
                var duplicateCreatedEvents = ProcessInterAccountTransactions(intuitTransactions, ledgerTransactions, ledger);
                duplicateCommands.AddRange(duplicateCreatedEvents);
            }

            return duplicateCommands;
        }


        public IEnumerable<Transaction_Entry_DuplicateCreateCommand> GetSingleAccountDuplicateCommands(List<EntryDocument> entries)
        {
            foreach (var manualEntry in entries.Where(x => x.Imported == false))
            {
                var minDate = manualEntry.BookedDate.AddDays(-7);
                var maxDate = manualEntry.BookedDate.Date.AddDays(7);
                var id = manualEntry.Id;
                var balance = manualEntry.EntryBalance;


                var potentialDuplicates = entries.Where(importedEntry => importedEntry.Imported &&
                                                        importedEntry.Id != id &&
                                                        importedEntry.BookedDate >= minDate && importedEntry.BookedDate <= maxDate &&
                                                        importedEntry.EntryBalance == balance).ToList();

                if (!potentialDuplicates.Any()) continue;

                var cmd = new Transaction_Entry_DuplicateCreateCommand()
                {
                    DuplicateId = _idGenerator.Generate(), 
                    LedgerId = manualEntry.LedgerId,
                    ManualEntry = Map(manualEntry),
                    PotentialDuplicates = potentialDuplicates.Select(Map).ToList()
                };

                yield return cmd;
            }
        }

        #region Private Methods


        private ExpandedEntryData Map(EntryDocument data)
        {
            return new ExpandedEntryData()
                       {
                           AccountId = data.AccountId,
                           BookedDate = data.BookedDate,
                           CreditAmountInCents = data.CreditAmountInCents,
                           DebitAmountInCents = data.DebitAmountInCents,
                           Memo = data.Memo,
                           Payee = data.Payee,
                           AccountLabel = data.AccountLabel,
                           AccountName = data.AccountName,
                           AccountType = data.AccountType,
                           OffsetAccountId = data.OffsetAccountId,
                           OffsetAccountName = data.OffsetAccountName,
                           LedgerId = data.LedgerId,
                           TransactionId = data.LedgerId,
                           TransactionImported = data.Imported
                       };
        }

        private IEnumerable<Transaction_DuplicateCreatedEvent> ProcessInterAccountTransactions(IEnumerable<ImportedTransactionDto> intuitTransactions,
                                          List<TransactionDocument> ledgerTransactions, LedgerDocument ledger)
        {

            foreach (var intuitTrans in intuitTransactions)
            {
                var potentialDuplicates =
                    FindTransactionDuplicates(intuitTrans, ledgerTransactions);

                if (!potentialDuplicates.Any()) continue;


                var baseTransactionDocument = ledgerTransactions.SingleOrDefault(
                    x => x.Id == intuitTrans.LedgerTransactionId);

                // We only want to scan from bank accounts outward.  This will avoid finding the same dupliate twice
                if (baseTransactionDocument == null ||
                    baseTransactionDocument.BaseEntryAccountType != AccountTypeEnum.Asset || 
                    baseTransactionDocument.ConfirmedNotDuplicate)
                    continue;

                if (baseTransactionDocument.ConfirmedNotDuplicate)
                    continue;

                var baseTransactionData = Map(baseTransactionDocument);

                var baseEntry = baseTransactionData.Entries.Where(x => x.AccountId == baseTransactionData.BaseEntryAccountId);
                var amount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                    baseEntry.Sum(x => x.DebitAmountInCents),
                    baseEntry.Sum(x => x.CreditAmountInCents),
                    baseTransactionData.BaseEntryAccountType);
                // We only want to scan add reductions from the bank account to avoid duplicates between account transfers
                if (amount > 0)
                    continue;

                var cmd = _transactionGenerator.GenerateDuplicateCreatedEvent(new TransactionCreateDuplicateDto()
                {
                    DuplicateId = _idGenerator.Generate(),
                    BaseTransaction = baseTransactionData,
                    PotentialDuplicates = potentialDuplicates.Where(x => x.BaseEntryAccountId != baseTransactionData.BaseEntryAccountId).Select(Map).ToList()
                }, ledger);

                if (cmd.PotentialDuplicates.Count > 0) yield return cmd;
            }
        }

        private List<TransactionDocument> FindTransactionDuplicates(ImportedTransactionDto intuitTransaction, IEnumerable<TransactionDocument> ledgerTransactions)
        {
            try
            {
                var minDate = intuitTransaction.Date.AddDays(-7);
                var maxDate = intuitTransaction.Date.AddDays(7);


                var potentialDuplicates = ledgerTransactions.Where(transactionDocument =>
                                                                   transactionDocument.Id !=
                                                                   intuitTransaction.LedgerTransactionId &&
                                                                   transactionDocument.BookedDate >= minDate &&
                                                                   transactionDocument.BookedDate <= maxDate &&
                                                                   AmountsAreEqual(transactionDocument, intuitTransaction)).ToList();

                return potentialDuplicates;
            }
            catch(Exception e)
            {
               return new List<TransactionDocument>();
            }

        }


        private static TransactionData Map(TransactionDocument data)
        {
            return new TransactionData
                       {
                           BaseEntryAccountId = data.BaseEntryAccountId,
                           BookedDate = data.BookedDate,
                           Id = data.Id,
                           Imported = data.Imported,
                           Memo = data.Memo,
                           LedgerId = data.LedgerId,
                           ReferenceNumber = data.ReferenceNumber,
                           Type = data.Type,
                           Entries = data.Entries.Select(Map).ToList(),
                           BaseEntryAccountType = data.BaseEntryAccountType
                       };
        }

        private static EntryData Map(TransactionEntryDocument data)
        {
            return new EntryData
                       {
                           AccountId = data.AccountId,
                           Memo = data.Memo,
                           BookedDate = data.BookedDate,
                           Payee = data.Payee,
                           CreditAmountInCents = data.CreditAmountInCents,
                           DebitAmountInCents = data.DebitAmountInCents
                       };
        }

        private AmountTypeEnum? GetTransactionType(string ledgerId, string accountId, long amountInCents)
        {
            AmountTypeEnum? result = null;
            var ledger = _ledgerDocumentService.GetById(ledgerId);
            if (ledger != null)
            {
                var account = ledger.Accounts.FirstOrDefault(x => x.Id == accountId);
                if (account != null)
                {
                    result = AccountingFormatter.DebitOrCredit(amountInCents, account.TypeEnum);
                }
            }
            return result;
        }

        private bool AmountsAreEqual(TransactionDocument transactionDocument, ImportedTransactionDto intuitTransaction)
        {
            var intuitTransactionType = GetTransactionType(transactionDocument.LedgerId, intuitTransaction.AccountId, intuitTransaction.AmountInCents);
            if (!intuitTransactionType.HasValue)
            {
                return false;
            }

            // Here we are checking to see if amounts are equal by comparing the contenter service item amount
            // To the transaction amount.  We filter out all entries with accounts that are not Bank, Credit Card or Loan
            // This way we don't get debit / credit amounts for accounts like Income or Expense Accounts.

            switch (intuitTransactionType)
            {
                case AmountTypeEnum.Credit:

                    return transactionDocument.Entries.Where(x =>
                        x.AccountLabel == AccountLabelEnum.Bank ||
                        x.AccountLabel == AccountLabelEnum.CreditCard ||
                        x.AccountLabel == AccountLabelEnum.Loan
                        ).Sum(x => x.CreditAmountInCents) == intuitTransaction.AmountInCents;

                case AmountTypeEnum.Debit:

                    return transactionDocument.Entries.Where(x =>
                        x.AccountLabel == AccountLabelEnum.Bank ||
                        x.AccountLabel == AccountLabelEnum.CreditCard ||
                        x.AccountLabel == AccountLabelEnum.Loan
                        ).Sum(x => x.DebitAmountInCents) == intuitTransaction.AmountInCents;

                default:
                    throw new NotImplementedException();
            }
        }


        #endregion


    }
}
