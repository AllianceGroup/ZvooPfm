using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Phone_AddCommand : Command
    {
        public string UserId { get; set; }

        public string Phone { get; set; }
    }
}