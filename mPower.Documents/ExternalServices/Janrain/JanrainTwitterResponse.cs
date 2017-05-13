namespace mPower.Documents.ExternalServices.Janrain
{
    public class JanrainTwitterResponse : IJanrainResponse
    {
        public JanrainProviderType Provider { get; set; }

        public JanrainTwitterProfile profile { get; set; }
        
        public string Identifier { get; set; }
    }

    public class JanrainTwitterProfile
    {
        public JanrainTwitterProfileName name { get; set; }

        public string identifier { get; set; }

        public string preferredUsername { get; set; }

        public string providerName { get; set; }

        public string url { get; set; }

        public string photo { get; set; }

        public string displayName { get; set; }
    }

    public class JanrainTwitterProfileName
    {
        public string formatted { get; set; }
    }
}
