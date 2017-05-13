using System;
using System.Collections.Generic;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public enum TransactionsSortFieldEnum
    {
        Default = 1,
        BookedDate = 2,
        BookedDateDescending = 3,
    }

    public class TransactionFilter : BaseFilter
    {
        
        public TransactionsSortFieldEnum SortByFiled { get; set; }

        public TransactionFilter()
        {
            AccountIds = new List<string>();
        }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public List<string> AccountIds { get; set; }

        public DateTime? EntryFromDate { get; set; }

        public DateTime? EntryToDate { get; set; }

       
    }
}
