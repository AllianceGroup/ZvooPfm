using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils.Extensions;

namespace Default.Helpers
{
    public class SegmentViewHelper
    {
        private readonly AccountsService _accountsService;
        private readonly UserDocumentService _userService;

        public SegmentViewHelper(AccountsService accountsService, UserDocumentService userService)
        {
            _accountsService = accountsService;
            _userService = userService;
        }

        public void BindSegmentSelectLists(dynamic viewBag)
        {
            var expenseAccounts = new List<string> {"Uncategorized Expense"};
            foreach (var account in _accountsService.CommonPersonalExpenseAccounts())
            {
                expenseAccounts.Add(account.Name);
                expenseAccounts.AddRange(account.SubAccounts.Select(x => x.Name));
            }
            expenseAccounts.Sort();
            var categoriesList = expenseAccounts.ToDictionary(x => x, x => AccountLabelEnum.Expense);
            categoriesList.Add("Uncategorized Income", AccountLabelEnum.Income);

            viewBag.CategoriesList = categoriesList.Select(pair => new GroupedSelectListItem
            {
                Text = String.Format("{0}|{1}", pair.Key, pair.Value.GetDescription()),
                Value = pair.Key,
                Group = AccountingFormatter.GenericCategoryGroup(pair.Value)
            });

            viewBag.SpendingCategories = _accountsService.CommonPersonalExpenseAccounts().ToList();
        }

        public string FormatReachNumber(int value, string affiliateId)
        {
            var usersCount = _userService.Count(new UserFilter {AffiliateId = affiliateId});
            return string.Format("{0} of {1}", value, usersCount);
        }
    }
}