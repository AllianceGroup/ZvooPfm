using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_CreateCommand : Command
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
