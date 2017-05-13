using System;

namespace mPower.Framework.Utils.Extensions
{
    public static class UriExtensions
    {
        public static string GetDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
            {
                string host = uri.Host;
                if (host.Split('.').Length > 2)
                {
                    int firstIndex = host.IndexOf('.');
                    return host.Substring(firstIndex + 1);
                }
                return host;
            }
            return null;
        }

        public static string BaseUrl(this Uri uri)
        {
            return (uri.Scheme + "://" + uri.Authority + '/').Trim();
        }
        
    }
}