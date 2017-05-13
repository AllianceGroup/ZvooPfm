using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Business.BusinessController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;

namespace Default.Factories.ViewModels
{
    public class AccountListViewModelFactory: IObjectFactory<string,IEnumerable<Account>>
    {
        private readonly LedgerDocumentService _ledgerService; 

        public AccountListViewModelFactory(LedgerDocumentService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        public IEnumerable<Account> Load(string ledgerId)
        {
            var ledger = _ledgerService.GetById(ledgerId);

            var accounts =
                ledger.Accounts.
                    Where(x => x.Archived == false).
                    Select(x => new Account
                    {
                        Balance = (x.TypeEnum == AccountTypeEnum.Expense || x.TypeEnum == AccountTypeEnum.Income || x.TypeEnum == AccountTypeEnum.Equity) //hide balance
                                        ? String.Empty
                                        : AccountingFormatter.ConvertToDollarsThenFormat(x.ActualBalance),
                        Id = x.Id,
                        ParentAccountId = x.ParentAccountId,
                        Description = x.Description,
                        Label = x.LabelEnum.GetDescription(),
                        LabelEnum = x.LabelEnum,
                        Name = x.Name,
                        Number = x.Number,
                        Type = x.TypeEnum.GetDescription(),
                        TypeEnum = x.TypeEnum,
                        Level = 0,
                        IsCoreAccount = BaseAccounts.All().Contains(x.Id),
                        Order = x.Order
                    }).ToList();

            var hierarchicalOrderedList = new List<Account>();

            // Setting the Level and building hierarhical structure
            var topAccounts = accounts.Where(x => String.IsNullOrEmpty(x.ParentAccountId) || x.ParentAccountId == "0");
            foreach (var top in topAccounts)
            {
                top.Level = 0;
                hierarchicalOrderedList.Add(top);
                GetChildAccounts(top, accounts, 1);
            }
            return hierarchicalOrderedList;
        }

        private void GetChildAccounts(Account parent, List<Account> allAccounts, int level)
        {
            if (parent != null && level < 10) // to avoid infinite loop
            {
                var children = allAccounts.Where(x => x.ParentAccountId == parent.Id);
                foreach (var child in children)
                {
                    child.Level = level;
                    parent.Children.Add(child);
                    GetChildAccounts(child, allAccounts, level + 1);
                }
            }
        }
    }
}