using System;
using Default.Models;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.WebApi.Tenants.Model;

namespace mPower.WebApi.Factories.Command.Calendar
{
    public class Calendar_OnetimeCalendarEvent_CreateCommandFactory :
        IObjectFactory<AddEventModel, Calendar_OnetimeCalendarEvent_CreateCommand>
    {
        private readonly IIdGenerator _idGenerator;

        public Calendar_OnetimeCalendarEvent_CreateCommandFactory(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        public Calendar_OnetimeCalendarEvent_CreateCommand Load(AddEventModel model)
        {
            return new Calendar_OnetimeCalendarEvent_CreateCommand
            {
                CalendarEventId = _idGenerator.Generate(),
                UserId = model.UserId,
                CalendarId = model.CalendarId,
                CreatedDate = DateTime.Now,
                EventDate = model.Date.Value,
                Description = model.Description,
                IsDone = false,
                SendAlertOptions = Map(model.SendAlertOptions)
            };
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