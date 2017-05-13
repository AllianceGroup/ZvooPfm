using System;
using System.Web.Routing;
using Moq;
using mPower.Framework.Environment.MultiTenancy;
using NUnit.Framework;
using StructureMap;

namespace mPower.Tests.UnitTests.Framework.MultiTenancy
{
    public class TenantContainerResolverTests
    {
        [Test]
        public void TenantContainerResolver_ThrowsException_GivenNullTenantSelector()
        {
            Assert.Throws<ArgumentNullException>(() => new ContainerResolver(null));
        }

        [Test]
        public void TenantContainerResolver_Resolve_ReturnsContainerFromSelectedTenant()
        {
            var container = new Container();
            var tenant = new Mock<IApplicationTenant>();
            tenant.Setup(x=>x.DependencyContainer).Returns(container);
            var tenantSelector = new Mock<ITenantSelector>();
            tenantSelector.Setup(x => x.Select(It.IsAny<RequestContext>())).Returns(tenant.Object);
            var resolver = new ContainerResolver(tenantSelector.Object);
            Assert.AreSame(container, resolver.Resolve(new RequestContext()));
        }
    }
}
