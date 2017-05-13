using System.Collections.Generic;
using System.Web.Helpers;
using Default.ViewModel.Areas.Business.BusinessController;

namespace Default.ViewModel.Areas.Business.AccountsController
{
    public class ChartOfAccountsViewModel
    {
        public List<Account> Accounts { get; set; }

        public SortDirection SortDirection { get; set; }

        public AccountSortFieldEnum SortField { get; set; }

        public ChartOfAccountsViewModel()
        {
            SortDirection = SortDirection.Descending;
            SortField = AccountSortFieldEnum.Custom;      
        }

        public string DeleteAccountId { get; set; }
    }

    public enum AccountSortFieldEnum
    {
        Custom = 0,
        Name = 1,
        Type = 2,
        Balance = 3,
    }
}