using System;
using System.IO;
using System.Linq;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Application.Enums;
using mPower.Framework;
using mPower.Framework.Utils.Extensions;
using Paralect.ServiceBus;
using IIdGenerator = mPower.Framework.Environment.IIdGenerator;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class AffiliateEventHandler :
        IMessageHandler<Affiliate_CreatedEvent>,
        IMessageHandler<Affiliate_SynchronizedChargifyProductsEvent>,
        IMessageHandler<Affiliate_UpdatedEvent>,
        IMessageHandler<Affiliate_DeleteEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IIdGenerator _idGenerator;
        private readonly IEventService _eventService;
        private readonly MPowerSettings _settings;

        public AffiliateEventHandler(AffiliateDocumentService affiliateService, IIdGenerator idGenerator, IEventService eventService, MPowerSettings settings)
        {
            _affiliateService = affiliateService;
            _idGenerator = idGenerator;
            _eventService = eventService;
            _settings = settings;
        }

        public void Handle(Affiliate_CreatedEvent message)
        {
            var affiliate = new AffiliateDocument { ApplicationId = message.Id, ApplicationName = message.Name };

            _affiliateService.Insert(affiliate);
        }

        public void Handle(Affiliate_SynchronizedChargifyProductsEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);

            var update = Update<AffiliateDocument>.Set(x => x.Products, message.Products.Select(product=> new ChargifyProductDocument
                {
                    AccountingCode = product.AccountingCode,
                    Description = product.Description,
                    Handle = product.Handle,
                    Id = product.Id,
                    Name = product.Name,
                    PriceInCents = product.PriceInCents,
                    IsArchived = product.IsArchived,
                    SignUpPage = product.SignUpPage
                }).ToList());

            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_UpdatedEvent message)
        {
            var affiliate = _affiliateService.GetById(message.ApplicationId);
            var publicUrl = affiliate.PublicUrl; 
            affiliate.ApplicationName = message.ApplicationName;
            affiliate.ChargifyApiKey = message.ChargifyApiKey;
            affiliate.ChargifySharedKey = message.ChargifySharedKey;
            affiliate.ChargifyUrl = message.ChargifyUrl;
            affiliate.ContactPhoneNumber = message.ContactPhoneNumber;
            affiliate.DisplayName = message.DisplayName;
            affiliate.EmailSuffix = message.EmailSuffix;
            affiliate.LegalName = message.LegalName;
            affiliate.MembershipApiKey = message.MembershipApiKey;
            affiliate.Smtp.CredentialsEmail = message.SmtpCredentialsEmail;
            affiliate.Smtp.CredentialsUserName = message.SmtpCredentialsUserName;
            affiliate.Smtp.CredentialsPassword = message.SmtpCredentialsPassword;
            affiliate.Smtp.EnableSsl = message.SmtpEnableSsl;
            affiliate.Smtp.Host = message.SmtpHost;
            affiliate.Smtp.Port = message.SmtpPort;
            affiliate.UrlPaths = message.UrlPaths.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            affiliate.AssemblyName = message.AssemblyName;
            affiliate.JanrainAppApiKey = message.JanrainAppApiKey;
            affiliate.JanrainAppUrl = message.JanrainAppUrl;
            affiliate.PfmEnabled = message.PfmEnabled;
            affiliate.BfmEnabled = message.BfmEnabled;
            affiliate.CreditAppEnabled = message.CreditAppEnabled;
            affiliate.SavingsEnabled = message.SavingsEnabled;
            affiliate.MarketingEnabled = message.MarketingEnabled;
            affiliate.Address = message.Address;
            affiliate.SignupProductId = message.SignUpProductId;
            affiliate.SignupProductIdWithoutTrial = message.SignupProductIdWithTrial;
            affiliate.AdditionalCreditIdentityProduct = message.AdditionalCreditIdentityProduct;
            affiliate.PublicUrl = message.PublicUrl;
            _affiliateService.Save(affiliate);

            var affiliateUrl = affiliate.PublicUrl ?? affiliate.UrlPaths.LastOrDefault();
            if (!string.IsNullOrEmpty(affiliateUrl))
            {
                if (!affiliateUrl.EndsWith("/"))
                {
                    affiliateUrl += "/";
                }
                if (affiliate.EmailTemplates.Count == 0)
                {
                    InstallDefaultEmailTemplates(affiliate, message.UpdateDate, affiliateUrl);
                }
                else if (publicUrl != message.PublicUrl)
                {
                    UpdateDefaultEmailTemplates(affiliate, message.UpdateDate, affiliateUrl);
                }
            }
        }

        public void Handle(Affiliate_DeleteEvent message)
        {
            _affiliateService.RemoveById(message.Id);
        }

        private void InstallDefaultEmailTemplates(AffiliateDocument affiliate, DateTime creationDate, string affiliateUrl)
        {
            var html = PrepareDefaultEmailTemplate(affiliate, affiliateUrl);
            if (string.IsNullOrEmpty(html)) return;

            var affiliateId = affiliate.ApplicationId;
            var addTemplateEvent = new Affiliate_Email_Template_AddedEvent
            {
                Id = _idGenerator.Generate(),
                AffiliateId = affiliateId,
                Name = "Default design template",
                Html = html,
                IsDefault = true,
                CreationDate = creationDate,
                Status = TemplateStatusEnum.Active,
            };
            _eventService.Send(addTemplateEvent);

            CreateOrUpdateDefaultNotificationsContent(affiliate, creationDate, affiliateUrl, addTemplateEvent.Id);
        }

        private void UpdateDefaultEmailTemplates(AffiliateDocument affiliate, DateTime updateDate, string affiliateUrl)
        {
            var def = affiliate.EmailTemplates.FirstOrDefault(x => x.IsDefault);
            if (def != null)
            {
                var html = PrepareDefaultEmailTemplate(affiliate, affiliateUrl);
                if (string.IsNullOrEmpty(html)) return;

                var changeTemplateEvent = new Affiliate_Email_Template_ChangedEvent
                {
                    Id = def.Id,
                    AffiliateId = affiliate.ApplicationId,
                    Name = "Default design template",
                    Html = html,
                    Status = def.Status
                };
                _eventService.Send(changeTemplateEvent);

                CreateOrUpdateDefaultNotificationsContent(affiliate, updateDate, affiliateUrl, changeTemplateEvent.Id);
            }
        }

        private string PrepareDefaultEmailTemplate(AffiliateDocument affiliate, string affiliateUrl)
        {
            var stream = new StreamReader(_settings.DefaultEmailTemplatePath);
            string html = null;
            try
            {
                html = stream.ReadToEnd()
                    .Replace("{{affiliateUrl}}", affiliateUrl)
                    .Replace("{{appName}}", affiliate.ApplicationName);
            }
            catch { }

            return html;
        }

        private void CreateOrUpdateDefaultNotificationsContent(AffiliateDocument affiliate, DateTime date, string affiliateUrl, string templateId)
        {
            var affiliateId = affiliate.ApplicationId;
            // set up default email content template for each EmailTypeEnum value
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.ForgotPassword, affiliateId, templateId, date, "Password reset", "<h1 class='h1'>Instructions for resetting your password:</h1><p>To reset your password, click on the link below:</p><p>{{reset_password_link}}</p><br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.CalendarEventCame, affiliateId, templateId, date, "Calendar event notification", "<h1 class='h1'>Reminder:</h1><p><strong>Date:</strong> {{eventDate}}</p><p><strong>Event Description:</strong> {{eventDetails}}</p><br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.LowBalance, affiliateId, templateId, date, EmailTypeEnum.LowBalance.GetDescription(), "<h1 class='h1'>Low Balance:</h1>{{accountName}} has a low balance of {{newBalance}} on {{date}}.<br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.LargePurchases, affiliateId, templateId, date, EmailTypeEnum.LargePurchases.GetDescription(), "<h1 class='h1'>Large Purchases:</h1>{{accountName}} shows a large purchase of {{purchase}} on {{date}}.<br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.BillReminder, affiliateId, templateId, date, EmailTypeEnum.BillReminder.GetDescription(), "<p>We just remind you about planned bill.</p>");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.AvailableCredit, affiliateId, templateId, date, EmailTypeEnum.AvailableCredit.GetDescription(), "<h1 class='h1'>Available Credit:</h1>Your available credit amount of {{accountName}} fell below {{setAmount}} at {{availableCredit}}.<br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.UnusualSpending, affiliateId, templateId, date, EmailTypeEnum.UnusualSpending.GetDescription(), "<h1 class='h1'>Unusual Spending:</h1>Your spending in {{accountName}} is over what you usually spend at {{amount}}.<br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.OverBudget, affiliateId, templateId, date, EmailTypeEnum.OverBudget.GetDescription(), "<h1 class='h1'>Over Budget:</h1>The budget you set up for {{accountName}} in {{month}} was {{budgetAmount}}. You've exceeded that budget by {{difference}}.<br /><br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.Signup, affiliateId, templateId, date, string.Format("Welcome to {0}", affiliate.DisplayName), string.Format("<h1>Congratulations! Your {0} account has been successfully created.</h1><p>Username: <strong>{1}</strong></p><p>Start managing your finances now. <a href='{2}Authentication/Login'>Click here<a>.</p><p>Sincerely,</p><p>{0} Support Team</p><p>This email may contain confidential and privileged material for the sole use of the intended recipient. Any review or distribution by others is strictly prohibited. If you are not the intended recipient, please contact the sender and delete all copies.</p>", affiliate.DisplayName, "{{user_name}}", affiliateUrl));
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.UserDeactivation, affiliateId, templateId, date, EmailTypeEnum.UserDeactivation.GetDescription(), "<h1 class='h1'>Your account was deactivated by administrator</h1> <br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.UserReactivation, affiliateId, templateId, date, EmailTypeEnum.UserReactivation.GetDescription(), "<h1 class='h1'>Your account was reactivated by administrator</h1> <br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.NewAccountAggregation, affiliateId, templateId, date, EmailTypeEnum.NewAccountAggregation.GetDescription(), "<h1 class='h1'>You've added a new account: {{accountName}}</h1> <br />");
            CreateOrUpdateDefaultNotificationContent(affiliate, EmailTypeEnum.NewCreditIdentity, affiliateId, templateId, date, EmailTypeEnum.NewCreditIdentity.GetDescription(), "<h1 class='h1'>You've added new credit identity: {{ssn}}</h1> <br />");
        }

        private void CreateOrUpdateDefaultNotificationContent(AffiliateDocument affiliate, EmailTypeEnum emailType, string affiliateId, string templateId, DateTime date, string subject, string html, string name = null)
        {
            var current = affiliate.EmailContents.FirstOrDefault(x => x.IsDefaultForEmailType == emailType);
            if (current == null)
            {
                var contentMessage = new Affiliate_Email_Content_AddedEvent
                {
                    Id = _idGenerator.Generate(),
                    AffiliateId = affiliateId,
                    TemplateId = templateId,
                    Name = name ?? subject,
                    Subject = subject,
                    Html = html,
                    IsDefaultForEmailType = emailType,
                    CreationDate = date,
                };
                _eventService.Send(contentMessage);

                var notificationTypeEmailEvent = new Affiliate_NotificationTypeEmail_AddedEvent
                {
                    AffiliateId = affiliateId,
                    Name = emailType.GetDescription(),
                    EmailType = emailType,
                    EmailContentId = contentMessage.Id,
                    Status = TriggerStatusEnum.Active,
                };
                _eventService.Send(notificationTypeEmailEvent);
            }
            else
            {
                var contentMessage = new Affiliate_Email_Content_ChangedEvent
                {
                    Id = current.Id,
                    AffiliateId = affiliateId,
                    TemplateId = templateId,
                    Name = name ?? subject,
                    Subject = subject,
                    Html = html,
                    Status = current.Status
                };
                _eventService.Send(contentMessage);
            }
        }
    }
}
