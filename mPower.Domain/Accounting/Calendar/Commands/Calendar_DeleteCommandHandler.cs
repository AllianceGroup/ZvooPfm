using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_DeleteCommandHandler : IMessageHandler<Calendar_DeleteCommand>
    {
        private readonly IRepository _repository;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Calendar_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_DeleteCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);
            calendar.SetCommandMetadata(message.Metadata);
            calendar.Delete();

            _repository.Save(calendar);
        }
    }
}
