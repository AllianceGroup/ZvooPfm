using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class VerificationQuestionData
    {
        public string Id { get; set; }
        public List<VerificationAnswerData> Answers { get; set; }
        public bool IsFakeQuestion { get; set; }
        public bool IsLastChanceQuestion { get; set; }
        public string QuestionType { get; set; }
        public int SequenceNumber { get; set; }
        public string Question { get; set; }
    }
}