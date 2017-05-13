using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_TransactionMap_AddItemCommand : Command
    {
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
        public string Keyword { get; set; }
    }
}