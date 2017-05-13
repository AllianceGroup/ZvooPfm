using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_ReAuthenticateCommand : Command
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int ContentServiceItemId { get; set; }
        public Dictionary<string, object> PostData { get; set; }
    }
}
