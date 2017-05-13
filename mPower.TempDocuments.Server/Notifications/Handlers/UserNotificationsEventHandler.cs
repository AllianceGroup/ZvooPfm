using System;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.User.Events;
using mPower.Framework.Environment;
using mPower.TempDocuments.Server.Notifications.Documents.System;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using Paralect.ServiceBus;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class UserNotificationsEventHandler : IMessageHandler<User_UpdatedResetPasswordTokenEvent>
    {
        private readonly NotificationTempService _notificationService;
        private readonly IIdGenerator _idGenerator;
        private readonly UserDocumentService _userDocumentService;

        public UserNotificationsEventHandler(NotificationTempService notificationService, IIdGenerator idGenerator, UserDocumentService userDocumentService)
        {
            _notificationService = notificationService;
            _idGenerator = idGenerator;
            _userDocumentService = userDocumentService;
        }

        public void Handle(User_UpdatedResetPasswordTokenEvent message)
        {
            var user = _userDocumentService.GetById(message.UserId);
            _notificationService.Insert(new ManuallyCreatedNotification
            {
                Id = _idGenerator.Generate(),
                SendEmail = true,
                Type = EmailTypeEnum.ForgotPassword,
                SendDate = DateTime.Now,
                AffiliateId = user.ApplicationId,
                UserId = user.Id
            });
        }
    }
}