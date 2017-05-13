using System.Collections.Generic;
using Default.ViewModel.Areas.Business.BusinessController;

namespace Default.ViewModel.Areas.Business.Categories
{
    public class Categories
    {
        public IEnumerable<Account> Accounts { get; set; }
        public string LedgerId { get; set; }
		public AddManualAccountViewModel AddManualAccountViewModel { get; set; }
    }
}
