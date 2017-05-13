using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.IifHelpers.Documents;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using Paralect.Domain;

namespace mPower.Documents.IifHelpers
{
    public class IifCommandsGenerator
    {
        private readonly IIdGenerator _idGenerator;
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly IObjectRepository _objectRepository;

        public IifCommandsGenerator(IIdGenerator idGenerator, LedgerDocumentService ledgerDocumentService, IObjectRepository objectRepository)
        {
            _idGenerator = idGenerator;
            _ledgerDocumentService = ledgerDocumentService;
            _objectRepository = objectRepository;
        }

        public List<Command> Generate(string ledgerId, IifParsingResult parsingResult)
        {
            var result = new List<Command>();
            var ledger = _ledgerDocumentService.GetById(ledgerId);
            if (ledger != null)
            {
                var accountIds = new Dictionary<string, string>();
                foreach (var accountDocument in ledger.Accounts)
                {
                    if (!accountIds.ContainsKey(accountDocument.Name))
                    {
                        accountIds.Add(accountDocument.Name, accountDocument.Id);
                    }
                }

                result.AddRange(GenerateAccountCommands(ledgerId, parsingResult.Accounts, accountIds));
                result.AddRange(GenerateTransactionCommands(ledger, parsingResult.Transactions, accountIds));
            }

            return result;
        }

        private IEnumerable<Command> GenerateAccountCommands(string ledgerId, IEnumerable<IifAccount> iifAccounts, IDictionary<string, string> existAccountIds)
        {
            var result = new List<Command>();
            foreach (var iifAccount in iifAccounts.Where(a => string.IsNullOrEmpty(a.ExistingAccountId)))
            {
                var accountId = _idGenerator.Generate();
                result.Add(new Ledger_Account_CreateCommand
                {
                    LedgerId = ledgerId,
                    AccountId = accountId,
                    Name = iifAccount.Name,
                    AccountTypeEnum = iifAccount.TypeEnum,
                    AccountLabelEnum = iifAccount.LabelEnum,
                    Imported = true,
                });
                result.Add(new Ledger_Account_UpdateCommand
                {
                    LedgerId = ledgerId,
                    AccountId = accountId,
                    Name = iifAccount.Name,
                    Description = iifAccount.Description,
                });

                if (!existAccountIds.ContainsKey(iifAccount.Name))
                {
                    existAccountIds.Add(iifAccount.Name, accountId);
                }
            }
            return result;
        }

        private IEnumerable<Command> GenerateTransactionCommands(LedgerDocument ledger, IEnumerable<IifTransaction> iifTransactions, IDictionary<string, string> existAccountIds)
        {
            return iifTransactions
                .Where(t => t.Include && t.Entries.All(e => existAccountIds.ContainsKey(e.AccountName)))
                .Select(iifTransaction =>

                    _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto()
                            {
                                TransactionId = _idGenerator.Generate(),
                                LedgerId = ledger.Id,
                                Type = iifTransaction.Type,
                                Entries = iifTransaction.Entries.Select(
                                    iifEntry => new EntryData
                                                    {
                                                        AccountId = existAccountIds[iifEntry.AccountName],
                                                        BookedDate = iifTransaction.BookedDate,
                                                        DebitAmountInCents = iifEntry.Debit,
                                                        CreditAmountInCents = iifEntry.Credit,
                                                        Payee = iifEntry.Payee,
                                                        Memo = iifEntry.Memo,
                                                    }).ToList(),
                                Imported = true,
                            }));
        }
    }
}
