using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_change_password : ApiTest
    {
        private const string userName = "anorsich";
        private const string password = "asd123";
        private const string userId = "1";


        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument() { UserName = userName, Password = SecurityUtil.GetMD5Hash(password), Id = userId });
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() =>
            {
                return controller.ChangePassword(userId, password, "new pass");
            });

            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
        }
    }
}
