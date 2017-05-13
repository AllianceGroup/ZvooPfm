using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Yodlee.Form;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_AuthenticateMFACommand : Command
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int ContentServiceItemId { get; set; }
        public List<FormInput> PostData { get; set; }
    }
}
