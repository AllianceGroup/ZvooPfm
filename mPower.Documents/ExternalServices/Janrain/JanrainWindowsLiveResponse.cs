namespace mPower.Documents.ExternalServices.Janrain
{
    public class JanrainWindowsLiveResponse : IJanrainResponse
    {
        public JanrainProviderType Provider { get; set; }

        public JanrainFacebookProfile profile { get; set; }

        public string Identifier { get; set; }
    }

    public class JanrainWindowsLiveProfile
    {
        public string identifier { get; set; }

        public string preferredUsername { get; set; }

        public string url { get; set; }

        public string providerName { get; set; }

        public JanrainGoogleProfileName name { get; set; }

        public string verifiedEmail { get; set; }

        public string displayName { get; set; }

        public string email { get; set; }
    }

    public class JanrainWindowsProfileName
    {
        public string givenName { get; set; }
        public string familyName { get; set; }
        public string formatted { get; set; }
    }
}
