using System.Collections.Generic;
using System.Web.Mvc;
using Default.Areas.Credit.Controllers;
using Default.ViewModel.Areas.Credit.Verification;
using Moq;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using NUnit.Framework;
using Paralect.Domain;
using TransUnionWrapper.Model;

namespace mPower.Tests.UnitTests.Web.Areas.Credit.Controllers
{
    [TestFixture]
    public class CreditIdentityControllerTests : BaseCreditControllersTest
    {
        private CreditIdentityController controller;

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            
            controller = new CreditIdentityController(_idGenerator.Object,
                                                      _creditIdentityDocumentService.Object,
                                                      _transUnionService.Object,
                                                      _userDocumentService.Object,
                                                      _affiliateDocumentService.Object,
                                                      _objectRepository.Object,
                                                      _commandService.Object,
                                                      _sessionContext.Object,
                                                      null
                                                     );
        }


        #region New()
        [Test]
        public void when_user_doesnt_have_any_credit_identities_show_new_identity_form()
        {
            _creditIdentityDocumentService.Setup(x => x.GetCreditIdentitiesByUserId(It.IsAny<string>())).Returns(
                new List<CreditIdentityDocument>());
            
            var result = controller.New() as ViewResult;

            Assert.IsNotNull(result);

        }

        #endregion

        #region Create()

        [Test]
        public void when_credit_identity_contains_invalid_data_returns_new_credit_identity_form_and_no_commands_sent()
        {
            controller.ModelState.AddModelError("Error", "Error");
            var result = controller.Create(new IdentityViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            _commandService.Verify(x => x.Send(It.IsAny<ICommand>()), Times.Never());
        }
        

        [Test]
        [Ignore]
        public void when_model_state_valid_send_credit_identity_create_command()
        {
            _objectRepository.Setup(x => x.Load<IdentityViewModel, CreditIdentity_CreateCommand>(It.IsAny<IdentityViewModel>())).Returns(new CreditIdentity_CreateCommand());

           
            _objectRepository.Setup(
                x => x.Load<CreditIdentity_CreateCommand, CreditIdentity>(It.IsAny<CreditIdentity_CreateCommand>())).
                Returns(new CreditIdentity(){});

            _transUnionService.Setup(x => x.SubscribeToAlerts(It.IsAny<CreditIdentity>())).Returns("1");
            
            var result = controller.Create(new IdentityViewModel()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            _commandService.Verify(x => x.Send(It.IsAny<ICommand>()), Times.Once());

        }
        #endregion

        #region Questions()

        [Test]
        public void when_no_credit_identity_id_in_session_context_redirect_to_new()
        {
            var result = controller.Questionaire() as RedirectToRouteResult;
            Assert.IsNotNull(result);
        }

        #endregion

    }
}
