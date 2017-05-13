using System.Collections.Generic;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    /// <summary>
    /// We are sending two add permission commands with same permission, 
    /// but our domain model skip permission if it exists (that's mean that read model also skip same permission)
    /// </summary>
    public class user_add_two_same_permissions_test : UserTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_AddPermissionCommand()
            {
                Permission = UserPermissionEnum.ViewPfm,
                UserId = _id
            };

            yield return new User_AddPermissionCommand()
            {
                Permission = UserPermissionEnum.ViewPfm,
                UserId = _id
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_PermissionAddedEvent() { Permission = UserPermissionEnum.ViewPfm, UserId = _id };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.Permissions.Count, 1);
                Assert.AreEqual(user.Permissions[0], UserPermissionEnum.ViewPfm);
            });
        }
    }
}
