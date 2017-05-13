using System.Collections.Generic;
using Default.Factories.ViewModels;

namespace Default.Areas.Administration.Models
{
    public class NetworkOffersListingModel
    {
        public NetworkOffersFilter Filter { get; set; }

        public List<OfferListItemModel> Offers { get; set; }
    }
}