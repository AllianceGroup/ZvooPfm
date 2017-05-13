using System.Collections.Generic;

namespace Default.Areas.Finance.Controllers
{
    public class AccountOffersViewModel
    {
        public IEnumerable<MerchantSavingsViewModel> Merchants { get; set; }

        public decimal Spent { get; set; }

        public string AccountName { get; set; }

        public string Id { get; set; }

        public double Discount { get; set; }

        public decimal Savings { get; set; }

        public decimal MaxDiscountInDollars { get; set; }

        public string AccessCategoryName { get; set; }

        public string RootAccountName { get; set; }
    }
}