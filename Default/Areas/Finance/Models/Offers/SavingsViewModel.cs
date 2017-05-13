using System.Collections.Generic;
using mPower.Framework.Services;

namespace Default.Areas.Finance.Controllers
{
    public class SavingsViewModel : OfferViewModel
    {
        public PagedOffersShortListModel RelatedOffers { get; set; }

        public double Discount { get; set; }

        public decimal Spent { get; set; }

        public decimal MaxSavings { get; set; }

        public PagedOffersShortListModel YourDeals { get; set; }
    }

    public class PagedOffersShortListModel
    {
        public PagingInfo PagingInfo { get; set; }

        public IEnumerable<OfferListItemModel> Offers { get; set; }

        public string NextUrl { get; set; }

        public string PrevUrl { get; set; }
    }
}