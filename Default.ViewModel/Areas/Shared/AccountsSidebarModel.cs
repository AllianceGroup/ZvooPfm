using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Business.BusinessController;
using Default.ViewModel.RealestateController;

namespace Default.ViewModel.Areas.Shared
{
	public class AccountsSidebarModel
	{
	    public List<BusinessAccount> Investments { get; set; }
	    public List<BusinessAccount> Accounts { get; set; }
		public List<BusinessAccount> Loans { get; set; }
		public List<BusinessAccount> Property { get; set; }
		public List<BusinessAccount> CreditCards { get; set; }
        public RealestatesListModel RealEstates { get; set; }

        public decimal AccountsTotalInDollars { get; set; }
        public decimal LoansTotalInDollars { get; set; }
        public decimal PropertyTotalsInDollars { get; set; }
        public decimal CreditCardTotalsInDollars { get; set; }
        public decimal InvestmentTotalsInDollars { get; set; }
        public decimal RealEstatesTotalsInDollars { get; set; }
        public decimal ManualAndAggregatedAccountsTotalInDollars { get; set; }
        public decimal ManualAndAggregatedLoansTotalInDollars { get; set; }
        public decimal ManualAndAggregatedPropertyTotalsInDollars { get; set; }
        public decimal ManualAndAggregatedCreditCardTotalsInDollars { get; set; }
        public decimal ManualAndAggregatedInvestmentTotalsInDollars { get; set; }
        public decimal ManualAndAggregatedRealEstateTotalsInDollars { get; set; }

        public string Equity { get; set; }
        public string AggregatedEquity { get; set; }
		public decimal ManualAndAggregatedEquity { get; set; }

        public bool IsUpdating
        {
            get
            {
                return
                    Accounts.Concat(Loans).Concat(Property).Concat(CreditCards).Concat(Investments)
                        .Any(x => x.IsUpdating && x.IsAggregatedAccount);
            }
        }

		public AccountsSidebarModel()
		{
			Accounts = new List<BusinessAccount>();
			Loans = new List<BusinessAccount>();
			Property = new List<BusinessAccount>();
			CreditCards = new List<BusinessAccount>();
            Investments = new List<BusinessAccount>();
            RealEstates = new RealestatesListModel();
        }
	}

	
}
