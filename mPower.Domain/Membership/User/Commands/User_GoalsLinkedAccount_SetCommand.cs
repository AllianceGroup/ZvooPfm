using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_GoalsLinkedAccount_SetCommand : Command
    {
        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }
    }
}