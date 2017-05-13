using System;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_invalid_login_test : ApiTest
    {
        private const string userName = "anorsich";
        private const string password = "asd123";

        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument() { UserName = userName, Password = SecurityUtil.GetMD5Hash(password), Id = Guid.NewGuid().ToString() });
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() =>
            {
                return controller.LogIn(userName, "Some other password");
            });

            Assert.AreEqual(ApiResponseStatusEnum.Error, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.UserNotFound, result.error_code);
        }
    }
}
