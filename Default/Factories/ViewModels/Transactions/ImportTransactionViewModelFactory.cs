using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Default.ViewModel;
using Default.ViewModel.TransactionsController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;

namespace Default.Factories.ViewModels
{
    public class ImportTransactionViewModelFactory : IObjectFactory<ImportTransactionViewModel, ImportTransactionViewModel>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IObjectRepository _objectRepository;

        public ImportTransactionViewModelFactory(LedgerDocumentService ledgerService, IObjectRepository objectRepository)
        {
            _ledgerService = ledgerService;
            _objectRepository = objectRepository;
        }

        public ImportTransactionViewModel Load(ImportTransactionViewModel input)
        {
            var accounts = _ledgerService.GetById(input.LedgerId).Accounts;

            var accountsSelectList = _objectRepository.Load<string, IEnumerable<GroupedSelectListItem>>(input.LedgerId);


            var filteredAccounts = new List<SelectListItem> { new SelectListItem { Text = " ", Value = " " } };

            filteredAccounts.AddRange(
                        accounts.Where(x => x.Id != BaseAccounts.UnknownCash &&
                            x.LabelEnum == AccountLabelEnum.Bank ||
                            x.LabelEnum == AccountLabelEnum.Loan ||
                            x.LabelEnum == AccountLabelEnum.Investment ||
                            x.LabelEnum == AccountLabelEnum.CreditCard)
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Id }));

            var model = new ImportTransactionViewModel
            {
                AccountId = input.AccountId,
                Accounts = accountsSelectList,
                FilteredAccounts = filteredAccounts,

            };

            return model;
        }

     
    }
}
