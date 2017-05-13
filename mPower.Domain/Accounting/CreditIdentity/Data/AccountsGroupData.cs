using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class AccountsGroupData
    {
        public string AccountTypeDescription { get; set; }
        public string AccountTypeSymbol { get; set; }
        public string AccountTypeAbbreviation { get; set; }
        public List<AccountData> Accounts { get; set; }  
    }
}