using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Default.Areas.Api;
using Default.Areas.Api.Controllers;
using Default.Areas.Api.Models;
using Moq;
using NUnit.Framework;
using mPower.Framework.Environment.MultiTenancy;

namespace mPower.Tests.UnitTests.Domain.Membership.User.Api
{
    public class user_api_create_valid : ApiTest
    {
        public override void PrepareReadModel()
        {
        }

        [Test]
        public override void Test()
        {
            #region Configure tenant & Request

            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.HttpMethod).Returns("POST");
            request.Setup(r => r.Url).Returns(new Uri("http://localhost:8080/"));
            request.Setup(r => r.ApplicationPath).Returns("");
            request.Setup(r => r.Cookies).Returns(new HttpCookieCollection());

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request).Returns(request.Object);
            var controllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), new Mock<ControllerBase>().Object);

            var tenants = new List<IApplicationTenant>();
            var tenantMock = new Mock<IApplicationTenant>();
            tenantMock.Setup(t => t.UrlPaths).Returns(new List<string>() { "http://localhost:8080/" });
            tenants.Add(tenantMock.Object);

            //// resolves the tenant based on the url from the request
            var tenantSelector = new TenantSelector(tenants);

            //Set Tenant Selector to a Static Class
            TenantTools.Selector = tenantSelector;
            TenantTools.Selector.TenantsContainer = _container;

            #endregion

            var controller = _container.GetInstance<MembershipController>();
            controller.ControllerContext = controllerContext;

            var wr = new SimpleWorkerRequest("/dummy", @"c:\inetpub\wwwroot\dummy", "dummy.html", null, new StringWriter());
            HttpContext.Current = new HttpContext(wr);
            SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, new Mock<IHttpSessionState>().Object);

            var result = ExecuteApiAction(() =>
            {
                return controller.CreateUser(new CreateUserModel()
                {
                    Email = "an.orsich@gmail.com",
                    FirstName = "Andrew",
                    LastName = "Orsich",
                    Password = "asd123!@",
                    UserName = "anorsich"
                });
            });

            Assert.AreEqual(ApiResponseStatusEnum.Success, result.status);
            Assert.AreEqual((int)MembershipApiErrorCodesEnum.None, result.error_code);
        }
    }
}
