using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Segments
{
    public class UserSegment
    {
        public UserSegment()
        {
            AggregateData = new UserSegmentAggregateData();
        }

        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// used just for filtering by it
        /// </summary>
        public string FormattedDate { get; set; }

        [BsonIgnoreIfNull]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? Date { get; set; }

        public string AffiliateId { get; set; }

        public string AffiliateName { get; set; }

        public UserSegmentTypeEnum UserSegmentType { get; set; }

        public string ZipCode { get; set; }

        public UserSegmentAggregateData AggregateData { get; set; }

        // was missed
        public DateTime LastLoginDate { get; set; }
    }
}