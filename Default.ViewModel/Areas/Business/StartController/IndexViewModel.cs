using System.Collections.Generic;

namespace Default.ViewModel.Areas.Business.StartController
{
    public class IndexViewModel
    {
        public List<Company> Companies { get; set; }
        public AddLedgerViewModel AddLedger { get; set; }
        

        public IndexViewModel()
        {
            AddLedger = new AddLedgerViewModel();
            Companies = new List<Company>();
        }
    }
}