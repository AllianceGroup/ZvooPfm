using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using Default.Areas.Api.Models;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_create_empty : ApiTest
    {
        public override void PrepareReadModel()
        {
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            var result = ExecuteApiAction(() =>
            {
                controller.ModelState.AddModelError("UserId", "Required");
                return controller.CreateUser(new CreateUserModel());
            });

            Assert.AreEqual(ApiResponseStatusEnum.Error, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.ModelStateErrors, result.error_code);
            Assert.IsNotEmpty(result.log);
        }
    }
}
