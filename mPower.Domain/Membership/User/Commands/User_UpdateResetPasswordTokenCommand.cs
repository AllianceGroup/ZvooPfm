using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateResetPasswordTokenCommand : Command
    {
        public string UserId { get; set; }

        public string Token { get; set; }
    }
}
