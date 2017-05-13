using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;

namespace Default.Factories.ViewModels
{
    public class NetworkOffersFilter
    {
        public string AffiliateId { get; set; }
        public string SeachQuery { get; set; }
        public string Merchat { get; set; }
        [Display(Name = "Offer Type")]
        public OfferTypeEnum? OfferType { get; set; }
        public string Category { get; set; }

        public List<SelectListItem> Merchants { get; set; }
        public List<RootExpenseAccount> SpendingCategories { get; set; }

        public NetworkOffersFilter()
        {
            Merchants = new List<SelectListItem>();
        }
    }
}