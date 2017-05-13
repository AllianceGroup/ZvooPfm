using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using mPower.Framework.Mvc.Validation;

namespace Default.ViewModel.Areas.Credit.Verification
{
    public class QuestionaireAnswer
    {
        [Required]
        public string QuestionId { get; set; }

		[Required(ErrorMessage = "Missing Answer")]
        public List<string> Answers { get; set; }
    }
}
