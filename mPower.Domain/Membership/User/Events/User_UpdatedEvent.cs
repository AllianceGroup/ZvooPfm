using System;
using MongoDB.Bson.Serialization.Attributes;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Events
{
    [BsonIgnoreExtraElements]
    public class User_UpdatedEvent : Event
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ZipCode { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderEnum? Gender { get; set; }
    }
}
