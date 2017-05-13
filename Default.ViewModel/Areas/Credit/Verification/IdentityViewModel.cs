using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using DataAnnotationsExtensions;

namespace Default.ViewModel.Areas.Credit.Verification
{
    public class IdentityViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage="First Name Required")]
		[RegularExpression("[a-zA-Z '\\-\\.]+", ErrorMessage="Invalid entry, letters only")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "Name must have between 1 and 40 characters")]
		public string FirstName { get; set; }

		[RegularExpression("[a-zA-Z '\\-\\.]+", ErrorMessage = "Invalid entry, letters only")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "Name must have between 1 and 40 characters")]
		public string MiddleName { get; set; }

		[RegularExpression("[a-zA-Z '\\-\\.]+", ErrorMessage = "Invalid entry, letters only")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "Name must have between 1 and 40 characters")]
        [Required(ErrorMessage="Last Name is Required")]
        public string LastName { get; set; }

		[Required(ErrorMessage="Birth Day is Required")]
		[Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public int BirthDay { get; set; }

		[Required(ErrorMessage="Birth Month is Required")]
		[Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public int BirthMonth{ get; set; }

		[Required(ErrorMessage="Birth Year is Required")]
		[Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public int BirthYear { get; set; }
        
		[Required(ErrorMessage="Address is Required")]
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 256 characters")]
		[RegularExpression(@"[a-zA-Z0-9/@' \.,#\-\\:;]+", ErrorMessage="Invalid Address")]
        public string Address { get; set; }

		[StringLength(256, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 256 characters")]
		[RegularExpression(@"[a-zA-Z0-9/@' \.,#\-\\:;]+", ErrorMessage="Invalid Address")]
        public string Address2 { get; set; }
        
		[Required(ErrorMessage="City is Required")]
		[StringLength(80, MinimumLength = 1, ErrorMessage = "City must be between 1 and 80 characters")]
		[RegularExpression(@"[0-9 ]*[a-zA-Z]+[a-zA-Z0-9 '\.\-,]+")]
        public string City { get; set; }
        
		[Required(ErrorMessage="State is Required")]
		public string State { get; set; }
        
		[Required(ErrorMessage="Postal Code is Required")]
        [Digits(ErrorMessage = "Numbers only. No spaces or dashes.")]
        public string PostalCode { get; set; }
        
		[Required]
        public string Suffix { get; set; }

        [Required(ErrorMessage="Social Security Number is Required")]
		[RegularExpression("[0-9]+", ErrorMessage="Numbers only. No spaces or dashes")]
        public string SocialSecurityNumber { get; set; }


        public string CreditIdentityId { get; set; }

        public DateTime DateOfBirth { get { return new DateTime(BirthYear, BirthMonth, BirthDay); } }
        
    }
}
