using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class OfferListItemShortModel
    {
        public string CampaignId { get; set; }
        public string Name { get; set; }
        public string Public { get; set; }
        public string Merchant { get; set; }
        public OfferStatusEnum Status { get; set; }
        public string Delivery { get; set; }
    }
}