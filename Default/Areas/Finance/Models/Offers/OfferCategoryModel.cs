using System;
using System.Collections.Generic;
using System.Linq;

namespace Default.Areas.Finance.Controllers
{
    public class OfferCategoryModel
    {
        public String Title { get; set; }

        public IEnumerable<OfferCategoryModel> SubCategories { get; set; }

        public bool Selected { get { return Title == SelectedTitle || SubCategories.Any(x => x.Selected); }}

        public string SelectedTitle { get; set; }

        public OfferCategoryModel()
        {
            SubCategories =new List<OfferCategoryModel>();
        }
    }
}