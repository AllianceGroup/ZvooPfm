using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DeleteCommand : Command
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }

    }
}
