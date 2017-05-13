using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Moq;

namespace mPower.Tests.Environment
{
    public static class Extentions
    {
        public static void MapBaseUrl(this Mock<HttpRequestBase> mock, string uri)
        {
            mock.Setup(x => x.Url).Returns(new Uri(uri));
            mock.Setup(x => x.ApplicationPath).Returns("");
        }
    }
}