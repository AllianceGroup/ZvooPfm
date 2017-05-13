using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Phone_RemoveCommand : Command
    {
        public string UserId { get; set; }

        public string Phone { get; set; }
    }
}