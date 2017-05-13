using Moq;
using mPower.Framework.Environment;
using mPower.Tests.Environment;

namespace mPower.Tests.MoqObjects.Common
{
    public class IIdGeneratorMock : IMock<IIdGenerator>
    {
        private Mock<IIdGenerator> _current;

        public IIdGeneratorMock()
        {
            _current = Create();
        }

        public Mock<IIdGenerator> Create()
        {
            return new Mock<IIdGenerator>();
        }

        public IIdGenerator Object
        {
            get { return _current.Object; }
        }

        public IIdGeneratorMock MockGenerateId(string id)
        {
            _current.Setup(x => x.Generate()).Returns(id);

            return this;
        }
    }
}
