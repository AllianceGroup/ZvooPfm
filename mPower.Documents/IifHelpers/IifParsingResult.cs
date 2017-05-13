using System.Collections.Generic;
using mPower.Documents.IifHelpers.Documents;

namespace mPower.Documents.IifHelpers
{
    public class IifParsingResult
    {
        public List<IifAccount> Accounts { get; set; }

        public List<IifTransaction> Transactions { get; set; }

        public IifParsingResult()
        {
            Accounts = new List<IifAccount>();
            Transactions = new List<IifTransaction>();
        }
    }
}
