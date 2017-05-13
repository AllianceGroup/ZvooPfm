using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Default.Models;
using mPower.Domain.Accounting.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Model
{
    public class AddEventModel
    {
        public string CalendarId { get; set; }

        public string Description { get; set; }

        public CalendarEventTypeEnum Type { get; set; }

        [Required(ErrorMessage = "Date is required field")]
        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }

        public CalendarEventFrequencyEnum Frequency { get; set; }

        public int Repeat { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        public IEnumerable<DayOfWeek> Days { get; set; }

        public IEnumerable<SelectListItem> AvailibleDays { get; set; }

        public DayAsPartOfEnum DayAsPartOf { get; set; }

        public int? EndAfter { get; set; }

        [DataType(DataType.DateTime)]

        public DateTime? EndDate { get; set; }

        public SendAlertOptionModel SendAlertOptions { get; set; }

        public IEnumerable<SelectListItem> Calendars { get; set; }

        public IEnumerable<SelectListItem> Frequencies { get; set; }

        public EndEventRepeatingEnum EndEventRepeating { get; set; }

        public IEnumerable<SelectListItem> RepeatList { get; set; }

        public string UserId { get; set; }

        public AddEventModel()
        {
            SendAlertOptions = new SendAlertOptionModel();
            Days = new List<DayOfWeek>();
            Frequency = CalendarEventFrequencyEnum.Weekly;
            Frequencies = new SelectList(Enum.GetValues(typeof(CalendarEventFrequencyEnum)));
            AvailibleDays = new SelectList(Enum.GetValues(typeof (DayOfWeek)));
            RepeatList = new SelectList(Enumerable.Range(1,30));
        }
    }

    public enum CalendarEventTypeEnum
    {
        Onetime,
        Repeating
    }

    public enum EndEventRepeatingEnum
    {
        Never,
        After,
        On
    }
}
