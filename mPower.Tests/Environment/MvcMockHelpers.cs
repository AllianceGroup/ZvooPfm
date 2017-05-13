﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace mPower.Tests.Environment
{
   
    public static class MvcMockHelpers
    {
        public static HttpContextBase FakeHttpContext()
        {

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

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

        public static HttpContextBase FakeHttpContext(string url)
        {
            HttpContextBase context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);            
            return context;
        }


        #region Controller Context
        public static void SetFakeControllerContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }

        public static void SetFakeControllerContext(this Controller controller, RouteData routeData)
        {
            SetFakeControllerContext(controller, new Dictionary<string, string>(), new HttpCookieCollection(), routeData);
        }

        public static void SetFakeControllerContext(this Controller controller, HttpCookieCollection requestCookies)
        {
            SetFakeControllerContext(controller, new Dictionary<string, string>(), requestCookies, new RouteData());
        }

        public static void SetFakeControllerContext(this Controller controller, Dictionary<string, string> formValues)
        {
            SetFakeControllerContext(controller, formValues, new HttpCookieCollection(), new RouteData());
        }

        public static void SetFakeControllerContext(this Controller controller,
            Dictionary<string, string> formValues,
            HttpCookieCollection requestCookies,
            RouteData routeData)
        {
            var httpContext = FakeHttpContext();

            foreach (string key in formValues.Keys)
            {
                httpContext.Request.Form.Add(key, formValues[key]);

            }
            foreach (string key in requestCookies.Keys)
            {
                httpContext.Request.Cookies.Add(requestCookies[key]);

            }
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, routeData), controller);
            controller.ControllerContext = context;
        }

        public static void SetFakeControllerContextWithLogin(this Controller controller, string userName,
            string password,
            string returnUrl)
        {

            var httpContext = FakeHttpContext();


            httpContext.Request.Form.Add("username", userName);
            httpContext.Request.Form.Add("password", password);
            httpContext.Request.QueryString.Add("ReturnUrl", returnUrl);

            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }

        #endregion


        static string GetUrlFileName(string url)
        {
            if (url.Contains("?"))
                return url.Substring(0, url.IndexOf("?"));
            else
                return url;
        }

        static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                NameValueCollection parameters = new NameValueCollection();

                string[] parts = url.Split("?".ToCharArray());
                string[] keys = parts[1].Split("&".ToCharArray());

                foreach (string key in keys)
                {
                    string[] part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            else
            {
                return null;
            }
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));
            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
        }
    }
}
