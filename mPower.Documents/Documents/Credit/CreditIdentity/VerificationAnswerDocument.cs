using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class VerificationAnswerDocument
    {
        [BsonId]
        public string Id { get; set; }

        public bool IsCorrect { get; set; }
        public string Answer { get; set; }
        public int SequenceNumber { get; set; }
    }
}
