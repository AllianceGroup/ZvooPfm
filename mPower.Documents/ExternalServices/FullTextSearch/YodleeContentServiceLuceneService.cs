using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using Lucene.Net.Search;
using mPower.Framework.Services;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class ContentServiceLuceneDocument
    {
        public string Id { get; set; }
        public string ContentServiceDisplayName { get; set; }
        public string SiteDisplayName { get; set; }
        public string HomeUrl { get; set; }
        public string OrganizationDisplayName { get; set; }
        public ContentServiceTypesEnum Type { get; set; }
    }

    public class YodleeContentServiceLuceneService : LuceneIndexService<ContentServiceLuceneDocument>
    {
        public YodleeContentServiceLuceneService(MPowerSettings settings)
            : base(settings)
        {
            SetIndexName("YodleeContentServices");
        }

        public List<ContentServiceLuceneDocument> SearchByQuery(string searchKey, int take = 200)
        {
            if (string.IsNullOrEmpty(searchKey))
                return new List<ContentServiceLuceneDocument>();

            searchKey = QueryParser.Escape(searchKey);
            searchKey = searchKey.ToLower();

            var words = SplitSearchKey(searchKey);

            var query = JoinQueriesOr(
                BuildFuzzyQueryAnd("ContentServiceDisplayName", words),
                BuildPrefixQueryAnd("ContentServiceDisplayName", words),
                BuildFuzzyQueryAnd("SiteDisplayName", words),
                BuildFuzzyQueryAnd("OrganizationDisplayName", words),
                BuildFuzzyQuery("HomeUrl", searchKey),
                BuildFuzzyQuery("Type", searchKey));

            return Search(query, null, new Sort(new SortField("ContentServiceDisplayName", SortField.STRING)), new PagingInfo() { Take = take }, take).ToList();
        }

        public void UpdateOrInsert(params ContentServiceLuceneDocument[] items)
        {
            foreach (var item in items)
            {
                    var doc = GetById("_id", item.Id);
                    if (doc == null)
                        Insert(item);
                    else
                        Update(item);
            }
        }

        protected override Document MapToLucene(ContentServiceLuceneDocument item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.ContentServiceDisplayName != null)
                doc.Add(new Field("ContentServiceDisplayName", item.ContentServiceDisplayName, Field.Store.YES, Field.Index.ANALYZED));
            if (item.SiteDisplayName != null)
                doc.Add(new Field("SiteDisplayName", item.SiteDisplayName, Field.Store.YES, Field.Index.ANALYZED));
            if (item.HomeUrl != null)
                doc.Add(new Field("HomeUrl", item.HomeUrl, Field.Store.YES, Field.Index.ANALYZED));
            if (item.OrganizationDisplayName != null)
                doc.Add(new Field("OrganizationDisplayName", item.OrganizationDisplayName, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Type", item.Type.ToString(), Field.Store.YES, Field.Index.ANALYZED));

            return doc;
        }

        protected override ContentServiceLuceneDocument MapFromLucene(Document doc)
        {
            var item = new ContentServiceLuceneDocument();
            item.Id = doc.Get("_id");
            item.ContentServiceDisplayName = doc.Get("ContentServiceDisplayName");
            item.SiteDisplayName = doc.Get("SiteDisplayName");
            item.HomeUrl = doc.Get("HomeUrl");
            item.OrganizationDisplayName = doc.Get("OrganizationDisplayName");
            item.Type = (ContentServiceTypesEnum)Enum.Parse(typeof(ContentServiceTypesEnum), doc.Get("Type"));

            return item;
        }
    }
}
