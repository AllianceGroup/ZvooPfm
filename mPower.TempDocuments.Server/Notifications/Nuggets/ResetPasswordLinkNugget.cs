using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using System;

namespace mPower.TempDocuments.Server.Notifications.Nuggets
{
    public class ResetPasswordLinkNugget : INugget
    {
        private const string ResetPasswordUrlTemplate = "{0}/resetPassword?token={1}";

        private readonly AffiliateDocumentService _affiliateService;

        public ResetPasswordLinkNugget(AffiliateDocumentService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        public string Tag
        {
            get { return "reset_password_link"; }
        }

        public string DisplayName
        {
            get { return "Reset Password Link"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.ForgotPassword}; }
        }

        public string TestValue
        {
            get { return string.Format(ResetPasswordUrlTemplate, "http://yourdomain.com", Guid.NewGuid()); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var affiliate = _affiliateService.GetById(user.ApplicationId);
            return string.Format(ResetPasswordUrlTemplate, affiliate.UrlPaths.Last().TrimEnd('/'), user.ResetPasswordToken);
        }
    }
}
