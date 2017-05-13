using System;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateCommand : Command
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
