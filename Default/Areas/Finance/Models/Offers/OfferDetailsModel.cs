namespace Default.Areas.Finance.Controllers
{
    public class OfferDetailsModel: LayoutOfferModel
    {
        public OfferViewModel Offer { get; set; }

        public OfferDetailsModel()
        {
            Offer = new OfferViewModel();
        }
    }
}