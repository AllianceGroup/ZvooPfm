namespace Default.Areas.Finance.Controllers
{
    public class OfferPrintModel: OfferViewModel
    {
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string OrganizationName { get; set; }

        public string SupportPhoneNumber { get; set; }

        public string ExpirationDate { get; set; }

        public string DataIdentifier { get; set; }

        public string OrganizationId { get; set; }

        public string MemberId { get; set; }

        public string ChannelId { get; set; }

        public string Exclusions { get; set; }
    }
}