namespace mPower.Documents.Documents.Affiliate
{
    public class SmtpSettingsDocument
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public string CredentialsEmail { get; set; }

        public string CredentialsUserName { get; set; }

        public string CredentialsPassword { get; set; }
    }
}
