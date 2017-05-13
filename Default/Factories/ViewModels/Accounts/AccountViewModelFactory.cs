using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Business.AccountsController;
using Default.ViewModel.Areas.Business.BusinessController;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;

namespace Default.Factories.ViewModels
{
    public class AccountViewModelFactory :
        IObjectFactory<string, ChartOfAccountsViewModel>,
        IObjectFactory<EditAccountFilter, EditManualAccountViewModel>,
        IObjectFactory<AccountsSortingFilter, ChartOfAccountsViewModel>,
        IObjectFactory<ParentAccountsFilter, IDictionary<string, string>>
    {
        private readonly IObjectRepository _objectRepository;
        private readonly LedgerDocumentService _ledgerService;

        public AccountViewModelFactory(IObjectRepository objectRepository, LedgerDocumentService ledgerDocumentService)
        {
            _objectRepository = objectRepository;
            _ledgerService = ledgerDocumentService;
        }

        public ChartOfAccountsViewModel Load(string ledgerId)
        {
            var accounts = _objectRepository.Load<string, IEnumerable<Account>>(ledgerId).OrderBy(x => x.Order).ThenBy(x => x.TypeEnum).ThenBy(x => x.Name);
            accounts.Each(x => x.Children = x.Children.OrderBy(c => c.Name).ToList());

            var accountsList = accounts.ToList();
            var uncknownCash = accountsList.Where(x => x.Name == "UC" || x.Name == "Unknown Cash").FirstOrDefault();
            //move uncknownCash to the end of list
            if (uncknownCash != null)
            {
                accountsList.Remove(uncknownCash);
                // accountsList.Add(uncknownCash);  // Hidden because we don't want to show unkown cash.  We are working on deprecating this account
            }

            return new ChartOfAccountsViewModel
            {
                Accounts = accountsList
            };
        }

        public ChartOfAccountsViewModel Load(AccountsSortingFilter filter)
        {
            var model = _objectRepository.Load<string, ChartOfAccountsViewModel>(filter.LedgerId);
            model.SortField = filter.Field;
            model.SortDirection = filter.Direction;
            model.Accounts = filter.Field == AccountSortFieldEnum.Custom ? model.Accounts : model.Accounts.OrderBy(filter.Field.ToString(), model.SortDirection).ToList();

            return model;
        }

        public EditManualAccountViewModel Load(EditAccountFilter input)
        {
            var ledger = _ledgerService.GetById(input.LedgerId);
            var accountToEdit = ledger.Accounts.SingleOrDefault(x => x.Id == input.AccountId);
            if (accountToEdit == null)
                throw new Exception("Error - Account Id doesn't exist");

            var accountLabels = Enum.GetValues(typeof(AccountLabelEnum))
                .Cast<AccountLabelEnum>()
                .Where(x => x == accountToEdit.LabelEnum)
                .ToDictionary(x => x.ToString(), x => x.GetDescription());

            var model = new EditManualAccountViewModel
                            {
                                AccountLabels = accountLabels,
                                Description = accountToEdit.Description,
                                Label = accountToEdit.LabelEnum,
                                Type = accountToEdit.TypeEnum,
                                Name = accountToEdit.Name,
                                Number = accountToEdit.Number,
                                ParentAccountId = accountToEdit.ParentAccountId,
                                InstitutionName = accountToEdit.InstitutionName,
                                Id = accountToEdit.Id,
                                Accounts = GetListOfAvailableParentAccounts(ledger.Accounts, accountToEdit.LabelEnum, accountToEdit.Id),
                                InterestRatePercentage = accountToEdit.InterestRatePerc,
                                MinMonthPaymentInDollars =
                                    AccountingFormatter.CentsToDollars(accountToEdit.MinMonthPaymentInCents),
                                CreditLimitInDollars =
                                    AccountingFormatter.CentsToDollars(accountToEdit.CreditLimitInCents),
                            };
            return model;
        }

        public IDictionary<string, string> Load(ParentAccountsFilter input)
        {
            var filteredAccounts = input.Accounts.Where(x => x.LabelEnum == input.AccountLabel && x.Id != input.Id) //should have same lable and can't be parent for itself
                                           .Where(x => string.IsNullOrEmpty(x.ParentAccountId)); //child account can't be parent 
            return filteredAccounts.ToDictionary(x => x.Id, x => x.Name);
        }

        private IDictionary<string, string> GetListOfAvailableParentAccounts(List<AccountDocument> accounts, AccountLabelEnum accountLabel, string id)
        {
            return Load(new ParentAccountsFilter
            {
                Accounts = accounts,
                AccountLabel = accountLabel,
                Id = id
            });
        }
    }

    public class EditAccountFilter
    {
        public string AccountId { get; set; }

        public string LedgerId { get; set; }
    }

    public class ParentAccountsFilter
    {
        public List<AccountDocument> Accounts { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }
        public string Id { get; set; }
    }
}