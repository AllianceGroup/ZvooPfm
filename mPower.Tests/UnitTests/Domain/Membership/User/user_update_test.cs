using System;
using System.Collections.Generic;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_update_test : UserTest
    {
        private const string email = "updated@gmail.com";
        private const string firstName = "FirstName";
        private const string lastName = "LastName";
        private const string zipCode = "12345";
        private readonly DateTime? birthDate = new DateTime(2012,1,1);
        private readonly GenderEnum? gender = GenderEnum.Male;

        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_UpdateCommand()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserId = _id,
                ZipCode = zipCode,
                BirthDate = birthDate,
                Gender = gender,
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_UpdatedEvent()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserId = _id,
                ZipCode = zipCode,
                BirthDate = birthDate,
                Gender = gender,
            };
        }

        [Test]
        public void Test()
        {
            Validate();
            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.Email, email);
                Assert.AreEqual(user.FirstName, firstName);
                Assert.AreEqual(user.LastName, lastName);
                Assert.AreEqual(user.ZipCode, zipCode);
                Assert.AreEqual(user.BirthDate, birthDate);
                Assert.AreEqual(user.Gender, gender);
            });
        }
    }
}
