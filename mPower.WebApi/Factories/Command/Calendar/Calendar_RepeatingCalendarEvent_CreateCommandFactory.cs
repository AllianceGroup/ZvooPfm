using System;
using Default.Models;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.WebApi.Tenants.Model;

namespace mPower.WebApi.Factories.Command.Calendar
{
    public class Calendar_RepeatingCalendarEvent_CreateCommandFactory :
        IObjectFactory<AddEventModel, Calendar_RepeatingCalendarEvent_CreateCommand>
    {
        private readonly IIdGenerator _idGenerator;

        public Calendar_RepeatingCalendarEvent_CreateCommandFactory(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        public Calendar_RepeatingCalendarEvent_CreateCommand Load(AddEventModel model)
        {
            var command = new Calendar_RepeatingCalendarEvent_CreateCommand
            {
                UserId = model.UserId,
                CalendarId = model.CalendarId,
                CreatedDate = DateTime.Now,
                EventDate = model.Date.Value,
                Description = model.Description,
                EventId = _idGenerator.Generate(),
                Frequency = model.Frequency,
                Period = model.Repeat,
                RepeatOn = model.Days,
                DayAsPartOf = model.DayAsPartOf,
                StartDate = model.StartDate ?? model.Date.Value,
                SendAlertOptions = Map(model.SendAlertOptions),
                End =
                    new RepeatingEventEndOption
                    {
                        After = model.EndAfter,
                        Never = model.EndEventRepeating == EndEventRepeatingEnum.Never
                    }
            };
            if (model.EndDate.HasValue)
                command.End.EndDate = model.EndDate;

            return command;
        }

        public static SendAlertOption Map(SendAlertOptionModel model)
        {
            return new SendAlertOption
            {
                Count = model.Count,
                Mode = model.Mode,
                TimeRange = model.TimeRange
            };
        }
    }
}