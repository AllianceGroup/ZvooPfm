using System.Collections.Generic;
using System.Linq;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Calendar.Events;
using mPower.Domain.Application.Enums;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class CalendarEventAlertDocumentEventHandler : 
        IMessageHandler<Calendar_OnetimeCalendarEventAddedEvent>,
        IMessageHandler<Calendar_OnetimeCalendarEventChangeStatusEvent>,
        IMessageHandler<Calendar_OnetimeCalendarEventDeletedEvent>,
        IMessageHandler<Calendar_RepeatingEventPrecalculated_AddedEvent>,
        IMessageHandler<Calendar_RepeatingCalendarEventDoneEvent>,
        IMessageHandler<Calendar_RepeatingCalendarEventDeletedEvent>
    {
        private readonly DashboardAlertBuilder _dashboardAlertBuilder;
        private readonly CalendarDocumentService _calendarService;
        private readonly LedgerDocumentService _ledgerService;

        public CalendarEventAlertDocumentEventHandler(DashboardAlertBuilder dashboardAlertBuilder, CalendarDocumentService calendarService, LedgerDocumentService ledgerService)
        {
            _dashboardAlertBuilder = dashboardAlertBuilder;
            _calendarService = calendarService;
            _ledgerService = ledgerService;
        }

        public void Handle(Calendar_OnetimeCalendarEventAddedEvent message)
        {
            var data = new OnetimeCalendarEventData
            {
                UserId = message.UserId ?? message.Metadata.UserId,
                CreatedDate = message.CreatedDate,
                EventDate = message.EventDate,
                Description = message.Description,
                SendAlertOptions = message.SendAlertOptions,
                EventId = message.CalendarEventId,
                ParentId = message.ParentId,
                IsDone = message.IsDone,
            };

            // fix for old 'incorrect' events
            if (string.IsNullOrEmpty(data.UserId))
            {
                // get userId from calendar
                var calendar = _calendarService.GetById(message.CalendarId);
                if (calendar != null)
                {
                    var ledger = _ledgerService.GetById(calendar.LedgerId);
                    if (ledger != null)
                    {
                        data.UserId = ledger.Users.Select(x => x.Id).FirstOrDefault();
                    }
                }
            }

            if (!string.IsNullOrEmpty(data.UserId))
            {
                _dashboardAlertBuilder.CreateCalendarEventAlert(data, message.CalendarId);
            }
        }

        public void Handle(Calendar_OnetimeCalendarEventChangeStatusEvent message)
        {
            if (message.NewStatus) // is done
            {
                // remove document
                _dashboardAlertBuilder.CancelAlerts(EmailTypeEnum.CalendarEventCame, new List<string> { message.EventId });
            }
            else
            {
                // create document
                var data = new OnetimeCalendarEventData
                {
                    UserId = message.UserId ?? message.Metadata.UserId,
                    CreatedDate = message.CreatedDate,
                    EventDate = message.EventDate,
                    Description = message.Description,
                    SendAlertOptions = message.SendAlertOptions,
                    EventId = message.EventId,
                    ParentId = message.ParentId,
                    IsDone = message.NewStatus,
                };
                _dashboardAlertBuilder.CreateCalendarEventAlert(data, message.CalendarId);
            }
        }

        public void Handle(Calendar_OnetimeCalendarEventDeletedEvent message)
        {
            _dashboardAlertBuilder.CancelAlerts(EmailTypeEnum.CalendarEventCame, new List<string> { message.EventId });
        }

        public void Handle(Calendar_RepeatingEventPrecalculated_AddedEvent message)
        {
            _dashboardAlertBuilder.CreateCalendarEventAlert(message.PrecalculatedData, message.CalendarId);
        }

        public void Handle(Calendar_RepeatingCalendarEventDoneEvent message)
        {
            CancelRepeatingEventNotifications(message.CalendarId, message.EventId);
        }

        public void Handle(Calendar_RepeatingCalendarEventDeletedEvent message)
        {
            CancelRepeatingEventNotifications(message.CalendarId, message.EventId);
        }

        private void CancelRepeatingEventNotifications(string calendarId, string eventId)
        {
            var calendar = _calendarService.GetById(calendarId);
            if (calendar != null)
            {
                var repeatingEvent = calendar.RepeatingEvents.Find(x => x.Id == eventId);
                if (repeatingEvent != null)
                {
                    var onetimeEventsIds = calendar.CalendarEvents.Where(x => x.ParentId == repeatingEvent.Id).Select(x => x.Id).ToList();
                    if (onetimeEventsIds.Count > 0)
                    {
                        _dashboardAlertBuilder.CancelAlerts(EmailTypeEnum.CalendarEventCame, onetimeEventsIds);
                    }
                }
            }
        }
    }
}