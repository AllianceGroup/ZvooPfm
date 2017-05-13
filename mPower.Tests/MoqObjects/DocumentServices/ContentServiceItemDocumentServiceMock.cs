using System;
using Moq;
using mPower.Domain.Yodlee.Storage.Documents;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;
using mPower.Framework.Mongo;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class ContentServiceItemDocumentServiceMock : IMock<ContentServiceItemDocumentService>
    {
        private readonly MockFactory _mockFactory;

        private Mock<ContentServiceItemDocumentService> _current;

        public ContentServiceItemDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;

            _current = Create();
        }


        public Mock<ContentServiceItemDocumentService> Create()
        {
            var mongoMock = _mockFactory.Create<MongoYodleeMock>();

            return new Mock<ContentServiceItemDocumentService>(mongoMock.Object);
        }

        public ContentServiceItemDocumentService Object
        {
            get { return _current.Object; }
        }

        public ContentServiceItemDocumentServiceMock AddGetByIdReturnNull()
        {
            ContentServiceItemDocument item = null;

            _current.Setup(x => x.GetById(It.IsAny<string>())).Returns(item);

            return this;
        }
    }
}
