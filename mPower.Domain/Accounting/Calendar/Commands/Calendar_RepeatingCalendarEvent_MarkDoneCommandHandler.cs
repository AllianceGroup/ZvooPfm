using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_RepeatingCalendarEvent_MarkDoneCommandHandler :
        IMessageHandler<Calendar_RepeatingCalendarEvent_MarkDoneCommand>
    {
        private readonly IRepository _repository;

        public Calendar_RepeatingCalendarEvent_MarkDoneCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_RepeatingCalendarEvent_MarkDoneCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);

            calendar.SetCommandMetadata(message.Metadata);
            calendar.MarkRepeatingCalendarEventAsDone(message.EventId);
            _repository.Save(calendar);
        }
    }
}
