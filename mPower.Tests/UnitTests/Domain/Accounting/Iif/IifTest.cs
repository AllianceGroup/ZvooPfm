using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Default.Factories.Commands;
using MongoDB.Bson;
using Moq;
using mPower.Documents;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.IifHelpers;
using mPower.Documents.IifHelpers.Documents;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using NUnit.Framework;
using Paralect.Config.Settings;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Iif
{
    [TestFixture]
    public class IifTest
    {
        protected readonly DateTime BookedDate = new DateTime(2011, 8, 13);
        protected readonly LedgerDocument Ledger = new LedgerDocument();
        protected readonly List<TransactionDocument> Transactions = new List<TransactionDocument>();
        protected readonly string TestFilePath = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("bin", StringComparison.InvariantCulture))
            + @"UnitTests\Domain\Accounting\Iif\IifFiles\iif_formatter_ledger_test.iif";
        protected readonly IifParsingResult ParsingResult = new IifParsingResult();
        protected readonly List<Command> ExpectedCommands = new List<Command>();
        protected IifCommandsGenerator CommandsGenerator;

        [SetUp]
        public virtual void SetUp()
        {
            Ledger.Users.Add(new LedgerUserDocument {Id = Guid.NewGuid().ToString()});

            #region Add Accounts

            var account1 = new AccountDocument
            {
                Id = "34",
                Name = "My Expense Account",
                TypeEnum = AccountTypeEnum.Expense,
                LabelEnum = AccountLabelEnum.Expense,
                Description = "Old Good Account",
            };
            Ledger.Accounts.Add(account1);

            var account2 = new AccountDocument
            {
                Id = "345",
                Name = "My Property",
                TypeEnum = AccountTypeEnum.Asset,
                LabelEnum = AccountLabelEnum.OtherAsset,
                Description = "All my things",
            };
            Ledger.Accounts.Add(account2);

            #endregion

            #region Add Transactions

            var trabsaction1 = new TransactionDocument
            {
                BookedDate = BookedDate,
                Type = TransactionType.Bill,
            };

            var entry1 = new TransactionEntryDocument
            {
                AccountId = "34",
                DebitAmountInCents = 3500,
                CreditAmountInCents = 0,
                Memo = "bike1",
                Payee = "Honda1",
                BookedDate = BookedDate,
            };
            trabsaction1.Entries.Add(entry1);

            var entry2 = new TransactionEntryDocument
            {
                AccountId = "345",
                DebitAmountInCents = 0,
                CreditAmountInCents = 3500,
                Memo = "bike2",
                Payee = "Honda2",
                BookedDate = BookedDate,
            };
            trabsaction1.Entries.Add(entry2);

            Transactions.Add(trabsaction1);

            #endregion

            #region Parsing Result

            ParsingResult.Accounts.AddRange(Ledger.Accounts.Select(
                a => new IifAccount
                {
                    Name = a.Name,
                    Description = a.Description,
                    LabelEnum = a.LabelEnum,
                    TypeEnum = a.TypeEnum,
                }));

            var accounts = Ledger.Accounts.ToDictionary(a => a.Id, a => a);
            var iifTransactions = new List<IifTransaction>();

            foreach (var trans in Transactions)
            {
                var iifTrans = new IifTransaction { BookedDate = trans.BookedDate, Type = trans.Type };
                foreach (var entry in trans.Entries)
                {
                    var iifEntry = new IifEntry
                    {
                        AccountName = accounts[entry.AccountId].Name,
                        Amount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(entry.DebitAmountInCents, entry.CreditAmountInCents, accounts[entry.AccountId].TypeEnum),
                        Debit = entry.DebitAmountInCents,
                        Credit = entry.CreditAmountInCents,
                        Payee = entry.Payee,
                        Memo = entry.Memo,
                    };
                    iifTrans.Entries.Add(iifEntry);
                }
                iifTransactions.Add(iifTrans);
            }

            ParsingResult.Transactions.AddRange(iifTransactions);

            #endregion

            #region Generate Commands

            foreach (var accountDocument in Ledger.Accounts)
            {
                ExpectedCommands.Add(new Ledger_Account_CreateCommand
                {
                    LedgerId = Ledger.Id,
                    AccountId = accountDocument.Id,
                    Name = accountDocument.Name,
                    AccountTypeEnum = accountDocument.TypeEnum,
                    AccountLabelEnum = accountDocument.LabelEnum,
                    Imported = true,
                });
                ExpectedCommands.Add(new Ledger_Account_UpdateCommand
                {
                    LedgerId = Ledger.Id,
                    AccountId = accountDocument.Id,
                    Name = accountDocument.Name,
                    Description = accountDocument.Description,
                });
            }

            foreach (var transactionDocument in Transactions)
            {
                ExpectedCommands.Add(new Transaction_CreateCommand
                                         {
                                             TransactionId = transactionDocument.Id,
                                             LedgerId = transactionDocument.LedgerId,
                                             Type = transactionDocument.Type,
                                             Entries = null,
                                             Imported = true,
                                         });
            }

            var settings = SettingsMapper.Map<MPowerSettings>();
            var sessionId = String.Format("mpower_{0}", ObjectId.GenerateNewId());
            var mongoRead = new MongoRead(settings.MongoTestReadDatabaseConnectionString + sessionId);

            var ledgerServiceMock = new Mock<LedgerDocumentService>(mongoRead);
            ledgerServiceMock.Setup(ls => ls.GetById(Ledger.Id)).Returns(Ledger);

            var objectGenerator = new Mock<IObjectRepository>();
            var factory = new Transaction_CreateCommandFactory(ledgerServiceMock.Object, null);

            objectGenerator.Setup(x => x.Load<TransactionDto, Transaction_CreateCommand>(It.IsAny<TransactionDto>())).
                Returns((TransactionDto t) => factory.Load(t));

            CommandsGenerator = new IifCommandsGenerator(new MongoObjectIdGenerator(), ledgerServiceMock.Object, objectGenerator.Object);

            #endregion
        }
    }
}
