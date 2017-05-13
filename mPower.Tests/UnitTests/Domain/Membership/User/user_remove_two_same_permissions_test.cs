using System.Collections.Generic;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_remove_two_same_permissions_test : UserTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
            yield return new User_PermissionAddedEvent() { Permission = UserPermissionEnum.ViewPfm, UserId = _id };
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_RemovePermissionCommand()
            {
                Permission = UserPermissionEnum.ViewPfm,
                UserId = _id
            };

            yield return new User_RemovePermissionCommand()
            {
                Permission = UserPermissionEnum.ViewPfm,
                UserId = _id
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_PermissionRemovedEvent() { Permission = UserPermissionEnum.ViewPfm, UserId = _id };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.Permissions.Count, 0);
            });
        }
    }
}
