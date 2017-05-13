using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using mPower.Framework.Utils;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_update_reset_password_token_test : UserTest
    {
        private string _token;

        public user_update_reset_password_token_test()
        {
            _token = SecurityUtil.GetUniqueToken();
        }

        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_UpdateResetPasswordTokenCommand()
            {
                UserId = _id,
                Token = _token
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_UpdatedResetPasswordTokenEvent()
            {
                UserId = _id,
                UniqueToken = _token
            };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.ResetPasswordToken, _token);
            });
        }
    }
}
