﻿using System;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Data
{
    public class UserData
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationId { get; set; }
        public string ZipCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderEnum? Gender { get; set; }
        public string ReferralCode { get; set; }

        public ICommandMetadata Metadata { get; set; }
        public bool IsAgent { get; set; }
        public bool IsCreatedByAgent { get; set; }
        public string CreatedBy { get; set; }
    }
}