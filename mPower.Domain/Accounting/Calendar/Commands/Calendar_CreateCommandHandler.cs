using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_CreateCommandHandler : IMessageHandler<Calendar_CreateCommand>
    {
        private readonly IRepository _repository;

        public Calendar_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Calendar_CreateCommand message)
        {
            var calendar = new CalendarAR(new CalendarData
            {
                Name = message.Name,
                CalendarId = message.CalendarId, Type = message.Type,
                LedgerId = message.LedgerId
            }, message.Metadata);

            _repository.Save(calendar);

        }
    }
}