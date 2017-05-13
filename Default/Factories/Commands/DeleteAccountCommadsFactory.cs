using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework.Mvc;
using mPower.Documents.Documents.Accounting.Ledger;

namespace Default.Factories.Commands
{
    public class DeleteAccountCommadsFactory : IObjectFactory<DeleteAccountCommadsFilter, IEnumerable<Command>>
    {
        private readonly EntryDocumentService _entryDocumentService;
        private readonly LedgerDocumentService _ledgerService;

        public DeleteAccountCommadsFactory(LedgerDocumentService ledgerService, EntryDocumentService entryDocumentService)
        {
            _ledgerService = ledgerService;
            _entryDocumentService = entryDocumentService;
        }

        public IEnumerable<Command> Load(DeleteAccountCommadsFilter input)
        {
            var ledger = _ledgerService.GetById(input.LedgerId);

            var accountIds = new List<string>() { input.AccountId };
            accountIds.AddRange(ledger.Accounts.Where(x => x.ParentAccountId == input.AccountId).Select(x => x.Id));

            return GetDeleteAccountCommands(ledger, accountIds.ToArray());
        }

        private List<Command> GetDeleteAccountCommands(LedgerDocument ledger, params string[] accountIds)
        {
            var result = new List<Command>();
            var transactionIds = new List<string>();


            foreach (var id in accountIds)
            {
                transactionIds.AddRange(
                    _entryDocumentService
                    .GetByFilter(new EntryFilter { AccountId = id, LedgerId = ledger.Id })
                    .Select(e => e.TransactionId).Distinct().ToList());

                var account = ledger.Accounts.Single(x => x.Id == id);

                result.Add(new Ledger_Account_RemoveCommand
                {
                    AccountId = id,
                    LedgerId = ledger.Id,
                    Label = account.LabelEnum
                });
            }

            if (transactionIds.Count > 0)
            {
                result.Add(new Transaction_DeleteMultipleCommand()
                {
                    LedgerId = ledger.Id,
                    TransactionIds = transactionIds.Distinct().ToList()
                });
            }

            return result;
        }
    }

    public class DeleteAccountCommadsFilter
    {
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
    }
}