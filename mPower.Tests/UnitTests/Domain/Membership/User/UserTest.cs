using System;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.User;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public abstract class UserTest : AggregateTest<UserAR>
    {
        protected UserTest()
        {
            _currentDate = DateTime.Now;
        }

        protected string _email = "an.orsich@gmail.com";
        protected string _firstName = "Andrew";
        protected string _lastName = "Orsich";
        protected string _password = "asd123";
        protected string _userName = "anorsich";
        protected string _zipCode = "12345";
        protected DateTime _birthDate = DateTime.Now;
        protected GenderEnum? _gender = GenderEnum.Male;
        protected DateTime _currentDate;
        protected const string LedgerId = "test-ledger-id";
        protected const string AccountId = "test-account-id";

        public IEvent User_Created()
        {
            return new User_CreatedEvent
            {
                UserId = _id,
                Email = _email,
                FirstName = _firstName,
                LastName = _lastName,
                Password = _password,
                UserName = _userName,
                CreateDate = _currentDate,
                IsActive = true,
                ZipCode = _zipCode,
                BirthDate = _birthDate,
                Gender = _gender,
            };
        }


        public ICommand User_Create()
        {
            return new User_CreateCommand()
            {
                UserId = _id,
                Email = _email,
                FirstName = _firstName,
                LastName = _lastName,
                PasswordHash = _password,
                UserName = _userName,
                CreateDate = _currentDate,
                IsActive = true,
                ZipCode = _zipCode,
                BirthDate = _birthDate,
                Gender = _gender,
            };
        }

        protected UserDocumentService _userDocumentService
        {
            get
            {
                return GetInstance<UserDocumentService>();
            }
        }
    }
}
