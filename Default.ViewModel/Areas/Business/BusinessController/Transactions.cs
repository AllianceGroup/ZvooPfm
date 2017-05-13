using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;

namespace Default.ViewModel.Areas.Business.BusinessController
{
	public class Transactions
	{
		public List<Entry> Entries { get; set; }
		public string LedgerId { get; set; }
		public AccountsSidebarModel AccountsSideBar { get; set; }
	}
}
