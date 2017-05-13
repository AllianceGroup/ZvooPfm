using System;
using MongoDB.Bson.Serialization.Attributes;
using Paralect.Domain;

namespace mPower.Documents.Documents
{
    public class EventLogDocument
    {
        public static EventLogDocument Create(IEvent evnt, string message)
        {
            return new EventLogDocument()
            {
                UserId = evnt.Metadata.UserId,
                Id = evnt.Metadata.EventId ?? Guid.NewGuid().ToString(),
                Data = evnt,
                EventName = evnt.Metadata.TypeName,
                StoredDate = evnt.Metadata.StoredDate,
                Message = message
            };
        }

        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public DateTime StoredDate { get; set; }

        public string EventName { get; set; }

        public string Message { get; set; }

        public IEvent Data { get; set; }

        [BsonIgnore]
        public bool IsNew { get; set; }
    }
}
