using System;

namespace mPower.Framework.Utils
{
    public class DateRange
    {
        public static DateRange CreateEmpty()
        {
            return new DateRange(DateTime.MinValue, DateTime.MinValue);
        }

        public bool IsEmpty()
        {
            return From == DateTime.MinValue && To == DateTime.MinValue;
        }

        public DateRange(DateTime from, DateTime to, bool isShort = true)
        {
            From = from;
            To = to;
            FormattedRange = GetFormattedDateRange(from, to, isShort);
        }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string FromFormatted
        {
            get { return From == DateTime.MinValue ? String.Empty : From.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy); }
        }

        public string ToFormatted
        {
            get { return To == DateTime.MinValue ? String.Empty : To.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy); }
        }

        public string FormattedRange { get; private set; }

        private static string GetFormattedDateRange(DateTime from, DateTime to, bool isShort = true)
        {
            bool isSameMonth = from.Month == to.Month;
            bool isSameYear = from.Year == to.Year;

            string dateRange;

            if (isShort)
            {
                if (isSameYear && isSameMonth && from.IsFirstDayOfMonth() && to.IsLastDayOfMonth())
                {
                    dateRange = from.GetFormattedDate(DateTimeFormat.MMM_space_yy);
                }
                else if (isSameYear && isSameMonth)
                {
                    dateRange = String.Format("{0} - {1}", from.GetFormattedDate(DateTimeFormat.MMM_space_dd), to.GetFormattedDate(DateTimeFormat.dd_comma_yy));
                }
                else if (isSameYear && !isSameMonth && from.IsFirstDayOfMonth() && to.IsLastDayOfMonth())
                {
                    dateRange = String.Format("{0} - {1}", from.GetFormattedDate(DateTimeFormat.MMM), to.GetFormattedDate(DateTimeFormat.MMM_space_yy));
                }
                else if (isSameYear && !isSameMonth)
                {
                    dateRange = String.Format("{0} - {1}", from.GetFormattedDate(DateTimeFormat.MMM_space_dd), to.GetFormattedDate(DateTimeFormat.MMM_space_dd_comma_yy));
                }
                else
                {
                    dateRange = String.Format("{0} - {1}", from.GetFormattedDate(DateTimeFormat.MMM_space_dd_comma_yy), to.GetFormattedDate(DateTimeFormat.MMM_space_dd_comma_yy));
                }
            }
            else
            {
                if (isSameYear && isSameMonth)
                {
                    dateRange = String.Format("{0} - {1}, {2}", from.GetFormattedDate(DateTimeFormat.MMMM_space_dd), to.Day, to.Year);
                }
                else if (isSameYear)
                {
                    dateRange = String.Format("{0} through {1} {2}", from.GetFormattedDate(DateTimeFormat.MMMM_space_dd),
                                              to.GetFormattedDate(DateTimeFormat.MMMM_space_dd),
                                              to.Year);
                }
                else
                {
                    dateRange = String.Format("{0} through {1}", from.GetFormattedDate(DateTimeFormat.MMMM_space_dd_space_yyyy),
                                              to.GetFormattedDate(DateTimeFormat.MMMM_space_dd_space_yyyy));
                }
            }

            return dateRange;
        }
    }
}
