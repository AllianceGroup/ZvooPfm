using System.ComponentModel.DataAnnotations;
using mPower.Domain.Application.Enums;

namespace Default.Factories.ViewModels
{
    public class MyOffersFilter
    {
        public string AffiliateId { get; set; }
        public string SeachQuery { get; set; }
        [Display(Name = "Offer Type")]
        public OfferTypeEnum? OfferType { get; set; }
        public OfferStatusEnum? Status { get; set; }
    }
}