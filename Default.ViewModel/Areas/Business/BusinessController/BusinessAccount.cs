using System;

namespace Default.ViewModel.Areas.Business.BusinessController
{
	public class BusinessAccount
	{
        public string AuthenticationReferenceId { get; set; }
	    public string Id { get; set; }
		public string ContentServiceId { get; set; }
		public string ContentServiceItemId { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public decimal BalanceInDollars { get; set; }
        public decimal AggregatedBalanceInDollars { get; set; }
		public string LastUpdatedAgo { get; set; }
        public int Order { get; set; }
        public bool IsUpdating { get; set; }
        public bool IsAggregatedAccount { get; set; }
	    public bool IsUserAttentionRequired { get; set; }
        public string AccountStatus { get; set; }
	    public long? IntuitAccountId { get; set; }
        public long? IntuitInstitutionId { get; set; }

    }
}
