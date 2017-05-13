using System.Linq;
using mPower.Framework.Environment;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.System;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Messages;
using Paralect.ServiceBus;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class ManualEmailHandler : IMessageHandler<EmailManuallyCreatedMessage>
    {
        private readonly IIdGenerator _idGenerator;
        private readonly NotificationBuilder _notificationBuilder;
        private readonly NotificationTempService _notificationService;

        public ManualEmailHandler(IIdGenerator idGenerator, NotificationBuilder notificationBuilder, NotificationTempService notificationService)
        {
            _idGenerator = idGenerator;
            _notificationBuilder = notificationBuilder;
            _notificationService = notificationService;
        }

        public void Handle(EmailManuallyCreatedMessage message)
        {
            if (message.UsersIds != null && message.UsersIds.Count > 0)
            {
                var notifications = message.UsersIds.Distinct().Select(userId => 
                    new ManuallyCreatedNotification
                    {
                        Id = _idGenerator.Generate(),
                        UserId = userId,
                        AffiliateId = message.AffiliateId,
                        EmailContentId = message.EmailContentId,
                    }).ToList();
                _notificationBuilder.InitSendOptions(notifications.Cast<BaseNotification>().ToList());

                // plan notification if required
                var notifsToSend = notifications.Where(x => x.SendEmail || x.SendText).ToList();
                if (notifsToSend.Count > 0)
                {
                    _notificationService.InsertMany(notifsToSend);
                }
            }
        }
    }
}