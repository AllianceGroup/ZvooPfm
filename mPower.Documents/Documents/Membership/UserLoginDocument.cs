using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Membership
{
    public class UserLoginDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public DateTime LoginDate { get; set; }

        public string AffiliateName { get; set; }

        public string AffiliateId { get; set; }

        public bool IsFromMobile { get; set; }
    }
}
