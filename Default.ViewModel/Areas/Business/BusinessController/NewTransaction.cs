using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class NewTransaction
    {
        public string LedgerId { get; set; }
        public TransactionType Type { get; set; }
        public List<Entry> Entries { get; set; } 
    }
}
