using System.Collections.Generic;
using System.Web.Mvc;

namespace Default.ViewModel.Areas.Credit.Credit
{
    public class CreditSocials
    {
        public string NewReportMessage { get; set; }
        public IEnumerable<SelectListItem> Socials  { get; set; }
		public string SelectedClientKey { get; set; }
		public string CreditGrade { get; set; }
    }
}
