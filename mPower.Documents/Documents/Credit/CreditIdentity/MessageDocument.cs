using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class MessageDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string CodeDescription { get; set; }
        public string CodeSymbol { get; set; }
        public string Text { get; set; }
        public string TypeDescription { get; set; }
        public string TypeSymbol { get; set; }
        public int Rank { get; set; }  
    }
}
