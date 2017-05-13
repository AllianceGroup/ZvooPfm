using System.Collections.Generic;
using Default.ViewModel.Areas.Credit.Report;

namespace Default.ViewModel.Areas.Credit.Credit
{
    public class LoadCreditReport
    {
        public PersonalInfo PersonalInfo { get; set; }
        public IEnumerable<string> Inquiries { get; set; }
        public List<AccountGroup> ReportAccountGroups { get; set; }
        public IEnumerable<PublicRecord> PublicRecords { get; set; }
        public List<Creditor> Creditors { get; set; }

        public LoadCreditReport()
        {
            ReportAccountGroups = new List<AccountGroup>();
        }
    }
	
	public class AccountGroup
	{
		public string AccountTypeDescription { get; set; }
		public string AccountTypeSymbol { get; set; }
		public string AccountTypeAbbreviation { get; set; }
		public List<ReportAccount> ReportAccounts { get; set; }
		public AccountGroup()
		{
			ReportAccounts = new List<ReportAccount>();
		}
	}
}
