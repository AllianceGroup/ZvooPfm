using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Default.ViewModel;
using Default.ViewModel.TransactionsController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Framework.Mvc;

namespace Default.Factories.ViewModels
{
    public class EditStandardTransactionViewModelFactory : IObjectFactory<string, EditStandardTransactionViewModel>
    {
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly TransactionDocumentService _transactionDocumentService;
        private readonly IObjectRepository _objectRepository;

        public EditStandardTransactionViewModelFactory(LedgerDocumentService ledgerDocumentService,
            TransactionDocumentService transactionDocumentService,
            IObjectRepository objectRepository)
        {
            _ledgerDocumentService = ledgerDocumentService;
            _transactionDocumentService = transactionDocumentService;
            _objectRepository = objectRepository;
        }

        public EditStandardTransactionViewModel Load(string transactionId)
        {
            var transactionToEdit = _transactionDocumentService.GetById(transactionId);
            
            if (transactionToEdit == null)
                throw new Exception("Invalid Transaction Id");

            var ledger = _ledgerDocumentService.GetById(transactionToEdit.LedgerId);

            var accountsSelectList =
                _objectRepository.Load<IEnumerable<AccountDocument>, IEnumerable<GroupedSelectListItem>>(ledger.Accounts);

            var baseEntry = transactionToEdit.Entries.SingleOrDefault(x => x.AccountId == transactionToEdit.BaseEntryAccountId) ??
                            transactionToEdit.Entries.Single(x => AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountLabel(x.DebitAmountInCents, x.CreditAmountInCents, x.AccountLabel) < 0);

            var offsetEntry = transactionToEdit.Entries.Single(x => x.AccountId != baseEntry.AccountId);

            var filter = baseEntry.AccountLabel;

            var filteredAccounts = new List<SelectListItem> { new SelectListItem { Text = " ", Value = " " } };
            filteredAccounts.AddRange(ledger.Accounts.Where(x => x.LabelEnum == filter).Select(
                x => new SelectListItem { Text = x.Name, Value = x.Id }));

            var model = new EditStandardTransactionViewModel
            {
                Accounts = accountsSelectList,
                FilteredAccounts = filteredAccounts,
                AccountId = baseEntry.AccountId,
                BookedDate = transactionToEdit.BookedDate.ToShortDateString(),
                AmountInDollars = AccountingFormatter.CentsToDollarString(baseEntry.CreditAmountInCents == 0 ? baseEntry.DebitAmountInCents : baseEntry.CreditAmountInCents),
                Memo = baseEntry.Memo,
                Payee = baseEntry.Payee,
                OffSetAccountId = offsetEntry.AccountId,
                TransactionType = transactionToEdit.Type,
                TransactionId = transactionToEdit.Id,
                ReferenceNumber = transactionToEdit.ReferenceNumber,
                OffSetAccountName = ledger.Accounts.Single(x => x.Id == offsetEntry.AccountId).Name,
                AccountName = ledger.Accounts.Single(x => x.Id == baseEntry.AccountId).Name,
                Imported = transactionToEdit.Imported,
                EditType = transactionToEdit.Type.ToString("g")
            };

            
            return model;
        }
    }

    public class EditStandardTransactionViewModelData
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        

    }
}
