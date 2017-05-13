using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Email_RemoveCommand : Command
    {
        public string UserId { get; set; }

        public string Email { get; set; }
    }
}