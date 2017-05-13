using System;
using System.Collections.Generic;
using mPower.Framework;
using mPower.Schedule.Server.Environment;
using mPower.TempDocuments.Server.Notifications;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Messages;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace mPower.Schedule.Server.Jobs
{
    public class NotificationSendScheduleJob : IScheduledJob
    {
        private readonly NotificationTempService _notificationService;
        private readonly IEventService _eventService;
        private readonly IEmailHtmlBuilder _emailBuilder;

        public NotificationSendScheduleJob(NotificationTempService notificationService, 
            IEventService eventService, 
            IEmailHtmlBuilder emailBuilder)
        {
            _notificationService = notificationService;
            _eventService = eventService;
            _emailBuilder = emailBuilder;
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("EmailSenderJob", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("NotificationSenderEachHundredMilesecondsTrigger", Int32.MaxValue, TimeSpan.FromMilliseconds(100));
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        public void Execute(IJobExecutionContext context)
        {
            var notification = _notificationService.GetNotificationAndSetInProgress();
            if (notification != null && notification.UserId != null)
            {
                if (notification.SendEmail)
                {
                    GenerateAndSendEmail(notification);
                }

                if (notification.SendText)
                {
                    // TODO: implement SendText option
                }

                _notificationService.RemoveById(notification.Id);
            }
        }

        private void GenerateAndSendEmail(BaseNotification notification)
        {
            string subject, body;
            _emailBuilder.GenerateEmailData(notification, out subject, out body);

            // DO NOT SEND INVALID MAILS
            if (String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(body))
            {
                throw new ArgumentException("Email template wasn't set correctly.");
            }

            var sendMailEvent = new SendMailMessage
            {
                UsersIds = new List<string> { notification.UserId },
                AffiliateId = notification.AffiliateId,
                Subject = subject,
                Body = body,
            };
            _eventService.Send(sendMailEvent);
        }
    }
}
