using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Default.Models;
using mPower.Documents.Documents.Calendar;
using mPower.Documents.Documents.Goal;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Factories.Command.Calendar;
using mPower.WebApi.Tenants.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Paralect.Domain;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class CalendarController : BaseController
    {
        private const int DaysPerWeek = 7;
        private const string AllCalendarId = "-1";

        private readonly CalendarDocumentService _calendarService;
        private readonly IIdGenerator _idGenerator;
        private readonly IObjectRepository _objectRepository;

        public CalendarController(CalendarDocumentService calendarService, IIdGenerator idGenerator, 
            IObjectRepository objectRepository, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _calendarService = calendarService;
            _idGenerator = idGenerator;
            _objectRepository = objectRepository;
        }

        [HttpGet("events/filter")]
        public IEnumerable<CalendarEventModel> GetEvents(CalendarViewModelFilter input)
        {
            var filter = Map(input);
            var events = _calendarService.GetOnetimeEventsByFilter(filter);
            return events.Select(Map);
        }

        [HttpGet]
        public CalendarModel GetDefaultCalendar()
        {
            return GetDefaultCalendarModel();
        }

        [HttpPost("add")]
        public IActionResult AddCalendar([FromBody]AddCalendarModel model)
        {
            var command = new Calendar_CreateCommand
            {
                CalendarId = _idGenerator.Generate(),
                LedgerId = GetLedgerId(),
                Name = model.Name,
                Type = CalendarTypeEnum.Additional
            };
            Send(command);
            var calendars = GetAllCalendars();
            return new OkObjectResult(MapToSelectList(calendars, command.CalendarId));
        }

        [HttpGet("events/addmodel")]
        public AddEventModel GetAddEventModel()
        {
            var model = new AddEventModel
            {
                Calendars = MapToSelectList(GetAllCalendars(), includeAllCalendarId: false)
            };
            return model;
        }

        [HttpPost("events/add")]
        public IActionResult AddEvent([FromBody]AddEventModel model)
        {
            if (model.Type == CalendarEventTypeEnum.Onetime)
            {
                if (model.SendAlertOptions.Mode == AlertModeEnum.NotSend)
                {
                    ClearErrorFor(model, x => x.SendAlertOptions.Count);
                    ClearErrorFor(model, x => x.SendAlertOptions.TimeRange);
                }
                if (ModelStateIsValid())
                {
                    model.UserId = GetUserId();
                    var command = _objectRepository.Load<AddEventModel, Calendar_OnetimeCalendarEvent_CreateCommand>(model);
                    Send(command);
                    return new OkResult();
                }
            }
            else if (model.Type == CalendarEventTypeEnum.Repeating)
            {
                ValidateRepeatingEvent(model);
                if (ModelStateIsValid())
                {
                    model.UserId = GetUserId();
                    var command = _objectRepository.Load<AddEventModel, Calendar_RepeatingCalendarEvent_CreateCommand>(model);
                    Send(command);
                    return new OkResult();
                }
            }
            return new BadRequestObjectResult(ModelState);
        }

        [HttpDelete("events/delete")]
        public void DeleteEvent(CalendarDeleteEventCommandFilter filter)
        {
            var command = GetDeleteEventCommand(filter);
            Send(command);
        }

        [HttpPost("events/mark")]
        public void ChangeOnetimeEventStatus(string calendarid, string eventId, bool isDone)
        {
            var command = new Calendar_OnetimeCalendarEvent_ChangeStatusCommand
            {
                CalendarId = calendarid,
                EventId = eventId,
                IsDone = isDone
            };
            Send(command);
        }

        [HttpPost("events/complete/repeating")]
        public void MarkDoneRepeatingEvent(string calendarid, string eventId)
        {
            var command = new Calendar_RepeatingCalendarEvent_MarkDoneCommand
            {
                CalendarId = calendarid,
                EventId = eventId
            };
            Send(command);
        }

        #region Mapping(s)

        private CalendarEventsFilter Map(CalendarViewModelFilter input)
        {
            var filter = new CalendarEventsFilter
            {
                CalendarId = input.CalendarId,
                IsDone = input.IsDone
            };
            UpdateFilter(filter);
            var date = input.Date;
            switch (input.TimeSpanFilter)
            {
                case TimeSpanFilterEnum.ByDay:
                    filter.Date = input.Date;
                    break;
                case TimeSpanFilterEnum.ByWeek:
                    filter.StartDate = date.AddDays(-(int)date.DayOfWeek);
                    filter.EndDate = filter.StartDate.Value.AddDays(DaysPerWeek);
                    break;
                case TimeSpanFilterEnum.ByMonths:
                    filter.StartDate = new DateTime(date.Year, date.Month, 1);
                    filter.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    break;
                case TimeSpanFilterEnum.ByYear:
                    filter.StartDate = new DateTime(date.Year, 1, 1);
                    filter.EndDate = new DateTime(date.Year, 12, DateTime.DaysInMonth(date.Year, 12));
                    break;
                case TimeSpanFilterEnum.ByQuarter:
                    filter.StartDate = date.AddMonths(-1);
                    filter.EndDate = date.AddMonths(1);
                    break;
            }
            return filter;
        }

        private CalendarEventModel Map(CalendarEventDocument doc)
        {
            return new CalendarEventModel
            {
                CalendarId = doc.CalendarId,
                Date = doc.Date,
                Description = doc.Description,
                Id = doc.Id,
                IsDone = doc.IsDone,
                ParentId = doc.ParentId,
                DeleteAction = String.IsNullOrEmpty(doc.ParentId) ? "DeleteOnetimeEvent" : "DeleteRepeatingEvent"
            };
        }

        private IEnumerable<SelectListItem> MapToSelectList(IEnumerable<CalendarDocument> calendars, string selectedCalendarId = AllCalendarId, bool includeAllCalendarId = true)
        {
            var result = new List<SelectListItem>();
            if (includeAllCalendarId)
            {
                result.Add(new SelectListItem()
                {
                    Value = AllCalendarId,
                    Text = "All calendars",
                    Selected = selectedCalendarId == AllCalendarId
                });
            }
            var selectList = new SelectList(calendars, "Id", "Name", selectedCalendarId);
            result.AddRange(selectList);
            return result;
        }


        #endregion

        #region Helprer Method(s)

        private bool ModelStateIsValid()
        {
            return !ModelState.Values.Any(x => x.Errors.Any());
        }

        private void ClearErrorFor<T, TOut>(T model, Expression<Func<T, TOut>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            ClearError(name);
        }

        private void ClearError(string name)
        {
            if (ModelState.ContainsKey(name))
                ModelState[name].Errors.Clear();
        }

        private string GetGoalDescription(GoalDocument doc)
        {
            return $"Goal {doc.Type.GetDescription()}.Planned Date {doc.PlannedDate}";
        }

        private void UpdateFilter(CalendarFilter filter)
        {
            filter.LedgerId = GetLedgerId();
            filter.CalendarId = filter.CalendarId == AllCalendarId ? null : filter.CalendarId;
        }

        private ICommand GetDeleteOnetimeEventCommand(string calendarId, string eventId)
        {
            return GetDeleteEventCommand(new CalendarDeleteEventCommandFilter
            {
                Calendarid = calendarId,
                EventId = eventId,
                Type = CalendarEventTypeEnum.Onetime
            });
        }

        private ICommand GetDeleteEventCommand(CalendarDeleteEventCommandFilter filter)
        {
            return _objectRepository.Load<CalendarDeleteEventCommandFilter, ICommand>(filter);
        }

        private void ValidateRepeatingEvent(AddEventModel model)
        {
            if (model.Frequency == CalendarEventFrequencyEnum.Weekly && !model.Days.Any())
                ModelState.AddModelError(string.Empty, "You should select days of events.");
            if (!model.StartDate.HasValue)
                ModelState.AddModelError(string.Empty, "Start date is required field.");
            if (model.EndEventRepeating != EndEventRepeatingEnum.Never && model.EndAfter == null && model.EndDate == null)
                ModelState.AddModelError(string.Empty, "You should choose any End option. It is requeired.");
        }

        private IEnumerable<CalendarDocument> GetAllCalendars()
        {
            return _calendarService.GetByFilter(new CalendarFilter { LedgerId = GetLedgerId() }).OrderBy(
                c => c.Type);
        }

        private CalendarModel GetDefaultCalendarModel()
        {
            var model = new CalendarModel();
            var calendars = GetAllCalendars();
            model.Calendars = MapToSelectList(calendars);
            model.SelectedCalendarId = AllCalendarId;
            model.LedgerId = GetLedgerId();
            var filter = Map(new CalendarViewModelFilter());
            model.Events = _calendarService.GetOnetimeEventsByFilter(filter).Select(Map);
            return model;
        }
        #endregion
    }
}