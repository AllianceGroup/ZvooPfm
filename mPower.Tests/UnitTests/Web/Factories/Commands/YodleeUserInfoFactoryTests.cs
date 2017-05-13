using Default.Factories;
using Moq;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Yodlee.YodleeUser.Commands;
using mPower.Framework;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    [TestFixture]
    public class YodleeUserInfoFactoryTests
    {
        private YodleeUserInfoFactory factory;
        private Mock<UserDocumentService> userDocumentService;
        private Mock<ICommandService> commandService;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            var mongoRead = new Mock<MongoRead>("mongodb://admin(admin):1@localhost:27020/mpower_read");
            userDocumentService = new Mock<UserDocumentService>(mongoRead.Object);
            commandService = new Mock<ICommandService>();

        }

        [SetUp]
        public void TestSetup()
        {
            userDocumentService.Setup(x => x.GetById("1")).Returns(new UserDocument());
            userDocumentService.Setup(x => x.GetById("2")).Returns(new UserDocument()
                                                                       {YodleeUserInfo = new YodleeUserInfoDocument()});
            factory = new YodleeUserInfoFactory(userDocumentService.Object, commandService.Object);
        }

        [Test]
        public void WhenYodleeUserInfoMissing_SendRegisterWithYodleeCommand()
        {
            factory.Load("1");
            commandService.Verify(x => x.Send(It.IsAny<YodleeUser_RegisterCommand>()));

        }

        [Test]
        public void WhenYodleeUserInfoExists_NoCommandSent()
        {
            factory.Load("2");
            commandService.Verify(x => x.Send(It.IsAny<YodleeUser_RegisterCommand>()), Times.Never());


        }
    }
}
