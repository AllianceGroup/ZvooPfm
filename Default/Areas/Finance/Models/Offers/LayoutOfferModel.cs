using System.Collections.Generic;

namespace Default.Areas.Finance.Controllers
{
    public class LayoutOfferModel
    {
        public OfferFilterModel Filter { get; set; }

        public IEnumerable<OfferCategoryModel> Categories { get; set; }
        
        public bool ShowLayout { get; set; }

        public string ContainerClass { get; set; }

        public LayoutOfferModel()
        {
            ShowLayout = true;
            Categories = new List<OfferCategoryModel>();
        }
    }
}