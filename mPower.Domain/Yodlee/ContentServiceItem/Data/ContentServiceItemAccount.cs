using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Data
{
    public class ContentServiceItemAccount
    {
      
        public AccountLabelEnum AccountLabelEnum { get; set; }
        public string AccountId { get; set; }
        public string ItemAccountId { get; set; }
        public string AccountType { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankAccountId { get; set; }
        public long CurrentBalanceInCents { get; set; }
        public long AvailableBalanceInCents { get; set; }
        public List<ContentServiceItemAccountTransaction> Transactions { get; set; }
    }
}