using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace mPower.Framework.Environment.Sitemap
{
    public class TenantSiteMap
    {
        private TenantSiteMap()
        { }

        public TenantSiteMapNode RootNode { get; private set; }

        public string PathToSiteMapFile { get; private set; }

        public TenantSiteMapNode CurrentNode
        {
            get { return FindSiteMapNode(HttpContext.Current); }
        }

        public static TenantSiteMap LoadFromFile(string fileName)
        {
            var item = new TenantSiteMap();

            var doc = XDocument.Load(fileName);
            var root = Map(doc.Root.FirstNode as XElement);
            item.RootNode = root;
            item.PathToSiteMapFile = fileName;
            return item;
        }

        public TenantSiteMapNode FindSiteMapNode(HttpContext context, TenantSiteMapNode node = null)
        {
            if (context == null)
                return null;

            string rawUrl = context.Request.RawUrl.ToLower();

            TenantSiteMapNode result = null;

            // First check the RawUrl
            result = FindSiteMapNode(rawUrl);

            // try remove querystring
            if (result == null)
            {
                int queryStringIndex = rawUrl.IndexOf("?", StringComparison.Ordinal);
                if (queryStringIndex != -1)
                {
                    rawUrl = rawUrl.Substring(0, queryStringIndex);
                }
                result = FindSiteMapNode(rawUrl);
            
            }
            //try trim 3 slashes to find result
            if (result == null)
            {
                var url = rawUrl;
                for (int i = 3; i > 0; i--)
                {
                    url = PerpareUrl(url);
                    result = FindSiteMapNode(url);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        private string PerpareUrl(string url)
        {
            if (url.Count(x => x == '/') > 1) //leave only one slash
            {
                var lastSlash = url.LastIndexOf("/");
                url = url.Substring(0, lastSlash);
            }

            return url;
        }

        private TenantSiteMapNode FindSiteMapNode(string url, TenantSiteMapNode node = null)
        {
            var nodeToSearch = node ?? RootNode;
            return FindSiteMapNode(url, new[] { nodeToSearch }); // include parent in search
        }

        private TenantSiteMapNode FindSiteMapNode(string url, params TenantSiteMapNode[] nodes)
        {
            TenantSiteMapNode result = null;
            foreach (var node in nodes)
            {
                result = FindSiteMapNode(url, node.Nodes.ToArray()) ?? (String.Compare(url, node.Url, true) == 0 ? node : null);
                if (result != null)
                    break;
            }

            return result;
        }

        private static TenantSiteMapNode Map(XNode node)
        {
            var element = node as XElement;
            var item = new TenantSiteMapNode();
            item.Description = GetLoweredAttributeValue(element, "description");
            item.Title = GetLoweredAttributeValue(element, "title");
            var url = GetLoweredAttributeValue(element, "url");
            if (url != null)
            {
                url = url.TrimStart('~');
                //we need keep base urls like ~/
                if (url.Length > 1)
                    url = url.TrimEnd('/');
            }

            item.Url = url;
            item.ResourceKey = GetLoweredAttributeValue(element, "resourceKey");
            item.IsAccessDenied = GetLoweredAttributeValue(element, "access") == "denied";
            item.ShowSubmenu = GetLoweredAttributeValue(element, "showSubmenu") == "true";
            item.EnableTenantProperty = GetAttributeValue(element, "enableTenantProperty");

            var childs = element.Nodes().ToList();
            if (childs.Count > 0)
            {
                item.Nodes = new List<TenantSiteMapNode>();
                foreach (var xElement in childs)
                {
                    if (xElement != null)
                        item.Nodes.Add(Map(xElement));
                }
            }

            return item;
        }

        private static string GetLoweredAttributeValue(XElement element, string name)
        {
            return element.Attribute(name) != null ? element.Attribute(name).Value.ToLower() : null;
        }

        private static string GetAttributeValue(XElement element, string name)
        {
            return element.Attribute(name) != null ? element.Attribute(name).Value : null;
        }
    }

    public class TenantSiteMapNode
    {
        public TenantSiteMapNode()
        {
            Nodes = new List<TenantSiteMapNode>();
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string ResourceKey { get; set; }

        public bool IsAccessDenied { get; set; }

        public List<TenantSiteMapNode> Nodes { get; set; }

        public bool ShowSubmenu { get; set; }

        public string EnableTenantProperty { get; set; }
    }
}
