using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_OnetimeCalendarEvent_DeleteCommandHandler :
        IMessageHandler<Calendar_OnetimeCalendarEvent_DeleteCommand>
    {
        private readonly IRepository _repository;

        public Calendar_OnetimeCalendarEvent_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_OnetimeCalendarEvent_DeleteCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);

            calendar.SetCommandMetadata(message.Metadata);
            calendar.DeleteOnetimeCalendarEvent(message.EventId);
            _repository.Save(calendar);
        }
    }
}
