using System;
using System.Web;
using System.Web.Mvc;
using Moq;
using mPower.Framework.Environment.MultiTenancy;
using NUnit.Framework;
using StructureMap;

namespace mPower.Tests.UnitTests.Framework.MultiTenancy
{
    public class ExecutionFilterAttributeTests
    {
        [Test]
        public void ExecutionFilterAttribute_OnActionExecuting_CallsOnInValid_WhenValidateReturnsFalse()
        {
            bool called = false;
            var attr = new TestAttribute();
            attr.Invalid = () => called = true;
            attr.Validation = () => false;

            attr.OnActionExecuting(FakeContext);

            Assert.True(called, "OnInvalid was not called upon invalid validate");
        }

        [Test]
        public void ExecutionFilterAttribute_OnActionExecuting_DoesntCallOnInValid_WhenValidateReturnsTrue()
        {
            bool called = true;
            var attr = new TestAttribute();
            attr.Invalid = () => called = false;
            attr.Validation = () => true;

            attr.OnActionExecuting(FakeContext);
            
            Assert.True(called, "OnInvalid was called upon sucessful validation");
        }

        private ActionExecutingContext FakeContext
        {
            get
            {
                return new ActionExecutingContext
                {
                    HttpContext = new Mock<HttpContextBase>().Object
                };
            }
        }

        public class TestAttribute : ExecutionFilterAttribute
        {
            public Func<bool> Validation { get; set; }
            public Action Invalid { get; set; }

            public override bool Validate(IContainer dependencyContainer, ActionExecutingContext httpContext)
            {
                return Validation();
            }

            protected override void OnInvalid(System.Web.Mvc.ActionExecutingContext filterContext)
            {
                Invalid();
            }
        }
    }
}
