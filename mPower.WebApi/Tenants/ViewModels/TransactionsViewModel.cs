using System.Collections.Generic;
using Default.ViewModel;
using Default.ViewModel.Areas.Shared;
using mPower.Framework.Services;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class TransactionsViewModel
    {
        public List<Entry> Entries { get; set; }
        public PagingInfo Paging { get; set; }
        public IEnumerable<GroupedSelectListItem> CategorySelectList { get; set; }
    }
}