using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Framework.Mvc;
using mPower.WebApi.Tenants.Model;
using Paralect.Domain;

namespace mPower.WebApi.Factories.Command.Calendar
{
    public class Calendar_DeleteEventCommandFactory : 
        IObjectFactory<CalendarDeleteEventCommandFilter, ICommand>
    {
        public ICommand Load(CalendarDeleteEventCommandFilter filter)
        {
            if (filter.Type == CalendarEventTypeEnum.Onetime)
            {
                return new Calendar_OnetimeCalendarEvent_DeleteCommand
                           {
                               CalendarId = filter.Calendarid,
                               EventId = filter.EventId
                           };
            }

            return new Calendar_RepeatingCalendarEvent_DeleteCommand
            {
                CalendarId = filter.Calendarid,
                EventId = filter.ParentId
            };
        }
    }
}