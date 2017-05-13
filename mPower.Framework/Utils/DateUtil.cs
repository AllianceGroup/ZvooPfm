using System;
using System.Collections.Generic;

namespace mPower.Framework.Utils
{
    public class FiscalQuarter
    {
        private readonly DateTime _fiscalYearStart;

        /// <summary>
        /// Create Fiscal Quarter with Fiscal year start on january, 1
        /// </summary>
        public FiscalQuarter() : this(new DateTime(DateTime.Now.Year, 1, 1))
        {
        }

        public FiscalQuarter(DateTime fiscalYearStart)
        {
            _fiscalYearStart = new DateTime(fiscalYearStart.Year, fiscalYearStart.Month, fiscalYearStart.Day, 0, 0, 0);
        }

        public DateTime GetStartOfQuarter(DateTime date)
        {
            var quarter = GetQuarter(date);

            return new DateTime(date.Year, quarter.From.Month, quarter.From.Day, 0, 0, 0);
        }

        public DateTime GetEndOfQuarter(DateTime date)
        {
            var quarter = GetQuarter(date);

            return new DateTime(date.Year, quarter.To.Month, quarter.To.Day, 23, 59, 59);
        }

        private DateRange GetQuarter(DateTime date)
        {
            var fromDate = DateUtil.GetRangeStart(date, _fiscalYearStart, 3);
            return new DateRange(fromDate, fromDate.AddMonths(3).AddMilliseconds(-1));
        }

        public DateTime GetEndOfLastQuarter()
        {
            var date = GetQuarter(DateTime.Now).To.AddMonths(-3);
            return GetEndOfQuarter(date);
        }

        public DateTime GetStartOfLastQuarter()
        {
            var date = GetQuarter(DateTime.Now).From.AddMonths(-3);
            return GetStartOfQuarter(date);
        }

        public DateTime GetStartOfCurrentQuarter()
        {
            return GetStartOfQuarter(DateTime.Now);
        }

        public DateTime GetEndOfCurrentQuarter()
        {
            return GetEndOfQuarter(DateTime.Now);
        }
    }

    public static class DateUtil
    {
        public static List<DateRange> SplitDateRange(DateTime? fromDate, DateTime? toDate, DateRangeFormatEnum format)
        {
            if (fromDate == null || toDate == null)
                return new List<DateRange>();

            var to = toDate.GetValueOrDefault();
            var from = fromDate.GetValueOrDefault();

            bool IsSameYear = to.Year == from.Year;
            bool IsSameMonth = to.Month == from.Month;

            var dateRanges = new List<DateRange>();

            switch (format)
            {
                case DateRangeFormatEnum.Total:
                    var range = new DateRange(from, to);
                    dateRanges.Add(range);

                    break;
                case DateRangeFormatEnum.Month:
                    if (IsSameMonth && IsSameYear)
                        dateRanges.Add(new DateRange(from, to));
                    else
                    {
                        var start = from;
                        var end = from.GetEndOfMonth();

                        while (end < to)
                        {
                            dateRanges.Add(new DateRange(start, end));
                            start = start.AddMonths(1).GetStartOfMonth();
                            end = start.GetEndOfMonth();
                        }
                        dateRanges.Add(new DateRange(start, to));
                    }

                    break;
                case DateRangeFormatEnum.Quarter:
                    {
                        var fiscalQuarter = new FiscalQuarter();
                        var end = fiscalQuarter.GetEndOfQuarter(from);
                        if (end > to)
                            dateRanges.Add(new DateRange(from, to));
                        else
                        {
                            var start = from;
                            while (end < to)
                            {
                                dateRanges.Add(new DateRange(start, end));
                                start = start.AddMonths(3);
                                start = fiscalQuarter.GetStartOfQuarter(start);
                                end = fiscalQuarter.GetEndOfQuarter(start);
                            }

                            dateRanges.Add(new DateRange(start, to));
                        }
                    }
                    break;
                case DateRangeFormatEnum.Year:
                    {
                        if (IsSameYear)
                        {
                            dateRanges.Add(new DateRange(from, to));
                        }
                        else
                        {
                            var start = from;
                            var end = DateUtil.GetEndOfYear(from.Year);
                            while (end < to)
                            {
                                dateRanges.Add(new DateRange(start, end));
                                start = DateUtil.GetStartOfYear(start.AddYears(1).Year);
                                end = DateUtil.GetEndOfYear(start.Year);
                            }
                            dateRanges.Add(new DateRange(start, to));
                        }
                    }

                    break;
            }


            return dateRanges;
        }

