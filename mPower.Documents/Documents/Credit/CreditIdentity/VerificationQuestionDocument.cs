using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class VerificationQuestionDocument
    {
        [BsonId]
        public string Id { get; set; }
        
        public List<VerificationAnswerDocument> Answers { get; set; }
        public bool IsFakeQuestion { get; set; }
        public bool IsLastChanceQuestion { get; set; }
        public string QuestionType { get; set; }
        public int SequenceNumber { get; set; }
        public string Question { get; set; }
		public bool HasMultipleAnswers { get; set; }
    }
}
