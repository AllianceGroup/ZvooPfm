using System;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using Default.Areas.Api.Models;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_create_with_existsing_name : ApiTest
    {
        const string userName = "anorsich";

        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument() { UserName = userName, Id = Guid.NewGuid().ToString() });
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() =>
            {
                return controller.CreateUser(new CreateUserModel()
                {
                    UserName = userName
                });
            });

            Assert.AreEqual(ApiResponseStatusEnum.Error, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.UserNameExists, result.error_code);
        }
    }
}
