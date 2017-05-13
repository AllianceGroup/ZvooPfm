namespace mPower.Documents.ExternalServices.Janrain
{
    public class JanrainFacebookResponse : IJanrainResponse
    {
        public JanrainProviderType Provider { get; set; }

        public JanrainFacebookProfile profile { get; set; }

        public string Identifier { get; set; }
    }

    public class JanrainFacebookProfile
    {
        public JanrainGoogleProfileName name { get; set; }

        public string identifier { get; set; }

        public string preferredUsername { get; set; }

        public string providerName { get; set; }

        public string url { get; set; }

        public string verifiedEmail { get; set; }

        public string displayName { get; set; }

        public string email { get; set; }

        public string gender { get; set; }

        public string photo { get; set; }
    }

    public class JanrainFacebookProfileName
    {
        public string givenName { get; set; }
        public string familyName { get; set; }
        public string formatted { get; set; }
    }
}
