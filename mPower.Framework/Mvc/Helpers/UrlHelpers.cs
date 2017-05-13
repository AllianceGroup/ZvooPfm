using System.IO;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class UrlHelpers
    {
        private const string OffersContentDir = "/Uploads/OffersContent";

        public static string OfferFilePath(this UrlHelper helper, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }
            try
            {
                return helper.Content(Path.Combine(OffersContentDir, filename));
            }
            catch
            {
                return null;
            }
        }
    }
}