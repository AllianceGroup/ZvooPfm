using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Calendar;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Calendar
{
    public class CalendarFilter : BaseFilter
    {
        public string LedgerId { get; set; }

        public string CalendarId { get; set; }
    }

    public class CalendarEventsFilter : CalendarFilter
    {
        public string EventId { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsDone { get; set; }

        public int? Year { get; set; }
        
        public int? Month { get; set; }
    }

    public class CalendarDocumentService : BaseDocumentService<CalendarDocument, CalendarFilter>
    {
        private const int DaysPerWeek = 7;

        public CalendarDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Calendars; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(CalendarFilter filter)
        {           
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }
            if (!string.IsNullOrEmpty(filter.CalendarId))
            {
                yield return Query.EQ("_id", filter.CalendarId);
            }
        }

        //protected virtual IMongoQuery BuildEventsFilterQuery(CalendarEventsFilter filter)
        //{
        //    var query = Query.And(BuildFilterQuery(filter));
        //    if (!string.IsNullOrEmpty(filter.EventId))
        //    {
        //        query = Query.And(Query.EQ("CalendarEvents._id", filter.EventId));
        //    }
        //    if (filter.Date.HasValue)
        //    {
        //        query = Query.And(BuildDateEqualQuery("CalendarEvents.Date", filter.Date.Value));
        //    }
        //    if (filter.StartDate.HasValue)
        //    {
        //        query = Query.And(Query.GTE("CalendarEvents.Date", filter.StartDate));
        //    }
        //    if (filter.EndDate.HasValue)
        //    {
        //        query = Query.And(Query.LTE("CalendarEvents.Date", filter.EndDate));
        //    }

        //    return query;
        //}
        //private IMongoQuery BuildDateEqualQuery(string name, DateTime date)
        //{
        //    return
        //        Query.And(
        //            Query.EQ(String.Format("{0}.{1}", name, "Yaer"), date.Year),
        //            Query.EQ(String.Format("{0}.{1}", name, "Month"), date.Month),
        //            Query.EQ(String.Format("{0}.{1}", name, "Day"), date.Day)
        //            );
        //}

        public virtual IEnumerable<CalendarEventDocument> GetOnetimeEventsByFilter(CalendarEventsFilter filter)
        {
            var calendars = GetByFilter(filter);

            var result = calendars.SelectMany(x => x.CalendarEvents);

            if (!string.IsNullOrEmpty(filter.EventId))
            {
                result = result.Where(x => x.Id == filter.EventId);
            }
            if (filter.Date.HasValue)
            {
                result = result.Where( x => x.Date.Date == filter.Date.Value.Date);
            }
            if (filter.StartDate.HasValue)
            {
                result = result.Where(x => x.Date.Date >= filter.StartDate.Value.Date);
            }
            if (filter.EndDate.HasValue)
            {
                result = result.Where(x => x.Date.Date <= filter.EndDate.Value.Date);
            }
            if (filter.IsDone.HasValue)
            {
                result = result.Where(x => x.IsDone == filter.IsDone.Value);
            }
            if (filter.Month.HasValue)
            {
                result = result.Where(x => x.Date.Month == filter.Month.Value);
            }
            if (filter.Year.HasValue)
            {
                result = result.Where(x => x.Date.Year == filter.Year.Value);
            }
            return result;
        }
    }
}
