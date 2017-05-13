using System.Collections.Generic;
using Default.Factories.ViewModels;

namespace Default.Areas.Administration.Models
{
    public class MyOffersListingModel
    {
        public MyOffersFilter Filter { get; set; }

        public List<OfferListItemShortModel> Offers { get; set; }
    }
}