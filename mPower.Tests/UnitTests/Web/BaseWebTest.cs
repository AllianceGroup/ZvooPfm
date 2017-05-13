using System;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects;
using NUnit.Framework;
using StructureMap;

namespace mPower.Tests.UnitTests.Web
{
    [TestFixture]
    public class BaseWebTest
    {
        protected static DateTime CurrentDate = new DateTime(2011, 11, 11, 10, 10, 0, 0, DateTimeKind.Utc);

        protected IContainer _container;

        protected MockFactory MockFactory { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.BootstrapStructureMap();
            bootstrapper.BootstrapWebTests();
            _container = bootstrapper.Container;
            
            MockFactory = _container.GetInstance<MockFactory>();
        }
    }
}
