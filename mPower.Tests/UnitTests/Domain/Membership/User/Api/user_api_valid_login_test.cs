using System;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_valid_login_test : ApiTest
    {
        private const string userName = "anorsich";
        private const string password = "asd123";

        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument() { UserName = userName, Password = SecurityUtil.GetMD5Hash(password), Id = Guid.NewGuid().ToString() });
        }

        [Test]
        [Ignore]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() =>
            {
                return controller.LogIn(userName, password);
            });

            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
        }
    }
}
