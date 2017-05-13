using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using mPower.Environment.MultiTenancy;
using mPower.Framework.Environment.MultiTenancy;
using NUnit.Framework;
using StructureMap;
using mPower.Framework.Exceptions;

namespace mPower.Tests.UnitTests.Framework.MultiTenancy
{
    public class ContainerControllerFactoryTests
    {
        [Test]
        public void ContainerControllerFactory_Ctr_ThrowsException_GivenNullResolver()
        {
            Assert.Throws<ArgumentNullException>(() => new ContainerControllerFactory(null));
        }

        [Test]
        public void ContainerControllerFactory_CreateController_ReturnsNull_WhenGetTypeReturnsNull()
        {
            var factory = new TestContainerControllerFactory(new Mock<IContainerResolver>().Object);
            factory.GetControllerTypeDelegate = () => null;
            Assert.Throws<MpowerNotFoundException>(() => factory.CreateController(null, ""));
        }

        [Test]
        public void ContainerControllerFactory_CreateController_ReturnsExpectedTypeFromContainer_GivenContainerAndProperType()
        {
            var container = new Container();
            var controller = new ProperController();
            container.Inject<ProperController>(controller);
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            factory.GetControllerTypeDelegate = () => typeof(ProperController);

            Assert.AreSame(controller, factory.CreateController(null, ""));
        }

        [Test]
        public void ContainerControllerFactory_CreateController_ReturnsControllerWithActionInvoker_WhenActionInvokerIsNotContainerActionInvoker()
        {
            var container = new Container();
            var controller = new ProperController();
            container.Inject<ProperController>(controller);
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            factory.GetControllerTypeDelegate = () => typeof(ProperController);

            //Assert.IsType<ContainerControllerActionInvoker>((factory.CreateController(null, "") as ProperController).ActionInvoker);
        }

        [Test]
        public void ContainerControllerFactory_CreateController_DoesntSetActionInvoker_WhenOfCorrectType()
        {
            var container = new Container();
            var containerResolver = new ContainerResolver(container);
            var controller = new ProperController();
            var exceptionFilter = new Mock<IExceptionFilter>();
            var actionInvoker = new ContainerControllerActionInvoker(containerResolver, exceptionFilter.Object);

            controller.ActionInvoker = actionInvoker;
            container.Inject<ProperController>(controller);
            var factory = new TestContainerControllerFactory(containerResolver);
            factory.GetControllerTypeDelegate = () => typeof(ProperController);

            Assert.AreSame(actionInvoker, (factory.CreateController(null, "") as ProperController).ActionInvoker);
        }

        [Test]
        public void ContainercontrollerFactory_GetTypesFor_ReturnsEmpty_WhenNoControllersInContainer()
        {
            var container = new Container();
            var types = ContainerControllerFactory.GetControllersFor(container);
            Assert.IsTrue(types.Count() == 0);
        }

        [Test]
        public void ContainerControllerFactory_GetTypesFor_ReturnsAllControllerTypesInContainer()
        {
            Func<IEnumerable<Type>, string> typeToString = _types => _types.OrderBy(x => x.Name).Select(x => x.Name).ConcatAll(",");

            var container = new Container();
            var mockController = new Mock<IController>().Object;
            container.Inject<IController>(new ProperController());
            container.Inject<IController>(mockController);

            var types = ContainerControllerFactory.GetControllersFor(container);

            Assert.AreEqual(typeToString(new[] { typeof(ProperController), mockController.GetType() }),
                         typeToString(types));
        }

        [Test]
        [Ignore]
        public void ContainerControllerFactory_GetControllerType_ReturnsNull_WhenContainerDoesntContainController()
        {
            var container = new Container();
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            Assert.Null(factory.GetControllerType_(new Mock<RequestContext>().Object, "Proper"));
        }

        [Test]
        [Ignore]
        public void ContainerControllerFactory_GetControllerType_ReturnsType_WhenContainerContainsControllerWithType()
        {
            var container = new Container();
            container.Inject<IController>(new ProperController());
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            Assert.AreEqual(typeof(ProperController), factory.GetControllerType_(new Mock<RequestContext>().Object, "Proper"));
        }

        [Test]
        [Ignore]
        public void ContainerControllerFactory_GetControllerType_ReturnsType_WhenContainerContainsController_SpecifiedControllerContainsControllerAppendage()
        {
            var container = new Container();
            container.Inject<IController>(new ProperController());
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            Assert.AreEqual(typeof(ProperController), factory.GetControllerType_(new Mock<RequestContext>().Object, "ProperController"));
        }

        [Test]
        [Ignore]
        public void ContainerControllerFactory_GetControllerType_ReturnsType_WhenContainerContainsController_SpecifiedControllerIsInRandomCase()
        {
            var container = new Container();
            container.Inject<IController>(new ProperController());
            var factory = new TestContainerControllerFactory(new ContainerResolver(container));
            Assert.AreEqual(typeof(ProperController), factory.GetControllerType_(new Mock<RequestContext>().Object, "PrOpEr"));
        }

        [Test]
        public void ContainerControllerFactory_ReleaseController_DoesntThrowException_GivenNullController()
        {
            Assert.DoesNotThrow(() => new ContainerControllerFactory(new ContainerResolver(new Container())).ReleaseController(null));
        }

        [Test]
        public void ContainerControllerFactory_ReleaseController_DoesntThrowException_GivenNonDisposableController()
        {
            Assert.DoesNotThrow(() => new ContainerControllerFactory(new ContainerResolver(new Container())).ReleaseController(new ProperController()));
        }

        [Test]
        public void ContainerControllerFactory_ReleaseController_CallsDispose_GivenDisposableController()
        {
            var controller = new DisposableController();
            new ContainerControllerFactory(new ContainerResolver(new Container())).ReleaseController(controller);
            Assert.True(controller.Disposed);
        }

        private class DisposableController : IController, IDisposable
        {
            public DisposableController()
            {
                Disposed = false;
            }

            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public void Execute(RequestContext requestContext)
            {
                throw new NotImplementedException();
            }
        }

        private class ContainerResolver : IContainerResolver
        {
            public ContainerResolver(IContainer container)
            {
                this.container = container;
            }

            private IContainer container;

            public IContainer Resolve(RequestContext context)
            {
                return container;
            }
        }

        public class TestContainerControllerFactory : ContainerControllerFactory
        {
            public TestContainerControllerFactory(IContainerResolver resolver)
                : base(resolver)
            {

            }

            protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
            {
                return GetControllerType_(requestContext, controllerName);
            }

            public Func<Type> GetControllerTypeDelegate { get; set; }

            public virtual Type GetControllerType_(System.Web.Routing.RequestContext requestContext, string controllerName)
            {
                if (GetControllerTypeDelegate != null)
                    return GetControllerTypeDelegate();
                return base.GetControllerType(requestContext, controllerName);
            }

            public override System.Web.Mvc.IController CreateController(RequestContext requestContext, string controllerName)
            {
                return base.CreateController(requestContext, controllerName);
            }
        }
    }
}
