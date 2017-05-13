using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DuplicateDeleteCommand : Command
    {
        public string Id { get; set; }
        public string LedgerId { get; set; }
    }
}