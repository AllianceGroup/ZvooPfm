using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Default.Areas.Administration.Models;
using NUnit.Framework;
using Prelude.Extensions.FlashMessage;
using Prelude.IntegrationTesting.Browsing;
using Prelude.IntegrationTesting.Hosting;


namespace mPower.Tests.UnitTests.Web.Controllers.Integrations
{
    [TestFixture]
    [Ignore]
    public class AggregationControllerTests
    {
        private AppHost appHost;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            appHost = AppHost.Simulate("mPower.Web");
            AddUser();
        }

        [TestFixtureTearDown]
        public void TestFixtureTeadDown()
        {
            DeleteUser();
        }

        [Test]
        public void requires_authentication()
        {
            appHost.Start(browsingSession =>
                              {
                                  var result = browsingSession.Get("/Aggregation/SearchInstitutions");
                                  Assert.AreEqual(302, result.Response.StatusCode);
                              });
        }

        [Test]
        public void get_search_institutions()
        {
            appHost.Start(browsingSession =>
                              {
                                  Login(browsingSession);
                                  var result = browsingSession.Get("/Aggregation/SearchInstitutions");
                                  var viewResult = (ViewResult)result.ActionExecutedContext.Result;

                                  Assert.NotNull(viewResult);
                                  Assert.AreEqual(200, result.Response.StatusCode);

                              });

        }

        [Test]
        public void post_search_intitutions()
        {
            appHost.Start(browsingSession =>
                              {
                                  Login(browsingSession);

                                  var result = browsingSession.Post("/Aggregation/SearchInstitutions",
                                                       new NameValueCollection() { { "searchText", "Wells Fargo" } },
                                                       new NameValueCollection() { { "Accept", "text/html" }, { "X-Requested-With", "XMLHttpRequest" } });

                                  var viewResult = (PartialViewResult)result.ActionExecutedContext.Result;

                                  Assert.NotNull(viewResult);
                                  Assert.AreEqual(200, result.Response.StatusCode);
                              });
        }

        [Test]
        public void get_authenticate_to_institution()
        {
            appHost.Start(browsingSession =>
                              {
                                  Login(browsingSession);
                                  var result = browsingSession
                                      .Get("/Aggregation/AuthenticateToInstitution?id=100000", new NameValueCollection() { { "Accept", "text/html" }, { "X-Requested-With", "XMLHttpRequest" } });

                                  var viewResult = (PartialViewResult)result.ActionExecutedContext.Result;

                                  Assert.NotNull(viewResult);
                                  Assert.AreEqual(200, result.Response.StatusCode);

                              });
        }

        [Test]
        [Ignore]
        public void post_authenticate_to_institution_with_bad_credentials_shows_error_message()
        {
            appHost.Start(browsingSession =>
            {
                Login(browsingSession);


                var formRequest =  browsingSession
                                      .Get("/Aggregation/AuthenticateToInstitution?id=100000", new NameValueCollection() { { "Accept", "text/html" }, { "X-Requested-With", "XMLHttpRequest" } });


                var result = browsingSession
                    .Post("/Aggregation/AuthenticateToInstitution",
                    new NameValueCollection
                        {
                            {"contentServiceId", "100000"},
                            {"keys[0].Name", "Banking UserId"},
                            {"keys[0].InstitutionKeyId", "100000001"},
                            {"keys[0].Value", "bad"},
                            {"keys[1].Name", "Banking Password"},
                            {"keys[1].InstitutionKeyId", "100000002"},
                            {"keys[1].Value", "any"}
                        },
                    new NameValueCollection()
                        {
                            { "Accept", "text/html" }, 
                            { "X-Requested-With", "XMLHttpRequest" }
                        });

                var viewResult = (WrappedActionResultWithFlash<RedirectToRouteResult>)result.ActionExecutedContext.Result;

                Assert.NotNull(viewResult);
                Assert.AreEqual(302, result.Response.StatusCode);
            });
        }

        #region Private

        public void Login(BrowsingSession browsingSession, string email = "temp@user.com", string password = "Password")
        {
            var result = browsingSession.Post("/Authentication/Login", new NameValueCollection() { { "Email", email }, { "Password", password } });
        }

        private void AddUser()
        {
            appHost.Start(browsingSession =>
            {
                LoginAsAdmin(browsingSession);
                var result = browsingSession.Post("/Administration/Affiliate/AddUser", new NameValueCollection()
                {
                    {"UserName",
                "temp@user.com"},
                    { "FirstName", "temp" },
                    { "LastName", "user" },
                    {"Email", 
                "temp@user.com"},
                    {"NewPassword",
                "Password"},
                {"ConfirmNewPassword",
                "Password"},
                });

                var viewResult = (ViewResult)result.ActionExecutedContext.Result;
            });
        }

        private void LoginAsAdmin(BrowsingSession browsingSession)
        {
            Login(browsingSession, "brett@mpowering.com", "test123");
        }

        private void DeleteUser()
        {
            appHost.Start(browsingSession =>
                              {
                                  LoginAsAdmin(browsingSession);
                                  var result = browsingSession.Post("/Administration/Affiliate/AddUser", new NameValueCollection()
                                                                                                {
                                                                                                    {"Email",
                                                                                                "temp@user.com"},
                                                                                                });
                              });
        }

        #endregion

    }
}
