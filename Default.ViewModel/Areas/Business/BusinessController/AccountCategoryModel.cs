using System.Collections.Generic;
using Default.ViewModel.BusinessController;

namespace Default.ViewModel.Areas.Business.BusinessController
{
	public class AccountCategoryModel
	{
		public List<BusinessAccount> BusinessAccounts { get; set; }
		public string Name { get; set; }
		public string Total { get; set; }
	}
}
