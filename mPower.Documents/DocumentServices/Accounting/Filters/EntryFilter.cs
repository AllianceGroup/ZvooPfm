using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;
using System.Web.UI.WebControls;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class EntryFilter : BaseFilter
    {
        public EntryFilter()
        {
            SortDirection = SortDirection.Descending;
        }

        public EntrySortFieldEnum SortByFiled;
        public SortDirection SortDirection { get; set; }
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
        public string OffsetAccountId { get; set; }

        public List<string> AccountIds { get; set; }

        public string TransactionId { get; set; }
        public List<AccountLabelEnum> AccountLabel { get; set; }
    }

    public enum EntrySortFieldEnum
    {
       BookedDate
    }
}
