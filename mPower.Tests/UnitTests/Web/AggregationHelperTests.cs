using Default.Controllers;
using Moq;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Membership;

namespace mPower.Tests.UnitTests.Web
{
    [TestFixture]
    class AggregrationHelperTests
    {
        private Mock<UserDocumentService> userDocumentService;


        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            userDocumentService = new Mock<UserDocumentService>();
            userDocumentService.Setup(x => x.GetById(It.IsAny<string>())).Returns(new UserDocument());
        }

        [Test]
        public void if_previously_authenticated_to_content_service_redirect_to_reauthenticationform()
        {
            var baseController = new BaseController();
            //var helper = new AggregationHelper();

            //helper.AuthenticationForm(baseController, 1, "1");

        }


        [Test]
        public void if_not_previously_authenticated_to_content_service_show_authenticationform()
        {
            var baseController = new BaseController();
            //var helper = new AggregationHelper();

            //helper.AuthenticationForm(baseController, 1, "1");

        }


        [Test]
        public void if_authenticationForm_throws_redirect_to_search_institution()
        {
            

        }


        [Test]
        public void if_authenticationForm_throws_illegalargumentvalueexceptionfaultmessage_return_canned_response()
        {


        }

        [Test]
        public void if_authenticationForm_throws_but_its_not_an_illegalargumentvalueexceptionfaultmessage_return_exception_message()
        {


        }
    }
}
