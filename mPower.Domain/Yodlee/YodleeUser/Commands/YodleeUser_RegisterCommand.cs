using Paralect.Domain;

namespace mPower.Domain.Yodlee.YodleeUser.Commands
{
    public class YodleeUser_RegisterCommand : Command
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
}
}
