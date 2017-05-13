using System;
using System.Linq;
using Moq;
using mPower.Framework.Environment.MultiTenancy;
using NUnit.Framework;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Framework.MultiTenancy
{
    public class DefaultTenantSelectorTests
    {
        [Test]
        public void DefaultTenantSelector_Ctr_SetsControllers()
        { 
            var tenants = Enumerable.Empty<IApplicationTenant>();
            var selector = new TenantSelector(tenants);
            Assert.AreSame(tenants, selector.Tenants);
        }

        [Test]
        public void DefaultTenantSelector_Ctr_ThrowsException_GivenNullTenants()
        {
            Assert.Throws<ArgumentNullException>(() => new TenantSelector(null));
        }

        [Test]
        public void DefaultTenantSelector_Select_ThrowsException_GivenNullRequestContext()
        {
            Assert.Throws<ArgumentNullException>(() => new TenantSelector(Enumerable.Empty<IApplicationTenant>()).Select(null));
        }

        [Test]
        public void DefaultTenantSelector_Select_ReturnsTenantWithSpecifiedBasePath()
        {
            var url = "http://wwww.eagleenvision.net";

            var expected = GenerateTenant(url);
            var tenants = new[] { GenerateTenant(),
                                  GenerateTenant("http://www.google.com", "http://www.yahoo.com"), 
                                  expected };
            var selector = new TenantSelector(tenants);
            Assert.AreSame(expected, selector.Select(Fake.Request(url)));
        }
        
        

        private IApplicationTenant GenerateTenant(params string[] paths)
        {
            var mock = new Mock<IApplicationTenant>();
            mock.SetupGet(x => x.UrlPaths).Returns(paths);
            return mock.Object;
        }
    }
}
