using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class IntuitInstitutionLuceneDocument
    {
        public string Id { get; set; }
        public string HomeUrl { get; set; }
        public string IntuitId { get; set; }
        public string Name { get; set; }
    }

    public class IntutitInstitutionLuceneService : LuceneIndexService<IntuitInstitutionLuceneDocument>
    {
        public const string IndexName = "IntuitInstitutions";

        public IntutitInstitutionLuceneService(MPowerSettings settings)
            : base(settings)
        {
            SetIndexName(IndexName);
        }

        public List<IntuitInstitutionLuceneDocument> SearchByQuery(string searchKey, int take = 200)
        {
            if (string.IsNullOrEmpty(searchKey))
                return new List<IntuitInstitutionLuceneDocument>();

            searchKey = QueryParser.Escape(searchKey);
            searchKey = searchKey.ToLower();

            var words = SplitSearchKey(searchKey);

            var query = JoinQueriesOr(
                BuildFuzzyQueryAnd("Name", words),
                BuildPrefixQueryAnd("Name", words),
                BuildFuzzyQueryAnd("HomeUrl", words));

            return Search(query, null, new Sort(new SortField("ContentServiceDisplayName", SortField.STRING)), new PagingInfo { Take = take }, take).ToList();
        }

        public void UpdateOrInsert(params IntuitInstitutionLuceneDocument[] items)
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

        protected override Document MapToLucene(IntuitInstitutionLuceneDocument item)
        {
            var doc = new Document();
            doc.Add(new Field("_id", item.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (item.HomeUrl != null)
                doc.Add(new Field("HomeUrl", item.HomeUrl, Field.Store.YES, Field.Index.ANALYZED));
            if (item.Name != null)
                doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));

            //doc.Add(new Field("Type", item.Type.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("IntuitId", item.IntuitId, Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }

        protected override IntuitInstitutionLuceneDocument MapFromLucene(Document doc)
        {
            var item = new IntuitInstitutionLuceneDocument();
            item.Id = doc.Get("_id");
            item.HomeUrl = doc.Get("HomeUrl");
            item.Name = doc.Get("Name");
            item.IntuitId = doc.Get("IntuitId");
            //item.Type = (ContentServiceTypesEnum)Enum.Parse(typeof(ContentServiceTypesEnum), doc.Get("Type"));

            return item;
        }
    }
}
