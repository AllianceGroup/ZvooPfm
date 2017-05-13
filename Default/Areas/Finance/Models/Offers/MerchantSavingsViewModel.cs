using System.Collections.Generic;
using System.Linq;

namespace Default.Areas.Finance.Controllers
{
    public class MerchantSavingsViewModel
    {
        public IEnumerable<SavingsViewModel> Offers { get; set; }

        public string Merchant { get; set; }

        public double Discount
        {
            get { return Offers.Max(x => x.Discount); }
        }

        public decimal Spent
        {
            get { return Offers.Max(x => x.Spent); }
        }

        public decimal MaxSavings
        {
            get { return Offers.Max(x => x.MaxSavings); }
        }

        public MerchantSavingsViewModel()
        {
            Offers = new List<SavingsViewModel>();
        }
    }
}