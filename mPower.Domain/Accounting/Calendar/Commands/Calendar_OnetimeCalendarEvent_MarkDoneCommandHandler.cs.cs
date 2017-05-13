using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class CalendarOnetimeCalendarEventChangeStatusCommandHandler : 
        IMessageHandler<Calendar_OnetimeCalendarEvent_ChangeStatusCommand>
    {
        private readonly IRepository _repository;

        public CalendarOnetimeCalendarEventChangeStatusCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_OnetimeCalendarEvent_ChangeStatusCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);

            calendar.SetCommandMetadata(message.Metadata);
            calendar.ChangeOnetimeCalendarEventStatus(message.EventId, message.IsDone);
            _repository.Save(calendar);
        }
    }
}
