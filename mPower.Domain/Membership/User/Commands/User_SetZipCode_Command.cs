using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_SetZipCode_Command: Command
    {
        public string UserId { get; set; }
        public string ZipCode { get; set; }
    }
}