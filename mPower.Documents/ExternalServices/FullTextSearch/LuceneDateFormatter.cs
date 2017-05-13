using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class LuceneDateFormatter
    {
        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";

        public static long ConvertToLucene(DateTime date)
        {
            return date.Ticks;
        }

        public static DateTime ConvertFromLucene(string date)
        {
            return new DateTime(long.Parse(date));
        }
    }
}
