using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Affiliate
{
    public class AffiliateDocument
    {
        public AffiliateDocument()
        {
            Products = new List<ChargifyProductDocument>();
            Smtp = new SmtpSettingsDocument();
            UrlPaths = new List<string>();
            EmailTemplates = new List<EmailTemplateDocument>();
            EmailContents = new List<EmailContentDocument>();
            NotificationTypeEmails = new List<NotificationTypeEmailDocument>();
            Segments = new List<SegmentDocument>();
            Campaigns = new List<CampaignDocument>();
            NetworkCampaigns = new Dictionary<string, List<string>>();
            FaqDocuments = new List<FaqDocument>();
        }

        [BsonId]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string LegalName { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string EmailSuffix { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string DisplayName { get; set; }

        public SmtpSettingsDocument Smtp { get; set; }

        /// <summary>
        /// Gets the base URL paths the applicaiton should utilize
        /// </summary>
        public List<string> UrlPaths { get; set; }

        /// <summary>
        /// The URL user for link on exterbal sites
        /// </summary>
        public string PublicUrl { get; set; }

        /// <summary>
        /// Shared key for the affiliate site on chargify 
        /// </summary>
        public string ChargifyApiKey { get; set; }

        /// <summary>
        /// Shared key for the affiliate site on chargify 
        /// </summary>
        public string ChargifySharedKey { get; set; }

        /// <summary>
        /// Url for the affiliate site on chargify 
        /// </summary>
        /// <remarks>
        /// https://affiliate_domain.chargify.com
        /// </remarks>
        public string ChargifyUrl { get; set; }

        public List<ChargifyProductDocument> Products { get; set; }

        public string MembershipApiKey { get; set; }

        public string AssemblyName { get; set; }

        public string JanrainAppApiKey { get; set; }

        public string JanrainAppUrl { get; set; }

        public List<EmailTemplateDocument> EmailTemplates { get; set; }

        public List<EmailContentDocument> EmailContents { get; set; }

        public List<SegmentDocument> Segments { get; set; }

        public List<NotificationTypeEmailDocument> NotificationTypeEmails { get; set; }

        public List<FaqDocument> FaqDocuments { get; set; }

        public bool PfmEnabled { get; set; }

        public bool BfmEnabled { get; set; }

        public bool CreditAppEnabled { get; set; }

        public bool SavingsEnabled { get; set; }

        public bool MarketingEnabled { get; set; }

        public string Address { get; set; }

        public int SignupProductId { get; set; }

        public int SignupProductIdWithoutTrial { get; set; }

        public int AdditionalCreditIdentityProduct { get; set; }

        public List<CampaignDocument> Campaigns { get; set; }

        public Dictionary<string, List<string>> NetworkCampaigns { get; set; }

        public SmtpClient CreateSmptClient()
        {
            var smtp = new SmtpClient(Smtp.Host, Smtp.Port)
            {
                EnableSsl = Smtp.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            var userName = Smtp.CredentialsUserName != null && !string.IsNullOrEmpty(Smtp.CredentialsUserName.Trim())
                ? Smtp.CredentialsUserName : Smtp.CredentialsEmail;
            if (!string.IsNullOrEmpty(userName))
            {
                smtp.Credentials = new NetworkCredential(userName, Smtp.CredentialsPassword);
            }

            return smtp;
        }


        public void UpdateCampaign(string campaignId, Action<CampaignDocument> updater)
        {
            var campaign = Campaigns.Find(x => x.Id == campaignId);
            if (campaign != null)
            {
                updater(campaign);
            }
        }
    }
}
