using System;
using System.Collections.Generic;
using mPower.Aggregation.Contract.Documents;

namespace Default.Helpers
{
    [Serializable]
    public class DiscoverySession
    {
        public IList<AccountDocument> Accounts { get; set; }
        public List<KeyDocument> Keys { get; set; }
        public int ContentServiceId { get; set; }
    }
}
