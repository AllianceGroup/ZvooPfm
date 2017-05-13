using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_DeactivateCommand : Command
    {
        public string UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
