using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_User_RemoveCommand : Command
    {
        public string LedgerId { get; set; }
        public string UserId { get; set; }
    }
}
