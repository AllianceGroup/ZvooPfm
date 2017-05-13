using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Budget_UpdateCommand : Command
    {
        public string LedgerId { get; set; }
        public string BudgetId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public long AmountInCents { get; set; }
    }
}
