namespace mPower.Documents.ExternalServices.Janrain
{
    public class JanrainGoogleResponse : IJanrainResponse
    {
        public JanrainProviderType Provider { get; set; }

        public JanrainGoogleProfile profile { get; set; }

        public string Identifier { get; set; }
    }

    public class JanrainGoogleProfile
    {
        public JanrainGoogleProfileName name { get; set; }

        public string identifier { get; set; }

        public string preferredUsername { get; set; }

        public string providerName { get; set; }

        public string url { get; set; }

        public string googleUserId { get; set; }

        public string verifiedEmail { get; set; }

        public string displayName { get; set; }

        public string email { get; set; }
    }

    public class JanrainGoogleProfileName
    {
        public string givenName { get; set; }
        public string familyName { get; set; }
        public string formatted { get; set; }
    }
}
