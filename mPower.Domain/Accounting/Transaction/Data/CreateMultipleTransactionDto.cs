using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.Domain.Accounting.Transaction.Data
{
    public class CreateMultipleTransactionDto
    {
        public List<ExpandedEntryData> Entries { get; set; }

        public string TransactionId { get; set; }

        public string LedgerId { get; set; }

        public string BaseAccountId { get; set; }
    }
}
