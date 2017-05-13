using System.Web.Helpers;
using Default.ViewModel.Areas.Business.AccountsController;

namespace Default.Factories.ViewModels
{
    public class AccountsSortingFilter
    {
        public AccountSortFieldEnum Field { get; set; }
        public SortDirection Direction { get; set; }
        public string ContainerId { get; set; }
        public string LedgerId { get; set; }
    }
}
