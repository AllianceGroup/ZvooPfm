using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.TransactionsController;
using mPower.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;

namespace Default.Factories.Commands
{
    public class Transaction_CreateCommandFactory : 
        IObjectFactory<TransactionDto, Transaction_CreateCommand>,
        IObjectFactory<AddStandardTransactionViewModel, Transaction_CreateCommand>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IIdGenerator _idGenerator;

        public Transaction_CreateCommandFactory(LedgerDocumentService ledgerService, IIdGenerator idGenerator)
        {
            _ledgerService = ledgerService;
            _idGenerator = idGenerator;
        }
        
        public Transaction_CreateCommand Load(TransactionDto args)
        {
            var command = new Transaction_CreateCommand();

            var ledger =  _ledgerService.GetById(args.LedgerId);

            if (args.Entries != null)
                command.Entries = TransactionGenerator.ExpandEntryData(ledger, args.Entries.ToArray());

            command.Imported = args.Imported;
            command.UserId = ledger.Users.First().Id;
            command.LedgerId = args.LedgerId;
            command.ReferenceNumber = args.ReferenceNumber;
            command.TransactionId = args.TransactionId;
            command.Type = args.Type;
            command.ImportedTransactionId = args.ImportedTransactionId;

            try
            {
                var type = ledger.Accounts.Single(x => x.Id == args.BaseEntryAccountId).TypeEnum;

                if (!String.IsNullOrEmpty(args.BaseEntryAccountId))
                {
                    command.BaseEntryAccountId = args.BaseEntryAccountId;
                    command.BaseEntryAccountType = type;
                }

                return command;
            }
            catch
            {
                if (!String.IsNullOrEmpty(args.BaseEntryAccountId))
                {
                    command.BaseEntryAccountId = args.BaseEntryAccountId;
                    command.BaseEntryAccountType = args.BaseEntryAccountType;
                }

                return command;
            }
        }

        public Transaction_CreateCommand Load(AddStandardTransactionViewModel model)
        {
            if (model.AmountInDollars.ToDouble() == 0)
                throw new Exception("Cannot create a transaction with an amount of 0.00");

            DateTime bookedDate;
            DateTime.TryParse(model.BookedDate, out bookedDate);

            if (bookedDate == DateTime.MinValue)
            {
                bookedDate = DateTime.Now;
            }
            else
            {
                // we need to add exact time of transaction adding, otherwize lucene won't sort them correctly
                bookedDate = bookedDate.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);
            }

            var baseEntry = new EntryData
            {
                AccountId = model.AccountId,
                BookedDate = bookedDate,
                Memo = model.Memo,
                Payee = model.Payee,
                CreditAmountInCents = AccountingFormatter.CreditAmountByTransactionType(AccountingFormatter.DollarsToCents(model.AmountInDollars.ToDouble()), model.TransactionType),
                DebitAmountInCents = AccountingFormatter.DebitAmountByTransactionType(AccountingFormatter.DollarsToCents(model.AmountInDollars.ToDouble()), model.TransactionType)
            };

            var offsetEntry = new EntryData
            {
                AccountId = model.OffSetAccountId,
                BookedDate = bookedDate,
                Memo = model.Memo,
                Payee = model.Payee,
                CreditAmountInCents = baseEntry.DebitAmountInCents,
                DebitAmountInCents = baseEntry.CreditAmountInCents
            };

            var ledger = _ledgerService.GetById(model.LedgerId);
                

            var cmd = Load(new TransactionDto()
            {
                LedgerId = ledger.Id,
                TransactionId = _idGenerator.Generate(),
                Entries = new List<EntryData> { baseEntry, offsetEntry },
                Type = model.TransactionType,
                ReferenceNumber = model.ReferenceNumber,
                BaseEntryAccountId = model.AccountId,
                Imported = false,
            });

            return cmd;
        }
    }
}