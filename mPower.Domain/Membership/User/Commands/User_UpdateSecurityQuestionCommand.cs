using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecurityQuestionCommand : Command
    {
        public string UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

    }
}
