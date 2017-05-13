using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.Ledger
{
    public class AccountCollection
    {
        public readonly Dictionary<string, Account> AccountsById = new Dictionary<string, Account>();

        public Int32 Count
        {
            get { return AccountsById.Count; }
        }

        public void Add(Account account)
        {
            AccountsById[account.Id] = account;
        }

        public Account Get(String accountId)
        {
            return AccountsById[accountId];
        }

        public void Remove(String accountId)
        {
            AccountsById.Remove(accountId);
        }

        public Boolean Exists(String accountId)
        {
            return AccountsById.ContainsKey(accountId);
        }
    }
}