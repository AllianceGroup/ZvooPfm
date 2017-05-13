using System.Collections.Generic;

namespace Default.Areas.Finance.Controllers
{
    public class OfferViewModel
    {
        private string _onlineRedeemInstructions;
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Merchant { get; set; }

        public List<string> Images { get; set; }

        public string FormatedAward { get; set; }

        public string Terms { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Phone { get; set; }

        public string ZipCode { get; set; }

        public string Url { get; set; }

        public IEnumerable<OfferViewModel> OtherLocations { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string MerchantDescription { get; set; }

        public string MerchantLogoName { get; set; }

        public bool IsPrintable { get; set; }

        public bool IsOnline { get; set; }

        public bool IsPhoneRedeem { get; set; }

        public string OnlineRedeemInstructions
        {
            get { return _onlineRedeemInstructions; }
            set { _onlineRedeemInstructions = (value ?? "").Replace("\\",""); }
        }

        public string MerchantLogoUrl { get; set; }
    }
}