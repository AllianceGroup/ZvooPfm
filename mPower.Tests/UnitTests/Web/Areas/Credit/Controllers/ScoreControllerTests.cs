using System.Collections.Generic;
using System.Web.Mvc;
using Default.Areas.Credit.Controllers;
using Moq;
using NUnit.Framework;
using mPower.Documents.Documents.Credit.CreditIdentity;

namespace mPower.Tests.UnitTests.Web.Areas.Credit.Controllers
{
    
    [TestFixture]
    public class ScoreControllersTests : BaseCreditControllersTest
    {
        ScoreController controller;
        
       
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {


        }

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();

            controller = new ScoreController( _creditIdentityDocumentService.Object,
                _encryptionService.Object,
                _sessionContext.Object, 
                _transUnionService.Object,
                _objectRepository.Object);
        }

        [Test]
        public void when_session_client_key_missing_set_client_key_with_first_creditIdentity()
        {
            _sessionContext.Setup(x => x.UserId).Returns("1");

            _creditIdentityDocumentService.Setup(x =>
                x.GetCreditIdentitiesByUserId(It.IsAny<string>())
                ).Returns(new List<CreditIdentityDocument>(){new CreditIdentityDocument()});
            
            controller.Dashboard();

            _sessionContext.Verify(x => x.ClientKey, Times.Once());

        }
      
        [Test]
        public void dashboard_redirects_to_verification_controller_when_no_credit_identity_present()
        {
            _sessionContext.Setup(x => x.UserId).Returns("1");

            _creditIdentityDocumentService.Setup(x =>
                x.GetCreditIdentitiesByUserId(It.IsAny<string>())
                ).Returns(new List<CreditIdentityDocument>());


            var result = controller.Dashboard() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("CreditIdentity", result.RouteValues["controller"]);
        }

        [Test]
        public void when_credit_identity_present_view_is_returned()
        {
            _sessionContext.Setup(x => x.UserId).Returns("1");

            _creditIdentityDocumentService.Setup(x =>
               x.GetCreditIdentitiesByUserId(It.IsAny<string>())
               ).Returns(new List<CreditIdentityDocument>() { new CreditIdentityDocument() { FirstName = "Test" } });


            _sessionContext.Setup(x => x.UserId).Returns("1");

            var result = controller.Dashboard() as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
