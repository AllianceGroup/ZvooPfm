using System.Collections.Generic;
using Default.Areas.Administration.Models;
using Default.ViewModel;
using mPower.Domain.Accounting;

namespace mPower.WebApi.Tenants.ViewModels.AffiliateAdmin
{
    public class SegmentViewModel
    {
        public SegmentModel Segment { get; set; }

        public List<GroupedSelectListItem> CategoriesList { get; set; }

        public List<RootExpenseAccount> SpendingCategories { get; set; }
    }
}
