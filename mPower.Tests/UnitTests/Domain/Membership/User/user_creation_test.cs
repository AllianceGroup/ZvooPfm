using System.Collections.Generic;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_creation_test : UserTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield break;
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return User_Create();
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return User_Created();
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.IsNotNull(user, "User wasn't saved into read database");
                Assert.AreEqual(user.FirstName, _firstName);
                Assert.AreEqual(user.Email, _email);
            });
        }
    }
}
