using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Enums;
using mPower.Framework.Utils.Extensions;
using mPower.Framework.Utils.Notification;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications
{
    public class NotificationBuilder
    {
        private readonly UserDocumentService _userService;

        public NotificationBuilder(UserDocumentService userService)
        {
            _userService = userService;
        }

        public void InitSendOptions(List<BaseNotification> notifications)
        {
            if (notifications == null || notifications.Count == 0) return;

            notifications.Each(n =>
            {
                var user = _userService.GetById(n.UserId);
                if (user != null)
                {
                    bool sendEmail, sendText;
                    GetSendOptions(n, user, out sendEmail, out sendText);

                    if (sendEmail || sendText)
                    {
                        n.PublicKey = GetPublicKey(n);
                        n.AffiliateId = user.ApplicationId;
                        n.SendEmail = sendEmail;
                        n.SendText = sendText;
                        n.SendDate = CalcSendDate(n);
                    }
                }
            });
        }

        private string GetPublicKey(BaseNotification notification)
        {
            if (notification is CalendarEventAlertDocument)
            {
                return (notification as CalendarEventAlertDocument).CalendarEventId;
            }
            return notification.Id;
        }

        private void GetSendOptions(BaseNotification notification, UserDocument user, out bool sendEmail, out bool sendText)
        {
            sendEmail = sendText = false;

            if (notification.Type.GetNotificationGroup() == NotificationGroupEnum.User)
            {
                var notifSetting = user.Notifications.Find(n => n.Type == notification.Type);
                if (notifSetting != null)
                {
                    sendEmail = notifSetting.SendEmail;
                    sendText = notifSetting.SendText;
                }
            }
            else if (notification.Type.GetNotificationGroup() == NotificationGroupEnum.System)
            {
                switch (notification.Type)
                {
                    case EmailTypeEnum.ManuallyCreated:
                        sendEmail = true;
                        break;

                    case EmailTypeEnum.ForgotPassword:
                        sendEmail = true;
                        break;

                    case EmailTypeEnum.CalendarEventCame:
                        var calendEventAlert = notification as CalendarEventAlertDocument;
                        if (calendEventAlert != null && calendEventAlert.SendAlertOptions != null)
                        {
                            switch (calendEventAlert.SendAlertOptions.Mode)
                            {
                                case AlertModeEnum.Email:
                                    sendEmail = true;
                                    break;
                                case AlertModeEnum.Sms:
                                    sendText = true;
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private DateTime CalcSendDate(BaseNotification notification)
        {
            if (notification is CalendarEventAlertDocument)
            {
                var calendarEventAlert = notification as CalendarEventAlertDocument;
                return CalcSendDate(calendarEventAlert.Date, calendarEventAlert.SendAlertOptions);
            }
            return DateTime.Now;
        }

        private DateTime CalcSendDate(DateTime eventDate, SendAlertOption options)
        {
            var sendDate = eventDate;
            var offset = -options.Count;
            switch (options.TimeRange)
            {
                case SendAlertTimeRange.Minutes:
                    sendDate = sendDate.AddMinutes(offset);
                    break;
                case SendAlertTimeRange.Hours:
                    sendDate = sendDate.AddHours(offset);
                    break;
            }
            return sendDate;
        }


    }
}