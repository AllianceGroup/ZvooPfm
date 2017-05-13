using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class CampaignOfferModel
    {
        // is used for campaign editing
        public string CampaignId { get; set; }
        // is used for campaign creation
        public string SegmentId { get; set; }

        [Display(Name = "Offer Type")]
        public OfferTypeEnum OfferType { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Headline { get; set; }

        [Required]
        [StringLength(1000)]
        public string Body { get; set; }

        [Display(Name = "Logo/Image")]
        public string Logo { get; set; }

        [Display(Name = "Exp. Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Terms & Conditions")]
        [StringLength(250)]
        public string Terms { get; set; }

        public HttpPostedFileBase LogoFile { get; set; }

        public float? OfferValueInPerc { get; set; }
        public decimal? OfferValueInDollars { get; set; }
    }
}