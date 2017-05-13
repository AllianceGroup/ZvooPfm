using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_change_password_test : UserTest
    {
        private const string password = "asd123!";

        public override IEnumerable<IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new User_ChangePasswordCommand()
            {
                PasswordHash = password,
                ChangeDate = _currentDate,
                UserId = _id
            };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new User_PasswordChangedEvent()
                             {
                                 ChangeDate = _currentDate,
                                 NewPassword = password,
                                 UserId = _id
                             };
        }
        
        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.Password, password);
                Assert.AreEqual(user.LastPasswordChangedDate.Day, _currentDate.Day);
                Assert.AreEqual(user.LastPasswordChangedDate.Minute, _currentDate.Minute);
                Assert.AreEqual(user.LastPasswordChangedDate.Second, _currentDate.Second);
                Assert.AreEqual(user.ResetPasswordToken, null);
            });
        }
    }
}
