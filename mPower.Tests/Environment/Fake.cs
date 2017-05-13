using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace mPower.Tests.Environment
{
    public class Fake
    {

        public static RequestContext Request(string url)
        {
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            var mockHttpCookie = new HttpCookieCollection();
            mockHttpRequest.MapBaseUrl(url);
            mockHttpRequest.Setup(x => x.Cookies).Returns(mockHttpCookie);
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);

            return new RequestContext { HttpContext = mockHttpContext.Object };
        }


     
        public static HttpContextBase HttpContext(string url = null)
        {
            if(String.IsNullOrEmpty(url))
                url = "http://localhost:8080";

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var mockHttpCookie = new HttpCookieCollection();

            request.MapBaseUrl(url);
            request.Setup(x => x.Cookies).Returns(mockHttpCookie);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);


            var form = new NameValueCollection();
            var querystring = new NameValueCollection();
            var cookies = new HttpCookieCollection();
            var user = new GenericPrincipal(new GenericIdentity("testuser"), new string[] { "Administrator" });

            request.Setup(r => r.Cookies).Returns(cookies);
            request.Setup(r => r.Form).Returns(form);
            request.Setup(q => q.QueryString).Returns(querystring);

            response.Setup(r => r.Cookies).Returns(cookies);

            context.Setup(u => u.User).Returns(user);



            return context.Object;
        }

        public static ControllerContext  ControllerContext(string url)
        {
            return new ControllerContext(HttpContext(url), new RouteData(), new Mock<ControllerBase>().Object);
        }
    }
}
