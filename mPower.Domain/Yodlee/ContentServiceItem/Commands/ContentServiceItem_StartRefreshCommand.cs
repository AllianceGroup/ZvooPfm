using Paralect.Domain;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_StartRefreshCommand : Command
    {
        public long ItemId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
