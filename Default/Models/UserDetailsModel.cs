using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using mPower.Domain.Application.Enums;
using Validation = Default.Constants.Validation;

namespace Default.Models
{
    public class UserDetailsModel
    {
        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required, DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(Validation.Regexps.Email, ErrorMessage = Validation.Messages.InvalidEmail)]
        public string Email { get; set; }

        public string ZipCode { get; set; }

        public string BirthDate { get; set; }

        public GenderEnum? Gender { get; set; }

        public IDictionary<int, string> Genders =>
            Enum.GetValues(typeof (GenderEnum)).Cast<GenderEnum>().ToDictionary(t => (int) t, t => t.ToString());
    }
}