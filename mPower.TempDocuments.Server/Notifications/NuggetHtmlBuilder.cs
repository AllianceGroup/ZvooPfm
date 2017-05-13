using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.System;
using mPower.TempDocuments.Server.Notifications.Nuggets;

namespace mPower.TempDocuments.Server.Notifications
{
    public class NuggetHtmlBuilder : IEmailHtmlBuilder
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly UserDocumentService _userService;
        private readonly List<INugget> _nuggets;

        public NuggetHtmlBuilder(AffiliateDocumentService affiliateService, UserDocumentService userService, List<INugget> nuggets)
        {
            _nuggets = nuggets;
            _affiliateService = affiliateService;
            _userService = userService;
        }

        public List<INugget> AllNuggets
        {
            get { return _nuggets; }
        }

        public string BuildHtml(string htmlWithTags, BaseNotification notification)
        {
            var result = htmlWithTags;

            if (!String.IsNullOrEmpty(result))
            {
                var user = notification == null ? null : _userService.GetById(notification.UserId);
                foreach (var nugget in _nuggets)
                {
                    var tag = ToNuggetTag(nugget.Tag);
                    if (result.Contains(tag))
                    {
                        var value = user == null
                            ? (notification == null ? nugget.TestValue : "UNKNOWN") // default value for nugget
                            : nugget.GetValue(user, notification);
                        result = result.Replace(tag, value);
                    }
                }
            }
            return result;
        }

        public void GenerateEmailData(BaseNotification notification, out string subject, out string body)
        {
            var affiliate = GetCheckedAffiliate(notification.AffiliateId);
            var emailContent = GetCheckedEmailContent(affiliate, notification);
            var emailTemplate = GetCheckedEmailTemplate(affiliate, notification.Type, emailContent.TemplateId);

            subject = emailContent.Subject;
            var html = emailTemplate.Html.Replace(ToNuggetTag("Body"), emailContent.Html);
            body = BuildHtml(html, notification);
        }

        private static string ToNuggetTag(string nuggetCode)
        {
            return "{{" + nuggetCode + "}}";
        }

        private AffiliateDocument GetCheckedAffiliate(string id)
        {
            var affiliate = _affiliateService.GetById(id);
            if (affiliate == null)
            {
                throw new ArgumentException(string.Format("Cannot find affiliate with id = '{0}'.", id));
            }
            return affiliate;
        }

        private EmailContentDocument GetCheckedEmailContent(AffiliateDocument affiliate, BaseNotification notification)
        {
            string emailContentId;
            if (notification.Type == EmailTypeEnum.ManuallyCreated)
            {
                var manuallyCreatedNotification = notification as ManuallyCreatedNotification;
                if (manuallyCreatedNotification == null)
                {
                    throw new ArgumentException(string.Format("Incorrect notification format: affiliate - '{0}', emailType - {1}.", affiliate.ApplicationName, notification.Type));
                }
                emailContentId = manuallyCreatedNotification.EmailContentId;
            }
            else
            {
                var notifTypeEmail = affiliate.NotificationTypeEmails.LastOrDefault(nte => nte.EmailType == notification.Type);
                if (notifTypeEmail == null || string.IsNullOrEmpty(notifTypeEmail.EmailContentId))
                {
                    throw new ArgumentException(string.Format("Notifcation settings were not installed correctly: affiliate - '{0}', emailType - {1}.", affiliate.ApplicationName, notification.Type));
                }
                emailContentId = notifTypeEmail.EmailContentId;
            }

            
            var emailContent = affiliate.EmailContents.LastOrDefault(c => c.Id == emailContentId);
            if (emailContent == null || string.IsNullOrEmpty(emailContent.TemplateId))
            {
                throw new ArgumentException(string.Format("Email content was not installed correctly: affiliate - '{0}', emailType - {1}, contentId - '{2}'.", affiliate.ApplicationName, notification.Type, emailContentId));
            }
            return emailContent;
        }

        private EmailTemplateDocument GetCheckedEmailTemplate(AffiliateDocument affiliate, EmailTypeEnum emailType, string templateId)
        {
            var emailTemplate = affiliate.EmailTemplates.LastOrDefault(t => t.Id == templateId);
            if (emailTemplate == null)
            {
                throw new ArgumentException(string.Format("Email template was not found: affiliate - '{0}', emailType - {1}, templateId - {2}.", affiliate.ApplicationName, emailType, templateId));
            }
            return emailTemplate;
        }
    }
}
