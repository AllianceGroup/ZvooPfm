namespace mPower.Domain.Membership.User.Data
{
    public class MailData
    {
        public string AffiliateId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
