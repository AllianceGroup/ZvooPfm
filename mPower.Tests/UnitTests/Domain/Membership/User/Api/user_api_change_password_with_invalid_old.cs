using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_change_password_with_invalid_old : ApiTest
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
                return controller.ChangePassword(userId, "wrong_old_pass", "new pass");
            });

            Assert.AreEqual(ApiResponseStatusEnum.Error, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.UserNotFound, result.error_code);
        }
    }
}
