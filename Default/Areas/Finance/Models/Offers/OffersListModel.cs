using System.Collections.Generic;

namespace Default.Areas.Finance.Controllers
{
    public class OffersListModel: LayoutOfferModel
    {
        public IEnumerable<OfferListItemModel> Offers { get; set; }

    }
}