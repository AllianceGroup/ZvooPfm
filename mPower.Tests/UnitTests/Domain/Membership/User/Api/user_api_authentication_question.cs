using System;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using NUnit.Framework;
using Newtonsoft.Json;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_authentication_question : ApiTest
    {
        private readonly string userId = Guid.NewGuid().ToString();
        private const string userName = "anorsich";
        private const string password = "asd123";
        private const string question = "What is my first pet nick?";
        private const string answer = "Cezar";

        public override void PrepareReadModel()
        {
            _usersService.Insert(new UserDocument {UserName = userName, Password = SecurityUtil.GetMD5Hash(password), Id = userId, PasswordQuestion = question, PasswordAnswer = answer});
        }

        [Test]
        public override void Test()
        {
            var controller = _container.GetInstance<MembershipController>();

            // can't test this functionality cause write-database is needed
            //var result = ExecuteApiAction(() => controller.AddAuthenticationQuestion(userId, question, answer));
            //Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            //Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);

            var result = ExecuteApiAction(() => controller.GetAuthenticationQuestion(userId));
            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
            Assert.AreEqual(question, JsonConvert.DeserializeObject(result.data, typeof(string)));

            result = ExecuteApiAction(() => controller.ValidateAuthenticationQuestion(userId, answer));
            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
            Assert.AreEqual("true", result.data);

            result = ExecuteApiAction(() => controller.ValidateAuthenticationQuestion(userId, Guid.NewGuid().ToString()));
            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
            Assert.AreEqual("false", result.data);
        }
    }
}
