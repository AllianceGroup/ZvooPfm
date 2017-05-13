using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Web.Mvc
{
    public static class SelectListHelper
    {

        public static IEnumerable<SelectListItem> Days
        {
            get
            {
                var returnData = new List<SelectListItem>();
                for (var i = 1; i <= 31; i++)
                {
                    returnData.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }

                return
                    returnData.AsEnumerable();
            }
        }

        public static IEnumerable<SelectListItem> Months
        {
            get
            {
                return DateTimeFormatInfo
                    .InvariantInfo
                    .MonthNames
                    .Where(m => !String.IsNullOrEmpty(m))
                    .Select((monthName, index) => new SelectListItem
                                                      {
                                                          Value = (index + 1).ToString(),
                                                          Text = monthName
                                                      });
            }
        }

        public static IEnumerable<SelectListItem> Years
        {
            get
            {
                var returnData = new List<SelectListItem>();
                for (var i = 1900; i <= DateTime.Now.Year; i++)
                {
                    returnData.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }

                return
                    returnData.AsEnumerable().Reverse();
            }
        }

        public static IEnumerable<SelectListItem> Suffixes
        {
            get { return new List<SelectListItem>() {
                new SelectListItem() {Text = "- none -", Value = "0"},
                new SelectListItem() {Text = "I ", Value = "0"},
                new SelectListItem() {Text = "II", Value = "1"},
                new SelectListItem() {Text = "III", Value = "2"},
                new SelectListItem() {Text = "Jr.", Value = "3"},
                new SelectListItem() {Text = "Sr.", Value = "4"},
            }; }
        }

		public static IEnumerable<SelectListItem> Based
		{
			get
			{
				return new List<SelectListItem>(){
				new SelectListItem(){Text = "service based business", Value= "Service"},
				new SelectListItem(){Text = "product based business", Value= "Product"}
			}; }
		}
    }
}
