using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_AuthenticateCommand : Command
    {
        public string AuthenticationReferenceId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int ContentServiceId { get; set; }
        public Dictionary<string, object> PostData { get; set; }
    }
}
