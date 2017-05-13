using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_RepeatingCalendarEvent_CreateCommandHandler :
        IMessageHandler<Calendar_RepeatingCalendarEvent_CreateCommand>
    {
        private const int DAYS_IN_WEEK = 7;

        private readonly IRepository _repository;
        private readonly MPowerSettings _settings;

        public Calendar_RepeatingCalendarEvent_CreateCommandHandler(IRepository repository, MPowerSettings settings)
        {
            _repository = repository;
            _settings = settings;
        }

        public void Handle(Calendar_RepeatingCalendarEvent_CreateCommand message)
        {
            var calendar = _repository.GetById<CalendarAR>(message.CalendarId);
            calendar.SetCommandMetadata(message.Metadata);

            calendar.AddRepeatingCalendarEvent(new RepeatingCalendarEventData
            {
                UserId = message.UserId,
                CreatedDate = message.CreatedDate,
                EventDate = message.EventDate,
                Description = message.Description,
                SendAlertOptions = message.SendAlertOptions,
                End = message.End,
                Frequency = message.Frequency,
                DayAsPartOf = message.DayAsPartOf,
                Period = message.Period,
                RepeatOn = message.RepeatOn,
                StartDate = message.StartDate,
                EventId = message.EventId,
                PrecalculatedData = GetOnetimeEventsFrom(message),
            });

            _repository.Save(calendar);
        }

        #region Repeating Event to Onetime Events Transformation Helpers

        private int[] GetGaps(IEnumerable<DayOfWeek> days, int weekInterval)
        {
            var gaps = new List<int>();
            var count = days.Count();
            for (int i = 0; i < count - 1; i++)
            {
                gaps.Add(days.ElementAt(i + 1) - days.ElementAt(i));
            }
            gaps.Add(DAYS_IN_WEEK * weekInterval - (int)days.ElementAt(count - 1) + (int)days.ElementAt(0));
            return gaps.ToArray();
        }

        private DateTime IncrementDate(DateTime date, CalendarEventFrequencyEnum frequency, DayAsPartOfEnum dayAsPartOf, int periodsNumber, DateTime initialDate)
        {
            switch (frequency)
            {
                case CalendarEventFrequencyEnum.Daily:
                    return date.AddDays(periodsNumber);

                case CalendarEventFrequencyEnum.Monthly:
                    return IncrementDateMonth(date, dayAsPartOf, periodsNumber, initialDate);

                case CalendarEventFrequencyEnum.Yearly:
                    return CorrectDayOfMonth(date.AddYears(periodsNumber), initialDate.Day);

                default:
                    throw new NotImplementedException();
            }
        }

        private DateTime IncrementDateMonth(DateTime date, DayAsPartOfEnum dayAsPartOf, int periodsNumber, DateTime initialDate)
        {
            switch (dayAsPartOf)
            {
                case DayAsPartOfEnum.Month:
                    return CorrectDayOfMonth(date.AddMonths(periodsNumber), initialDate.Day);

                case DayAsPartOfEnum.Week:
                    return CalcDayByWeekNumber(date.AddMonths(periodsNumber), initialDate);

                default:
                    throw new NotImplementedException();
            }
        }

        private DateTime CorrectDayOfMonth(DateTime date, int requiredDay)
        {
            if (date.Day < requiredDay)
            {
                var newDay = Math.Min(DateTime.DaysInMonth(date.Year, date.Month), requiredDay);
                return date.AddDays(newDay - date.Day);
            }
            return date;
        }

        private int GetWeekNumber(DateTime date)
        {
            return (date.Day - 1) / DAYS_IN_WEEK + 1;
        }

        private DateTime CalcDayByWeekNumber(DateTime date, DateTime initialDate)
        {
            var result = date.AddDays(1 - date.Day); // 1st day of target month
            var dayOfWeek = initialDate.DayOfWeek;
            result = result.AddDays(dayOfWeek - result.DayOfWeek + (dayOfWeek >= result.DayOfWeek ? 0 : DAYS_IN_WEEK)); // first occurrence of dayOfWeek
            var weekNum = GetWeekNumber(initialDate);
            for (var i = 1; i < weekNum; i++)
            {
                if (result.AddDays(DAYS_IN_WEEK).Month == date.Month) // do not exceed month last date
                {
                    result = result.AddDays(DAYS_IN_WEEK);
                }
            }
            return result;
        }

        private void CreateEventsBetweenDates(List<OnetimeCalendarEventData> onetimeEvents, DateTime end, Calendar_RepeatingCalendarEvent_CreateCommand message)
        {
            var endBorder = (new DateTime(end.Year, end.Month, end.Day)).AddDays(1);
            if (message.Frequency == CalendarEventFrequencyEnum.Weekly)
            {
                foreach (var day in message.RepeatOn)
                {
                    var start = message.StartDate.DayOfWeek <= day
                        ? message.StartDate.AddDays(day - message.StartDate.DayOfWeek)
                        : message.StartDate.AddDays(DAYS_IN_WEEK - (message.StartDate.DayOfWeek - day));
                    while (start < endBorder)
                    {
                        onetimeEvents.Add(CreateOnetimeEvent(start, message));
                        start = start.AddDays(DAYS_IN_WEEK * message.Period);
                    }
                }
            }
            else
            {
                var start = message.StartDate;
                while (start < endBorder)
                {
                    onetimeEvents.Add(CreateOnetimeEvent(start, message));
                    start = IncrementDate(start, message.Frequency, message.DayAsPartOf, message.Period, message.StartDate);
                }
            }
        }

        private void CreateEventsUntilDate(List<OnetimeCalendarEventData> onetimeEvents, int after, Calendar_RepeatingCalendarEvent_CreateCommand message)
        {
            if (message.Frequency == CalendarEventFrequencyEnum.Weekly)
            {
                int iterator = after;
                int[] gaps = GetGaps(message.RepeatOn, message.Period);
                int i;
                var start = new DateTime();
                for (i = 0; i < message.RepeatOn.Count(); i++)
                {
                    if (message.StartDate.DayOfWeek <= message.RepeatOn.ElementAt(i))
                    {
                        start = message.StartDate.AddDays(message.RepeatOn.ElementAt(i) - message.StartDate.DayOfWeek);
                        break;
                    }
                }

                while (iterator > 0)
                {
                    for (; i < gaps.Count(); i++)
                    {
                        onetimeEvents.Add(CreateOnetimeEvent(start, message));
                        start = start.AddDays(gaps[i]);
                        iterator--;
                    }
                    i = 0;
                }
            }
            else
            {
                var start = message.StartDate;
                for (var i = 0; i < after; i++)
                {
                    onetimeEvents.Add(CreateOnetimeEvent(start, message));
                    start = IncrementDate(start, message.Frequency, message.DayAsPartOf, message.Period, message.StartDate);
                }
            }
        }

        private OnetimeCalendarEventData CreateOnetimeEvent(DateTime start, Calendar_RepeatingCalendarEvent_CreateCommand message)
        {
            return new OnetimeCalendarEventData
            {
                EventId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                EventDate = start,
                Description = message.Description,
                IsDone = false,
                ParentId = message.EventId,
                SendAlertOptions = message.SendAlertOptions,
                UserId = message.UserId,
            };
        }

        private List<OnetimeCalendarEventData> GetOnetimeEventsFrom(Calendar_RepeatingCalendarEvent_CreateCommand message)
        {
            var onetimeEvents = new List<OnetimeCalendarEventData>();
            if (message.End.Never)
            {
                DateTime end = message.StartDate.AddYears(int.Parse(_settings.NeverLengthInYears));
                CreateEventsBetweenDates(onetimeEvents, end, message);
            }

            if (message.End.EndDate.HasValue)
            {
                CreateEventsBetweenDates(onetimeEvents, message.End.EndDate.Value, message);
            }

            if (message.End.After.HasValue)
            {
                CreateEventsUntilDate(onetimeEvents, message.End.After.Value, message);
                onetimeEvents = onetimeEvents.Take(message.End.After.Value).ToList();
            }

            return onetimeEvents;
        }

        #endregion
    }
}
