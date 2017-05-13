using System;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices;
using mPower.Framework.Environment;

namespace mPower.TempDocuments.Server.Notifications
{
    public class TriggerBuilder
    {
        private readonly UserDocumentService _userService;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IIdGenerator _idGenerator;
        private readonly NotificationTempService _notificationService;

        public TriggerBuilder(UserDocumentService userService,
            AffiliateDocumentService affiliateService,
            IIdGenerator idGenerator,
            NotificationTempService notificationService)
        {
            _userService = userService;
            _affiliateService = affiliateService;
            _idGenerator = idGenerator;
            _notificationService = notificationService;
        }

        public T CreateAndSave<T>(string userId, EmailTypeEnum emailType, Action<T> initAction = null) where T : BaseNotification, new()
        {
            T notification = null;

            var user = _userService.GetById(userId);
            if (user != null)
            {
                var affiliate = _affiliateService.GetById(user.ApplicationId);
                if (affiliate != null)
                {
                    var triggerSettings = affiliate.NotificationTypeEmails.Find(x => x.EmailType == emailType);
                    if (triggerSettings != null && triggerSettings.Status != TriggerStatusEnum.Inactive)
                    {
                        notification = new T
                        {
                            Id = _idGenerator.Generate(),
                            UserId = user.Id,
                            AffiliateId = affiliate.ApplicationId,
                            SendDate = DateTime.Now,
                            SendEmail = true,
                            Type = emailType
                        };

                        if (initAction != null)
                            initAction(notification);

                        _notificationService.Insert(notification);
                    }
                }
            }

            return notification;
        }
    }
}
