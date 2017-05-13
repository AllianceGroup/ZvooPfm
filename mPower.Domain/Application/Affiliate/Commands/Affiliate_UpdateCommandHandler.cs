using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_UpdateCommandHandler : IMessageHandler<Affiliate_UpdateCommand>
    {
        private readonly Repository _repository;

        public Affiliate_UpdateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_UpdateCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.ApplicationId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            var data = new AffiliateData
            {
                ApplicationId = message.ApplicationId,
                ApplicationName = message.ApplicationName,
                ChargifyApiKey = message.ChargifyApiKey,
                ChargifySharedKey = message.ChargifySharedKey,
                ChargifyUrl = message.ChargifyUrl,
                ContactPhoneNumber = message.ContactPhoneNumber,
                DisplayName = message.DisplayName,
                EmailSuffix = message.EmailSuffix,
                LegalName = message.LegalName,
                MembershipApiKey = message.MembershipApiKey,
                SmtpCredentialsEmail = message.SmtpCredentialsEmail,
                SmtpCredentialsUserName = message.SmtpCredentialsUserName,
                SmtpCredentialsPassword = message.SmtpCredentialsPassword,
                SmtpEnableSsl = message.SmtpEnableSsl,
                SmtpHost = message.SmtpHost,
                SmtpPort = message.SmtpPort,
                UrlPaths = message.UrlPaths,
                AssemblyName = message.AssemblyName,
                JanrainAppApiKey = message.JanrainAppApiKey,
                JanrainAppUrl = message.JanrainAppUrl,
                PfmEnabled = message.PfmEnabled,
                BfmEnabled = message.BfmEnabled,
                CreditAppEnabled = message.CreditAppEnabled,
                SavingsEnabled = message.SavingsEnabled,
                MarketingEnabled = message.MarketingEnabled,
                Address = message.Address,
                SignUpProductId = message.SignUpProductId,
                SignupProductIdWithTrial = message.SignupProductIdWithTrial,
                AdditionalCreditIdentityProduct = message.AdditionalCreditIdentityProduct,
                UpdateDate = message.UpdateDate,
                PublicUrl = message.PublicUrl,
            };

            affiliateAr.Update(data);
            _repository.Save(affiliateAr);
        }
    }
}
