using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Realestate_DeleteCommand : Command
    {
        public string Id { get; set; }

        public string UserId { get; set; }
    }
}