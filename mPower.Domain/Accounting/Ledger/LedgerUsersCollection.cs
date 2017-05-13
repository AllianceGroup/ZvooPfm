using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.Ledger
{
    public class LedgerUsersCollection
    {
        public readonly Dictionary<string, string > UsersById = new Dictionary<string, string>();

        public Int32 Count
        {
            get { return UsersById.Count; }
        }

        public void Add(string userId)
        {
            UsersById[userId] = userId;
        }

        public string Get(String userId)
        {
            return UsersById[userId];
        }

        public void Remove(String userId)
        {
            UsersById.Remove(userId);
        }

        public Boolean Exists(String userId)
        {
            return UsersById.ContainsKey(userId);
        }
    }
}
