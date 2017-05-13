using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.TransactionsController;
using mPower.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Framework.Mvc;

namespace Default.Factories.Commands
{
    public class Transaction_ModifyCommandFactory :
        IObjectFactory<TransactionModifyDto, Transaction_ModifyCommand>,
        IObjectFactory<TransactionEntryChangeAccountDto, Transaction_ModifyCommand>,
        IObjectFactory<EditMultipleEntriesViewModel, IEnumerable<Transaction_ModifyCommand>>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly TransactionDocumentService _transactionDocumentService;

        public Transaction_ModifyCommandFactory(LedgerDocumentService ledgerService, TransactionDocumentService transactionDocumentService)
        {
            _ledgerService = ledgerService;
            _transactionDocumentService = transactionDocumentService;
        }

        public Transaction_ModifyCommand Load(TransactionModifyDto dto)
        {
            var ledger = _ledgerService.GetById(dto.LedgerId);

            var command = new Transaction_ModifyCommand();
            if (!String.IsNullOrEmpty(dto.BaseEntryAccountId))
            {
                command.BaseEntryAccountId = dto.BaseEntryAccountId;
                command.BaseEntryAccountType = ledger.Accounts.Single(x => x.Id == dto.BaseEntryAccountId).TypeEnum;
            }
            command.Entries = TransactionGenerator.ExpandEntryData(ledger, dto.Entries.ToArray());
            command.Imported = dto.Imported;
            command.LedgerId = dto.LedgerId;
            command.ReferenceNumber = dto.ReferenceNumber;
            command.TransactionId = dto.TransactionId;
            command.Type = dto.Type;

            return command;
        }

        public Transaction_ModifyCommand Load(TransactionEntryChangeAccountDto dto)
        {
            var transaction = _transactionDocumentService.GetById(dto.TransactionId);
            var ledger = _ledgerService.GetById(dto.LedgerId);

            var entries = transaction.Entries.Select(x => new EntryData()
            {
                AccountId = x.AccountId,
                BookedDate = x.BookedDate,
                CreditAmountInCents = x.CreditAmountInCents,
                DebitAmountInCents = x.DebitAmountInCents,
                Memo = x.Memo,
                Payee = x.Payee,

            }).ToArray();

            foreach (var entry in entries.Where(entry => entry.AccountId == dto.PreviousAccountId))
            {
                entry.AccountId = dto.NewAccountId;
            }

            return new Transaction_ModifyCommand()
            {
                BaseEntryAccountId = transaction.BaseEntryAccountId,
                BaseEntryAccountType = transaction.BaseEntryAccountType,
                TransactionId = dto.TransactionId,
                LedgerId = dto.LedgerId,
                ReferenceNumber = transaction.ReferenceNumber,
                Entries = TransactionGenerator.ExpandEntryData(ledger, entries),
                Type = transaction.Type,
                Imported = transaction.Imported,
            };
        }

        public IEnumerable<Transaction_ModifyCommand> Load(EditMultipleEntriesViewModel input)
        {
            var transactions = input.TransactionsIds.Split(';');
            var offsetAccountId = input.AccountId;
            var offsetMemo = input.Memo ?? string.Empty;

            foreach (var transactionId in transactions)
            {
                var transactionToEdit = _transactionDocumentService.GetById(transactionId);

                if (transactionToEdit == null || transactionToEdit.LedgerId != input.LedgerId) continue;

                var baseEntryOld =
                    transactionToEdit.Entries.SingleOrDefault(x => x.AccountId == transactionToEdit.BaseEntryAccountId) ??
                    transactionToEdit.Entries.Single(
                        x =>
                        AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountLabel(
                            x.DebitAmountInCents, x.CreditAmountInCents, x.AccountLabel) < 0);

                var offsetEntryOld = transactionToEdit.Entries.Single(x => x.AccountId != baseEntryOld.AccountId);

                var bookedDate = transactionToEdit.BookedDate;

                var baseEntry = new EntryData
                                    {
                                        AccountId = baseEntryOld.AccountId,
                                        BookedDate = bookedDate,
                                        Memo = baseEntryOld.Memo,
                                        Payee = baseEntryOld.Payee,
                                        CreditAmountInCents = baseEntryOld.CreditAmountInCents,
                                        DebitAmountInCents = baseEntryOld.DebitAmountInCents,
                                    };

                var offsetEntry = new EntryData
                                      {
                                          AccountId = offsetAccountId,
                                          BookedDate = bookedDate,
                                          Memo = offsetMemo,
                                          Payee = offsetEntryOld.Payee,
                                          CreditAmountInCents = offsetEntryOld.CreditAmountInCents,
                                          DebitAmountInCents = offsetEntryOld.DebitAmountInCents
                                      };

                var cmd =
                    this.Load(new TransactionModifyDto
                            {
                                LedgerId = input.LedgerId,
                                TransactionId =
                                    transactionId,
                                Entries =
                                    new List
                                    <EntryData>
                                        {
                                            baseEntry,
                                            offsetEntry
                                        },
                                Type =
                                    transactionToEdit
                                    .Type,
                                ReferenceNumber =
                                    transactionToEdit
                                    .ReferenceNumber,
                                BaseEntryAccountId =
                                    baseEntry.
                                    AccountId,
                                Imported =
                                    transactionToEdit
                                    .Imported
                            });

                yield return cmd;
            }
        }
    }
}
