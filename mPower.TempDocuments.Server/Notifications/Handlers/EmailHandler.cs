using System.Net.Mail;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Membership;
using mPower.Framework;
using mPower.TempDocuments.Server.Notifications.Messages;
using Paralect.ServiceBus;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class EmailHandler : IMessageHandler<SendMailMessage>
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly UserDocumentService _userService;
        private readonly MPowerSettings _settings;

        public EmailHandler(AffiliateDocumentService affiliateService, UserDocumentService userService, MPowerSettings settings)
        {
            _affiliateService = affiliateService;
            _userService = userService;
            _settings = settings;
        }

        public void Handle(SendMailMessage message)
        {
            var affiliate = _affiliateService.GetById(message.AffiliateId);
            var smtp = affiliate.CreateSmptClient();

            using (var mailMessage = new MailMessage { Subject = message.Subject, Body = message.Body, IsBodyHtml = message.IsBodyHtml })
            {
                mailMessage.From = new MailAddress(affiliate.Smtp.CredentialsEmail, affiliate.DisplayName);

                if (_settings.EnvironmentTypeEnum == EnvironmentTypeEnum.Prod)
                {
                    foreach (var userId in message.UsersIds)
                    {
                        var user = _userService.GetById(userId);
                        mailMessage.To.Add(user.Email);
                        foreach (var additionalEmail in user.AdditionalEmails)
                        {
                            mailMessage.To.Add(additionalEmail);
                        }
                    }
                    if (mailMessage.To.Count == 0) { mailMessage.To.Add(_settings.WhiteEmailsList); }
                    smtp.Send(mailMessage);
                }
                else // otherwise send email to white list
                {
                    mailMessage.To.Add(_settings.WhiteEmailsList);
                    smtp.Send(mailMessage);
                }
            }
        }
    }
}
