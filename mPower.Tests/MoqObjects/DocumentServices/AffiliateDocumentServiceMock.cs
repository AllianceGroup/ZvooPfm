using Moq;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class AffiliateDocumentServiceMock : IMock<AffiliateDocumentService>
    {
        private readonly MockFactory _mockFactory;
        private readonly Mock<AffiliateDocumentService> _current;

        public AffiliateDocumentService Object
        {
            get { return _current.Object; }
        }

        public AffiliateDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public Mock<AffiliateDocumentService> Create()
        {
            var mongoread = _mockFactory.Create<MongoReadMock>();

            return new Mock<AffiliateDocumentService>(mongoread.Object);
        }

        public AffiliateDocumentServiceMock AddGetById()
        {
            _current.Setup(x => x.GetById("1")).Returns(new AffiliateDocument());

            return this;
        }
    }
}