        #region Extentions

        public static string GetFormattedDate(this DateTime date, DateTimeFormat dateFormat)
        {
            if (date == default(DateTime))
                return String.Empty;

            var result = String.Empty;
            switch (dateFormat)
            {
                case DateTimeFormat.MM_slash_dd_slash_yyyy:
                    result = date.ToString("MM/dd/yyyy");
                    break;
                case DateTimeFormat.MMM_space_yy:
                    result = date.ToString("MMM yy");
                    break;
                case DateTimeFormat.MMM_space_dd:
                    result = date.ToString("MMM dd");
                    break;
                case DateTimeFormat.dd_comma_yy:
                    result = date.ToString("dd yy");
                    break;
                case DateTimeFormat.MMM:
                    result = date.ToString("MMM");
                    break;
                case DateTimeFormat.MMMM_space_dd:
                    result = date.ToString("MMMM dd");
                    break;
                case DateTimeFormat.MMM_space_dd_comma_yy:
                    result = date.ToString("MMM dd yy");
                    break;
                case DateTimeFormat.MMMM_space_dd_space_yyyy:
                    result = date.ToString("MMMM dd yyyy");
                    break;
            }

            return result;
        }

        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month,
                DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999);
        }

        public static bool IsLastDayOfMonth(this DateTime date)
        {
            var newDate = date.AddDays(1);

            return date.Month != newDate.Month;
        }

        public static bool IsFirstDayOfMonth(this DateTime date)
        {
            return date.Day == 1;
        }

        public static string ToShortTimeString(this TimeSpan span)
        {
            var fullYearsNumber = (int)(span.TotalDays/365);
            if (fullYearsNumber > 0)
            {
                return FormatTimePeriods(fullYearsNumber, "year", "years");
            }

            var fullMonthsNumber = (int)(span.TotalDays/30);
            if (fullMonthsNumber > 0)
            {
                return FormatTimePeriods(fullMonthsNumber, "month", "months");
            }

            if (span.Days > 0)
            {
                return FormatTimePeriods(span.Days, "day", "days");
            }

            if (span.Hours > 0)
            {
                return FormatTimePeriods(span.Hours, "hour", "hours");
            }

            if (span.Minutes > 0)
            {
                return FormatTimePeriods(span.Minutes, "minute", "minutes");
            }

            return FormatTimePeriods(span.Seconds, "second", "seconds");
        }

        private static string FormatTimePeriods(int periodsNumber, string singlePostfix, string multiPostfix)
        {
            return string.Format("{0} {1}", periodsNumber, periodsNumber == 1 ? singlePostfix : multiPostfix);
        }

        #endregion

        #region Weeks
        public static DateTime GetStartOfLastWeek()
        {
            int DaysToSubtract = (int)DateTime.Now.DayOfWeek + 7;
            DateTime dt =
              DateTime.Now.Subtract(System.TimeSpan.FromDays(DaysToSubtract));
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfLastWeek()
        {
            DateTime dt = GetStartOfLastWeek().AddDays(6);
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }

        public static DateTime GetStartOfCurrentWeek()
        {
            int DaysToSubtract = (int)DateTime.Now.DayOfWeek;
            DateTime dt =
              DateTime.Now.Subtract(System.TimeSpan.FromDays(DaysToSubtract));
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfCurrentWeek()
        {
            DateTime dt = GetStartOfCurrentWeek().AddDays(6);
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }
        #endregion

        #region Months

        public static DateTime GetStartOfMonth(int Month, int Year)
        {
            return new DateTime(Year, Month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfMonth(int Month, int Year)
        {
            return new DateTime(Year, Month,
               DateTime.DaysInMonth(Year, Month), 23, 59, 59, 999);
        }

        public static DateTime GetStartOfLastMonth()
        {
            if (DateTime.Now.Month == 1)
                return GetStartOfMonth(12, DateTime.Now.Year - 1);
            else
                return GetStartOfMonth(DateTime.Now.Month - 1, DateTime.Now.Year);
        }

        public static DateTime GetEndOfLastMonth()
        {
            if (DateTime.Now.Month == 1)
                return GetEndOfMonth(12, DateTime.Now.Year - 1);
            else
                return GetEndOfMonth((DateTime.Now.Month - 1), DateTime.Now.Year);
        }

        public static DateTime GetStartOfCurrentMonth()
        {
            return GetStartOfMonth(DateTime.Now.Month, DateTime.Now.Year);
        }

        public static DateTime GetEndOfCurrentMonth()
        {
            return GetEndOfMonth(DateTime.Now.Month, DateTime.Now.Year);
        }
        #endregion

        #region Years

        public static DateTime GetStartOfYear(int Year)
        {
            return new DateTime(Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime GetEndOfYear(int Year)
        {
            return new DateTime(Year, 12,
              DateTime.DaysInMonth(Year, 12), 23, 59, 59, 999, DateTimeKind.Utc);
        }

        public static DateTime GetStartOfLastYear()
        {
            return GetStartOfYear(DateTime.Now.Year - 1);
        }

        public static DateTime GetEndOfLastYear()
        {
            return GetEndOfYear(DateTime.Now.Year - 1);
        }

        public static DateTime GetStartOfCurrentYear()
        {
            return GetStartOfYear(DateTime.Now.Year);
        }

        public static DateTime GetEndOfCurrentYear()
        {
            return GetEndOfYear(DateTime.Now.Year);
        }

        public static DateTime GetCurrentFiscalYearStart(DateTime originalFiscalYearStart)
        {
            return GetRangeStart(DateTime.Now, originalFiscalYearStart, 12);
        }

        public static DateTime GetRangeStart(DateTime date, DateTime fiscalYearStart, int rangeLengthInMonths)
        {
            var diffInMonth = (date - fiscalYearStart).TotalDays / 365 * 12; // can be positive or negative
            var diffInRanges = ((int)diffInMonth)/rangeLengthInMonths;
            var fromDate = fiscalYearStart.AddMonths(diffInRanges * rangeLengthInMonths); // rough calculation

            // correct this value
            var correction = 0;
            for (var attemptsLeft = 100; fromDate > date || fromDate.AddMonths(rangeLengthInMonths) <= date; attemptsLeft--)
            {
                correction += fromDate > date ? -1 : 1;
                fromDate = fiscalYearStart.AddMonths((diffInRanges + correction) * rangeLengthInMonths);

                if (attemptsLeft < 0)
                {
                    throw new ArgumentException(string.Format("Can't create range for date '{0}' (fiscal year start - {1}, range length - {2} month(s))", date, fiscalYearStart, rangeLengthInMonths));
                }
            }

            return fromDate;
        }

        #endregion

        #region Days
        public static DateTime GetStartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime GetEndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month,
                                 date.Day, 23, 59, 59, 999, DateTimeKind.Utc);
        }
        #endregion
    }

    public enum Quarter
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }

    enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum DateTimeFormat
    {
        /// <summary>
        /// MM/dd/yyyy
        /// </summary>
        MM_slash_dd_slash_yyyy,
        /// <summary>
        /// MMM yy
        /// </summary>
        MMM_space_yy,
        /// <summary>
        /// MMM dd
        /// </summary>
        MMM_space_dd,
        /// <summary>
        /// dd, yy
        /// </summary>
        dd_comma_yy,
        /// <summary>
        /// MMM
        /// </summary>
        MMM,
        /// <summary>
        /// MMM dd, yy
        /// </summary>
        MMM_space_dd_comma_yy,
        /// <summary>
        /// MMMM dd
        /// </summary>
        MMMM_space_dd,
        /// <summary>
        /// MMMM dd yyyy
        /// </summary>
        MMMM_space_dd_space_yyyy
    }

    public enum DateRangeFormatEnum
    {
        Total = 1,
        Month = 2,
        Quarter = 3,
        Year = 4,
    }
}
