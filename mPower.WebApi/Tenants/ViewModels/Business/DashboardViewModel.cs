using System.Collections.Generic;
using Default.ViewModel;
using Default.ViewModel.Areas.Finance.DashboardController;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using Default.ViewModel.Areas.Shared;
using Category = Default.ViewModel.Areas.Shared.Category;

namespace mPower.WebApi.Tenants.ViewModels.Business
{
    public class DashboardViewModel
    {
        public IEnumerable<GroupedSelectListItem> CategorySelectList { get; set; }
        public AccountsSidebarModel AccountSideBar { get; set; }
        public List<Entry> Entries { get; set; }
        public List<DashboardAlertModel> Alerts { get; set; }
        public List<Category> Categories { get; set; }
        public List<ChartItem> Graph { get; set; }

        public DashboardViewModel()
    	{
    		AccountSideBar = new AccountsSidebarModel();
			Entries = new List<Entry>();
            Categories = new List<Category>();
    	}
    }
}
