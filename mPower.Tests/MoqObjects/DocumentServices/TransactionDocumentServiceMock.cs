using Moq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class TransactionDocumentServiceMock : IMock<TransactionDocumentService>
    {
        private readonly MockFactory _mockFactory;

        private Mock<TransactionDocumentService> _current;

        public TransactionDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public Mock<TransactionDocumentService> Create()
        {
            var mongoRead = _mockFactory.Create<MongoReadMock>().Object;
            var mock = new Mock<TransactionDocumentService>(mongoRead);
            return mock;
        }

        public TransactionDocumentService Object
        {
            get { return _current.Object; }
        }
    }
}
