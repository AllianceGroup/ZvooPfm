using Paralect.Domain;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DeleteMultipleCommand : Command
    {
        public string LedgerId { get; set; }
        public List<string> TransactionIds { get; set; }
    }
}
