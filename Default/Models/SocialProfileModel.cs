using System.Collections.Generic;
using mPower.Documents.ExternalServices.Janrain;

namespace Default.Models
{
    public class SocialProfileModel
    {
        public string JanrainAppUrl { get; set; }

        public List<JanrainProviderType> AcceptedProviders { get; set; }
    }
}