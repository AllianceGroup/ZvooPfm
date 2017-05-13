using System;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using Newtonsoft.Json;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_get_by_username : ApiTest
    {
        private const string userName = "anorsich";
        private const string password = "asd123";

        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument { UserName = userName, Password = SecurityUtil.GetMD5Hash(password), Id = Guid.NewGuid().ToString() });
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() => controller.GetUserByUsername(userName));

            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);

            var user = (UserDocument)JsonConvert.DeserializeObject(result.data, typeof(UserDocument));

            Assert.AreEqual(userName, user.UserName);
        }
    }
}
