using System;
using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_UpdateCommand : Command
    {
        public string ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string LegalName { get; set; }

        public string EmailSuffix { get; set; }

        public string ContactPhoneNumber { get; set; }

        public string DisplayName { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool SmtpEnableSsl { get; set; }

        public string SmtpCredentialsEmail { get; set; }

        public string SmtpCredentialsUserName { get; set; }

        public string SmtpCredentialsPassword { get; set; }

        public List<string> UrlPaths { get; set; }

        public string ChargifySharedKey { get; set; }

        public string ChargifyUrl { get; set; }

        public string MembershipApiKey { get; set; }

        public string AssemblyName { get; set; }

        public string JanrainAppApiKey { get; set; }

        public string JanrainAppUrl { get; set; }

        public bool PfmEnabled { get; set; }

        public bool BfmEnabled { get; set; }

        public bool CreditAppEnabled { get; set; }

        public bool SavingsEnabled { get; set; }

        public bool MarketingEnabled { get; set; }

        public string Address { get; set; }

        public int SignUpProductId { get; set; }

        public int SignupProductIdWithTrial { get; set; }

        public int AdditionalCreditIdentityProduct { get; set; }

        public DateTime UpdateDate { get; set; }

        public string PublicUrl { get; set; }

        public string ChargifyApiKey { get; set; }

        public Affiliate_UpdateCommand()
        {
            UpdateDate = DateTime.Now;
        }
    }
}
