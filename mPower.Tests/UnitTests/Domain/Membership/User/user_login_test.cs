using System;
using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using mPower.Framework.Utils;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_login_test : UserTest
    {
        private DateTime _loginDate = DateTime.Now;
        private string _authToken = SecurityUtil.GetUniqueToken();

        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_LogInCommand()
            {
                UserId = _id,
                LogInDate = _loginDate,
                AuthToken = _authToken
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_LoggedInEvent()
            {
                UserId = _id,
                Date = _loginDate,
                AuthToken = _authToken
            };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.LastLoginDate.Day, _loginDate.Day);
                Assert.AreEqual(user.LastLoginDate.Minute, _loginDate.Minute);
                Assert.AreEqual(user.LastLoginDate.Second, _loginDate.Second);
                Assert.AreEqual(user.AuthToken, _authToken);

            });
        }
    }
}
