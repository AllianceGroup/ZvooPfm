using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.IifHelpers.Documents
{
    public class IifAccount
    {
        [BsonId]
        public string Id { get; set; }

        public string ExistingAccountId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AccountTypeEnum TypeEnum { get; set; }

        public AccountLabelEnum LabelEnum { get; set; }
    }
}
