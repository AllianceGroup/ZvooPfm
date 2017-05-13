using Paralect.Domain;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_UpdateCommand : Command
    {
        public string AuthenticationReferenceId { get; set; }
        public string ContentServiceItemId { get; set; }
    }
}
