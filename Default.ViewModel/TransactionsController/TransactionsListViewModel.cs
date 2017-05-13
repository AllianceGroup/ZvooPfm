using System;
using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;

namespace Default.ViewModel.TransactionsController
{
    public class TransactionsListViewModel
    {
        public TransactionsListViewModel()
        {
            Entries = new List<Entry>();
            HiddenAuxDataForScript = new Dictionary<string, string>();
            Mode = null;
        }

        public List<Entry> Entries { get; set; }
        public AccountsSidebarModel AccountsSidebar { get; set; }
        public List<Category> Categories { get; set; }
        public IEnumerable<GroupedSelectListItem> CategorySelectList { get; set; }
        public PagingInfo Paging { get; set; }
        public Dictionary<string, string> HiddenAuxDataForScript { get; set; }
        public int TotalEntryCount { get; set; }

        public string AccountName { get; set; }
        public string AccountId { get; set; }

        public string SearchText { get; set; }
        public AccountLabelEnum? Mode { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public bool IsFilteringByAccount
        {
            get { return !String.IsNullOrEmpty(AccountId); }
        }

    }
}