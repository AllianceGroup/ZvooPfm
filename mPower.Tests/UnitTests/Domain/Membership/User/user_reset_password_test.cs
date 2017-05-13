using System;
using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_reset_password_test : UserTest
    {
        private const string passwordHash = "asd123";

        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_ResetPasswordCommand()
            {
                UserId = _id,
                ChangeDate = _currentDate,
                PasswordHash = passwordHash
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_PasswordResettedEvent()
            {
                UserId = _id,
                ChangeDate = _currentDate,
                NewPassword = passwordHash
            };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.Password, passwordHash);
                Assert.AreEqual(user.ResetPasswordToken, String.Empty);
            });
        }
    }
}
