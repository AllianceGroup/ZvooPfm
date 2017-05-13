using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Paralect.Domain;
using mPower.Documents;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Documents.Documents.Calendar;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Calendar.Events;
using mPower.Domain.Accounting.DebtElimination.Events;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using mPower.Framework.Environment;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class CalendarDocumentEventHandler :
        IMessageHandler<Calendar_CreatedEvent>,
        IMessageHandler<Calendar_DeletedEvent>,
        IMessageHandler<Calendar_OnetimeCalendarEventAddedEvent>,
        IMessageHandler<Calendar_OnetimeCalendarEventDeletedEvent>,
        IMessageHandler<Calendar_OnetimeCalendarEventChangeStatusEvent>,
        IMessageHandler<Calendar_RepeatingCalendarEventAddedEvent>,
        IMessageHandler<Calendar_RepeatingEventPrecalculated_AddedEvent>,
        IMessageHandler<Calendar_RepeatingCalendarEventDeletedEvent>,
        IMessageHandler<Calendar_RepeatingCalendarEventDoneEvent>,
        IMessageHandler<DebtElimination_AddedToCalendarEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_AddedToCalendarEvent>,
        IMessageHandler<DebtElimination_MortgageProgram_DeletedEvent>,
        IMessageHandler<Ledger_DeletedEvent>
    {

        private readonly CalendarDocumentService _calendarService;
        private readonly IIdGenerator _idGenerator;
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly IEventService _eventService;
        private readonly DebtCalculator _calculator;

        public CalendarDocumentEventHandler(CalendarDocumentService calendarService, IIdGenerator idGenerator, DebtEliminationDocumentService debtEliminationService,
            IEventService eventService, DebtCalculator calculator)
        {
            _calendarService = calendarService;
            _idGenerator = idGenerator;
            _debtEliminationService = debtEliminationService;
            _eventService = eventService;
            _calculator = calculator;
        }

        public void Handle(Calendar_CreatedEvent message)
        {
            var doc = new CalendarDocument
            {
                Id = message.CalendarId,
                LedgerId = message.LedgerId,
                Name = message.Name,
                Type = message.Type
            };

            _calendarService.Save(doc);
        }

        public void Handle(Calendar_DeletedEvent message)
        {
            _calendarService.RemoveById(message.CalendarId);
        }

        public void Handle(Calendar_OnetimeCalendarEventAddedEvent message)
        {
            var calendarEvent = new CalendarEventDocument
            {
                CalendarId = message.CalendarId,
                Date = message.EventDate,
                Description = message.Description,
                Id = message.CalendarEventId,
                IsDone = message.IsDone,
                SendAlertOptions = message.SendAlertOptions,
                ParentId = message.ParentId,
                UserId = message.UserId ?? message.Metadata.UserId,
            };

            IMongoQuery query = Query.EQ("_id", message.CalendarId);

            UpdateBuilder update = Update.Push("CalendarEvents", calendarEvent.ToBsonDocument());

            _calendarService.Update(query, update);
        }

        public void Handle(Calendar_OnetimeCalendarEventDeletedEvent message)
        {
            IMongoQuery query = Query.And(Query.EQ("_id", message.CalendarId));
            UpdateBuilder update = Update.Pull("CalendarEvents", Query.EQ("_id", message.EventId));

            _calendarService.Update(query, update);
        }

        public void Handle(Calendar_OnetimeCalendarEventChangeStatusEvent message)
        {
            IMongoQuery query = Query.And(Query.EQ("_id", message.CalendarId), Query.EQ("CalendarEvents._id", message.EventId));
            UpdateBuilder update = Update.Set("CalendarEvents.$.IsDone", message.NewStatus);

            _calendarService.Update(query, update);
        }

        [Obsolete]
        public void Handle(Calendar_RepeatingCalendarEventAddedEvent message)
        {
            var calendarEvent = new CalendarRepeatingEventDocument
            {
                UserId = message.UserId ?? message.Metadata.UserId,
                CalendarId = message.CalendarId,
                Date = message.Date,
                Description = message.Description,
                Id = message.EventId,
                SendAlertOptions = message.SendAlertOptions,
                End = message.End,
                Frequency = message.Frequency,
                DayAsPartOf = message.DayAsPartOf,
                Period = message.Period,
                RepeatOn = message.RepeatOn,
                StartDate = message.StartDate
            };

            IMongoQuery query = Query.EQ("_id", message.CalendarId);
            UpdateBuilder update = Update.Push("RepeatingEvents", calendarEvent.ToBsonDocument());
            _calendarService.Update(query, update);
        }

        public void Handle(Calendar_RepeatingEventPrecalculated_AddedEvent message)
        {
            var calendarEvent = new CalendarRepeatingEventDocument
            {
                UserId = message.UserId ?? message.Metadata.UserId,
                CalendarId = message.CalendarId,
                Date = message.Date,
                Description = message.Description,
                Id = message.EventId,
                SendAlertOptions = message.SendAlertOptions,
                End = message.End,
                Frequency = message.Frequency,
                DayAsPartOf = message.DayAsPartOf,
                Period = message.Period,
                RepeatOn = message.RepeatOn,
                StartDate = message.StartDate
            };

            IMongoQuery query = Query.EQ("_id", message.CalendarId);
            UpdateBuilder update = Update.Push("RepeatingEvents", calendarEvent.ToBsonDocument());
            _calendarService.Update(query, update);

            if (message.PrecalculatedData != null && message.PrecalculatedData.Count > 0)
            {
                var onetimeEvents = message.PrecalculatedData.Select(x => new CalendarEventDocument
                {
                    CalendarId = message.CalendarId,
                    Date = x.EventDate,
                    Description = message.Description,
                    Id = x.EventId,
                    IsDone = false,
                    SendAlertOptions = message.SendAlertOptions,
                    ParentId = message.EventId,
                    UserId = message.UserId,
                });

                update = Update.PushAllWrapped("CalendarEvents", onetimeEvents);
                _calendarService.Update(query, update);
            }
        }

        public void Handle(Calendar_RepeatingCalendarEventDeletedEvent message)
        {
            IMongoQuery query = Query.And(Query.EQ("_id", message.CalendarId));
            UpdateBuilder update = Update.Pull("RepeatingEvents", Query.EQ("_id", message.EventId));
            _calendarService.Update(query, update);

            query = Query.And(Query.EQ("_id", message.CalendarId));
            update = Update.Pull("CalendarEvents", Query.EQ("ParentId", message.EventId));
            _calendarService.Update(query, update);
        }

        public void Handle(Calendar_RepeatingCalendarEventDoneEvent message)
        {
            IMongoQuery query = Query.And(Query.EQ("_id", message.CalendarId), Query.EQ("RepeatingEvents._id", message.EventId));
            UpdateBuilder update = Update.Set("RepeatingEvents.$.IsDone", true);
            _calendarService.Update(query, update);

            query = Query.And(Query.EQ("_id", message.CalendarId), Query.EQ("CalendarEvents.ParentId", message.EventId));
            update = Update.Set("CalendarEvents.$.IsDone", true);
            _calendarService.Update(query, update);
        }

        public void Handle(DebtElimination_AddedToCalendarEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.CalendarId));
            var update = Update.Pull("CalendarEvents", Query.EQ("ParentId", message.DebtEliminationId));
            _calendarService.Update(query, update);

            var debt = _debtEliminationService.GetById(message.DebtEliminationId);
            var calendar = _calendarService.GetById(message.CalendarId);
            var details = _calculator.CalcAcceleratedDetails(debt);

            var onetimeEvents = details.Select(detailsItem =>
                new Calendar_OnetimeCalendarEventAddedEvent
                {
                    CalendarEventId = _idGenerator.Generate(),
                    CalendarId = calendar.Id,
                    CreatedDate = DateTime.Now,
                    EventDate = detailsItem.Date.AddDays(-1), // warning a day before payment
                    Description = GetEliminationItemDescription(detailsItem),
                    IsDone = false,
                    SendAlertOptions = new SendAlertOption
                    {
                        Mode = AlertModeEnum.Email,
                    },
                    ParentId = message.DebtEliminationId,
                    UserId = debt.UserId,
                }).ToList();

            _eventService.Send(onetimeEvents.Cast<IEvent>().ToArray());
        }

        public void Handle(DebtElimination_MortgageProgram_AddedToCalendarEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.CalendarId));
            var update = Update.Pull("CalendarEvents", Query.EQ("ParentId", message.MortgageProgramId));
            _calendarService.Update(query, update);

            var debt = _debtEliminationService.GetById(message.DebtEliminationId);
            var mortgageProgram = debt.MortgagePrograms.Find(mp => mp.Id == message.MortgageProgramId);
            var calendar = _calendarService.GetById(message.CalendarId);
            var onetimeEvents = mortgageProgram.Details.Select(detailsItem =>
                new Calendar_OnetimeCalendarEventAddedEvent
                {
                    CalendarEventId = _idGenerator.Generate(),
                    CalendarId = calendar.Id,
                    CreatedDate = DateTime.Now,
                    EventDate = detailsItem.Date.AddDays(-1), // warning a day before payment
                    Description = GetMortgageItemDescription(mortgageProgram, detailsItem),
                    IsDone = false,
                    SendAlertOptions = new SendAlertOption
                    {
                        Mode = AlertModeEnum.Email,
                    },
                    ParentId = message.MortgageProgramId,
                    UserId = debt.UserId,
                }).ToList();

            _eventService.Send(onetimeEvents.Cast<IEvent>().ToArray());
        }

        public void Handle(DebtElimination_MortgageProgram_DeletedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.CalendarId));
            var update = Update.Pull("CalendarEvents", Query.EQ("ParentId", message.Id));
            _calendarService.Update(query, update);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var filter = new CalendarFilter { LedgerId = message.LedgerId };
            _calendarService.Remove(filter);
        }

        private static string GetMortgageItemDescription(MortgageAccelerationProgramDocument program, ProgramDetailsItem item)
        {
            return string.Format("{0} mortgage acceleration program. Payment #{1} - {2} ({3}).",
                program.Title, item.Step, AccountingFormatter.ConvertToDollarsThenFormat(item.AccPaymentInCents), item.Date.ToShortDateString());
        }

        public static string GetEliminationItemDescription(ProgramDetailsItemShort item)
        {
            return string.Format("Debt elimination program: loan '{0}' - {1} ({2}).",
                item.Debt, AccountingFormatter.ConvertToDollarsThenFormat(item.ActualPaymentInCents), item.Date.ToShortDateString());
        }
    }
}
