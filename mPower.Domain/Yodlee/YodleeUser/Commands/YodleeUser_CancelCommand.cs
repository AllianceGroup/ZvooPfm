using Paralect.Domain;

namespace mPower.Domain.Yodlee.YodleeUser.Commands
{
    public class YodleeUser_CancelCommand : Command
    {
        
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
