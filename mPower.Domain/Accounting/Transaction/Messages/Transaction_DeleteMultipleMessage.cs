using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Messages
{
    public class Transaction_DeleteMultipleMessage : Event
    {
        public string LedgerId { get; set; }
        public List<string> TransactionIds { get; set; }
    }
}
