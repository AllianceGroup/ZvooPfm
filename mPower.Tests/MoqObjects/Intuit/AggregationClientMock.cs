using Moq;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Aggregation.Contract.Documents;
using mPower.Tests.Environment;
using System.Collections.Generic;

namespace mPower.Tests.MoqObjects.Intuit
{
    internal class AggregationClientMock : IMock<AggregationClient>
    {
        public const int FakeInstitutionId = 1;
        private List<KeyDocument> _fakeKeyDocuments;
        private readonly MockFactory _mockFactory;
        private readonly Mock<AggregationClient> _current;

        public AggregationClientMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public AggregationClient Object
        {
            get { return _current.Object; }
        }

        public Mock<AggregationClient> Create()
        {
            return new Mock<AggregationClient>(null, null);
        }

        public AggregationClientMock AddGetInstitutionKeys(string logonId)
        {
            _current.Setup(x => x.GetInstitutionKeys(new Metadata {LogonId = logonId}, FakeInstitutionId)).Returns(FakeKeyDocuments);
            return this;
        }

        public List<KeyDocument> FakeKeyDocuments
        {
            get
            {
                if (_fakeKeyDocuments == null)
                {
                    _fakeKeyDocuments = new List<KeyDocument>
                    {
                        new KeyDocument {DisplayFlag = true, DisplayOrder = 1},
                        new KeyDocument {DisplayFlag = true, DisplayOrder = 2},
                        new KeyDocument {DisplayFlag = false, DisplayOrder = 3},
                    };
                }
                return _fakeKeyDocuments;
            }
        }
    }
}