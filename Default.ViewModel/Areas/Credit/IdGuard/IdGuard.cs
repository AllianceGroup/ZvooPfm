using System.Collections.Generic;
using System.Web.Mvc;
using Default.ViewModel.Areas.Credit.Score;
using Default.ViewModel.Areas.Credit.Credit;

namespace Default.ViewModel.Areas.Credit.IdGuard
{
	public class IdGuard
	{
        public bool Enrolled { get; set; }
	    public IEnumerable<SelectListItem> Socials { get; set; }
		public string SelectedClientKey { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
		public string Address { get; set; }
		public string Ssn { get; set; }
		public List<CreditAlert> CreditAlerts { get; set; }

		public IdGuard()
		{
			Socials = new List<SelectListItem>();
			CreditAlerts = new List<CreditAlert>();
		}
	}
}
