using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_RepeatingCalendarEvent_DeleteCommandHandler :
        IMessageHandler<Calendar_RepeatingCalendarEvent_DeleteCommand>
    {
        private readonly IRepository _repository;

        public Calendar_RepeatingCalendarEvent_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_RepeatingCalendarEvent_DeleteCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);

            calendar.SetCommandMetadata(message.Metadata);
            calendar.DeleteRepeatingCalendarEvent(message.EventId);
            _repository.Save(calendar);
        }
    }
}
