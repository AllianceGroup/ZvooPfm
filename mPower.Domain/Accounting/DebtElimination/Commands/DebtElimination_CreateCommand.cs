using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_CreateCommand : Command
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }
    }
}
