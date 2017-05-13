using System;
using System.Collections.Generic;
using System.Linq;

namespace Default.ViewModel.Areas.Credit.Verification
{
    public class VerificationQuestion
    {
        public IEnumerable<VerificationAnswer> Answers { get; set; }
        public bool IsFakeQuestion { get; set; }
        public bool IsLastChanceQuestion { get; set; }
        public string QuestionType { get; set; }
        public int SequenceNumber { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }

		[Obsolete]
		public bool HasMultipleAnswers { get; set; }

		public bool MultipleAnswers {
			get {
				return this.Answers.Where(x => x.IsCorrect == true).Count() > 1;
			}
		}
    }
}