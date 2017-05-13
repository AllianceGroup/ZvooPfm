using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DataAnnotationsExtensions;

namespace Default.ViewModel.Areas.Business.StartController
{
    public class AddLedgerViewModel
    {
        [Required(ErrorMessage="Business Name is Required")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "Name must have between 1 and 40 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Business Address is Required")]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 256 characters")]
		[RegularExpression(@"[a-zA-Z0-9/@' \.,#\-\\:;]+", ErrorMessage = "Invalid Address")]
        public string Address { get; set; }

		[StringLength(256, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 256 characters")]
		[RegularExpression(@"[a-zA-Z0-9/@' \.,#\-\\:;]+", ErrorMessage = "Invalid Address")]
        public string Address2 { get; set; }

		[Required(ErrorMessage = "City is Required")]
		[StringLength(80, MinimumLength = 1, ErrorMessage = "City must be between 1 and 80 characters")]
		[RegularExpression(@"[0-9 ]*[a-zA-Z]+[a-zA-Z0-9 '\.\-,]+", ErrorMessage = "Invalid Address")]
        public string City { get; set; }

		[Required(ErrorMessage = "State is Required")]
        public string State { get; set; }

		[Required(ErrorMessage="Zip Code is Required")]
		[Digits(ErrorMessage="Numbers only")]
        public string Zip { get; set; }

		[RegularExpression(@"[0-9]+", ErrorMessage = "Numbers only. No spaces or dashes")]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "EIN's must be 9 digits, with no spaces or dashes")]
		public string EIN { get; set; }

        [Required(ErrorMessage = "Year Business Started is Required")]
        [Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        [Min(1900)]
        public int YearStarted { get; set; }

		public string AutoOrManual { get; set; }

		public string Based { get; set; }

		[Required(ErrorMessage = "Starting Fiscal Month isRequired")]
		[Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public int StartFiscalMonth { get; set; }

		[Required(ErrorMessage = "Starting Fiscal Day is Required")]
		[Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public int StartFiscalDay { get; set; }

        public AddLedgerViewModel()
        {
            StartFiscalDay = 1;
            StartFiscalMonth = 1;
            YearStarted = DateTime.Now.Year;
        }

        public DateTime FiscalYearStart 
        {
            get { return new DateTime(DateTime.Now.Year, StartFiscalMonth, StartFiscalDay, 0, 0, 0, DateTimeKind.Utc); }
        }
    }
}