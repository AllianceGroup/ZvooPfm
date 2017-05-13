using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_OnetimeCalendarEvent_CreateCommandHandler : IMessageHandler<Calendar_OnetimeCalendarEvent_CreateCommand>
    {
        private readonly IRepository _repository;

        public Calendar_OnetimeCalendarEvent_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_OnetimeCalendarEvent_CreateCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);
            calendar.SetCommandMetadata(message.Metadata);

            calendar.AddOnetimeCalendarEvent(new OnetimeCalendarEventData
                                                 {
                                                     UserId = message.UserId,
                                                     CreatedDate = message.CreatedDate,
                                                     EventDate = message.EventDate,
                                                     Description = message.Description,
                                                     SendAlertOptions = message.SendAlertOptions,
                                                     EventId = message.CalendarEventId,
                                                     ParentId = message.ParentId,
                                                     IsDone = message.IsDone,
                                                 });
            _repository.Save(calendar);
        }
    }
}
