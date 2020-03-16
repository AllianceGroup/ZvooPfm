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
    public class AddStandardTransactionViewModelFactory : IObjectFactory<AddStandardTransactionViewModel, AddStandardTransactionViewModel>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IObjectRepository _objectRepository;

        public AddStandardTransactionViewModelFactory(LedgerDocumentService ledgerService, IObjectRepository objectRepository)
        {
            _ledgerService = ledgerService;
            _objectRepository = objectRepository;
        }

        public AddStandardTransactionViewModel Load(AddStandardTransactionViewModel input)
        {
            var accounts = _ledgerService.GetById(input.LedgerId).Accounts;

            var accountsSelectList = _objectRepository.Load<string, IEnumerable<GroupedSelectListItem>>(input.LedgerId);


            var filteredAccounts = new List<SelectListItem> { new SelectListItem { Text = " ", Value = " " } };

            switch (input.TransactionType)
            {
                case TransactionType.Check:
                    filteredAccounts.AddRange(
                        accounts.Where(x => x.Id != BaseAccounts.UnknownCash && 
                            x.LabelEnum == AccountLabelEnum.Bank ||
                            x.LabelEnum == AccountLabelEnum.Equity ||
                            x.LabelEnum == AccountLabelEnum.Loan || 
                            x.LabelEnum == AccountLabelEnum.Investment)
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Id }));

                    break;
                case TransactionType.CreditCard:
                    filteredAccounts.AddRange(
                        accounts.Where(x =>
                            x.LabelEnum == AccountLabelEnum.CreditCard)
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Id }));
                    break;

                case TransactionType.BankTransfer:
                    filteredAccounts.AddRange(
                        accounts.Where(x =>
                            x.LabelEnum == AccountLabelEnum.Bank &&
                            x.Id != BaseAccounts.UnknownCash)
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Id }));
                    break;

                case TransactionType.Deposit:
                    filteredAccounts.AddRange(
                        accounts.Where(x =>
                            x.LabelEnum == AccountLabelEnum.Bank &&
                            x.Id != BaseAccounts.UnknownCash)
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Id }));
                    break;

                default:
                    throw new Exception("Invalid Type");
            }
            

            var model = new AddStandardTransactionViewModel
            {
                AccountId = input.AccountId,
                Accounts = accountsSelectList,
                FilteredAccounts = filteredAccounts,
                TransactionType = input.TransactionType,
                BookedDate = DateTime.Now.ToShortDateString(),
                AmountInDollars = input.AmountInDollars,
                Memo = input.Memo,
                OffSetAccountId = input.OffSetAccountId,
                Payee = input.Payee,
                ReferenceNumber = input.ReferenceNumber,
            };

            return model;
        }

    }
}
