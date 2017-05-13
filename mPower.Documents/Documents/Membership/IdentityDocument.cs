using mPower.Documents.ExternalServices.Janrain;

namespace mPower.Documents.Documents.Membership
{
    public class IdentityDocument
    {
        public string Identity { get; set; }

        public JanrainProviderType Provider { get; set; }
    }
}
