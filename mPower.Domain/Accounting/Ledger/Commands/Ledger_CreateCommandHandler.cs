using System;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Framework;
using mPower.Framework.Environment;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_CreateCommandHandler : IMessageHandler<Ledger_CreateCommand>
    {
        private readonly IRepository _repository;
        private readonly CommandService _commandService;
        private readonly IIdGenerator _iidGenerator;
        private readonly MPowerSettings _settings;

        public Ledger_CreateCommandHandler(IRepository repository, CommandService commandService, IIdGenerator generator, MPowerSettings settings)
        {
            _repository = repository;
            _commandService = commandService;
            _iidGenerator = generator;
            _settings = settings;
        }

        public void Handle(Ledger_CreateCommand message)
        {
            var ledger = new LedgerAR(message.LedgerId, new LedgerData
            {
                Name = message.Name,
                TypeEnum = message.TypeEnum,
                Address = message.Address,
                Address2 = message.Address2,
                City = message.City,
                State = message.State,
                Zip = message.Zip,
                TaxId = message.TaxId,
                FiscalYearStart = message.FiscalYearStart,
                CreatedDate = message.CreatedDate,
            }, message.Metadata
            );

            _repository.Save(ledger);

            var calendarCreate = new Calendar_CreateCommand
            {
                CalendarId = _iidGenerator.Generate(),
                LedgerId = message.LedgerId,
                Name = message.Name,
                Type = CalendarTypeEnum.Default,
            };

            var welcomeEventCreate = new Calendar_OnetimeCalendarEvent_CreateCommand
            {
                CalendarId = calendarCreate.CalendarId,
                CalendarEventId = _iidGenerator.Generate(),
                CreatedDate = DateTime.Now,
                Description = _settings.WelcomeEventText,
                EventDate = DateTime.Now,
                SendAlertOptions = new SendAlertOption
                {
                    Mode = AlertModeEnum.NotSend
                },
                UserId = message.Metadata.UserId,
            };

            _commandService.Send(calendarCreate, welcomeEventCreate);
        }
    }
}