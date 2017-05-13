using System.Collections.Generic;
using System.Web.Mvc;

namespace Default.ViewModel.Areas.Credit.Simulator
{
    public class Simulator
    {
        //public int SimulatedScore { get; set; }
        //public string Date { get; set; }
        //public string Social { get; set; }

		public LoanType LoanType { get; set; }
		public int? LoanAmount { get; set; }

		public int Inquiries { get; set; }
		public int? TransferBalance { get; set; }
		public int? IncreaseCreditLimitOnCard { get; set; }
		public int MonthsOnTime { get; set; }
		public int? AddNewCreditCardLimit { get; set; }
		public int OneAccountsDaysPastDue { get; set; }
		public int AllAccountsDaysPastDue { get; set; }
		public int ModifyCreditCardBalance { get; set; }

		public bool PayOffAllCards { get; set; }
		public bool CloseOldestCard { get; set; }
		public bool AddTaxLien { get; set; }
		public bool AddForeclosure { get; set; }
		public bool AddChildSupport { get; set; }
		public bool AddWageGarnishment { get; set; }
		public bool AddOneAccountToCollections { get; set; }
		public bool DeclareBankruptcy { get; set; }

        public string CreditReportId { get; set; }

       

        public IEnumerable<SelectListItem> Socials { get; set; }
        public string SelectedClientKey { get; set; }

        
    }

	public enum LoanType
	{
		Mortgage,
		Auto,
		Personal
	}

    //public enum DaysPastDue
    //{
    //    Thirty,
    //    Sixty,
    //    Ninety
    //}
}
