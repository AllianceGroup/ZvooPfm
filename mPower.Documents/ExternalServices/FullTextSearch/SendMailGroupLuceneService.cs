using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class SendMailGroupLuceneDocument
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> UserIds { get; set; }

        public string AffiliateId { get; set; }
    }

    public class SendMailGroupLuceneService : LuceneIndexService<SendMailGroupLuceneDocument>
    {
        public SendMailGroupLuceneService(MPowerSettings settings)
            : base(settings)
        {
            base.SetIndexName("SendMailGroups");
        }

        public List<SendMailGroupLuceneDocument> SearchByQuery(string key, string affiliateId = null, int take = 15)
        {
            string[] words = null;
            if (key != null)
                words = key.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var filterQuery = new BooleanQuery();

            if (!String.IsNullOrEmpty(affiliateId))
            {
                filterQuery.Add(BuildMatchQuery("AffiliateId", affiliateId), BooleanClause.Occur.MUST);
            }

            Filter queryFilter = new QueryWrapperFilter(filterQuery);

            return Search(String.IsNullOrEmpty(key) ? new MatchAllDocsQuery() : BuildPrefixQueryAnd("Name", words),
                 queryFilter, new Sort(), new PagingInfo() { Take = 15 }, take).ToList();
        }

        public void AddSegment(string segmentId, string name, List<string> ids, string affiliateId)
        {
            name = String.Format("Segment: {0}", name);

            Insert(new SendMailGroupLuceneDocument() { Id = segmentId, Name = name, UserIds = ids.ToList(), AffiliateId = affiliateId });
        }

        public void UpdateSegment(string segmentId, string name, List<string> ids, string affiliateId)
        {
            name = String.Format("Segment: {0}", name);

            Update(new SendMailGroupLuceneDocument() { Id = segmentId, Name = name, UserIds = ids.ToList(), AffiliateId = affiliateId });
        }

        public void AddUser(string userId, string email, string firstName, string lastName, string affiliateId)
        {
            var name = String.Format("{0} {1}: {2}", firstName, lastName, email);

            Insert(new SendMailGroupLuceneDocument()
            {
                Id = userId,
                Name = name,
                UserIds = new List<string>() { userId },
                AffiliateId = affiliateId
            });
        }

        public void UpdateUser(string userId, string email, string firstName, string lastName, string affiliateId)
        {
            var name = String.Format("{0} {1}: {2}", firstName, lastName, email);

            Update(new SendMailGroupLuceneDocument()
            {
                Id = userId,
                Name = name,
                UserIds = new List<string>() { userId },
                AffiliateId = affiliateId
            });
        }

        public void DeleteDocuments(params string[] ids)
        {
            Delete(ids);
        }

        public SendMailGroupLuceneDocument GetById(string id)
        {
            return GetById("_id", id);
        }

        protected override Document MapToLucene(SendMailGroupLuceneDocument item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
            if (!String.IsNullOrEmpty(item.AffiliateId))
                doc.Add(new Field("AffiliateId", item.AffiliateId, Field.Store.YES, Field.Index.ANALYZED));

            doc.Add(new Field("UserIds", String.Join(",", item.UserIds), Field.Store.YES, Field.Index.NO));

            return doc;
        }

        protected override SendMailGroupLuceneDocument MapFromLucene(Document doc)
        {
            var result = new SendMailGroupLuceneDocument();

            result.Id = doc.Get("_id");
            result.Name = doc.Get("Name");
            result.AffiliateId = doc.Get("AffiliateId");

            result.UserIds = doc.Get("UserIds").Split(new[] { "," }, StringSplitOptions.None).ToList();

            return result;
        }
    }
}
