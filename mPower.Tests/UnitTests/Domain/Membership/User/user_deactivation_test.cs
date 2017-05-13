using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_deactivation_test : UserTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_DeactivateCommand()
            {
                UserId = _id
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_DeactivatedEvent()
            {
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
                Assert.AreEqual(user.IsActive, false);
            });
        }
    }
}
