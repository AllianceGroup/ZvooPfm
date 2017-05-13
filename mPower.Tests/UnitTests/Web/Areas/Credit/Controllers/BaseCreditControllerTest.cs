using Default;
using Moq;
using TransUnionWrapper;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.DocumentServices.Membership;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Security;

namespace mPower.Tests.UnitTests.Web.Areas.Credit.Controllers
{

  
    public class BaseCreditControllersTest
    {
        public Mock<CreditIdentityDocumentService> _creditIdentityDocumentService;
        public Mock<IEncryptionService> _encryptionService;
        public Mock<ISessionContext> _sessionContext;
        public Mock<IIdGenerator> _idGenerator;
        public Mock<ITransUnionService> _transUnionService;
        public Mock<UserDocumentService> _userDocumentService;
        public Mock<AffiliateDocumentService> _affiliateDocumentService;
        public Mock<IObjectRepository> _objectRepository;
        public Mock<ICommandService> _commandService; 
            
        public virtual void TestSetup()
        {
            var mongoRead = new MongoRead("mongodb://localhost:27020");
            
            _creditIdentityDocumentService =
                new Mock<CreditIdentityDocumentService>(mongoRead);
            _affiliateDocumentService = new Mock<AffiliateDocumentService>(mongoRead);
            _userDocumentService = new Mock<UserDocumentService>(mongoRead);

            _encryptionService = new Mock<IEncryptionService>();
            _transUnionService = new Mock<ITransUnionService>();
            _commandService = new Mock<ICommandService>();
            
            _sessionContext = new Mock<ISessionContext>();
            _idGenerator = new Mock<IIdGenerator>();
            
            _objectRepository = new Mock<IObjectRepository>();
        }
    }
}
