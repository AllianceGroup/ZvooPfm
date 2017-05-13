using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Framework;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices.Lucene
{
    public interface IOfferLuceneService
    {
        void SetFlushAfter(int i);
        void Insert(OfferDocument[] batch);
    }

    public class OfferLuceneService : LuceneIndexService<OfferLuceneIndex>, IOfferLuceneService
    {
        public OfferLuceneService(MPowerSettings settings) : base(settings)
        {
            SetIndexName("Offers");
            NameOfIdField = "_id";
        }

        public IEnumerable<OfferLuceneIndex> GetByMerchants(params string[] merchants)
        {
            var queries = new List<Query>();
            foreach (var s in merchants)
            {
                var name = s.ToLower();
                queries.Add(BuildPrefixQuery("Merchant", name));
            }
            
            return Search(JoinQueriesOr(queries.ToArray()));
        }

        protected override Document MapToLucene(OfferLuceneIndex item)
        {
            return item.ToLuceneDocument();
        }

        protected override OfferLuceneIndex MapFromLucene(Document doc)
        {
           return new OfferLuceneIndex(doc);
        }

        public void Insert(OfferDocument[] documents)
        {
            Insert(documents.Select(x=> new OfferLuceneIndex(x.Id,x.Title,x.Merchant)).ToArray());
        }
    }

    public class OfferLuceneIndex
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Merchant { get; set; }

        public OfferLuceneIndex()
        {
            
        }

        public OfferLuceneIndex(Document doc)
        {
            Id = doc.Get("_id");
            Title = doc.Get("Title");
            Merchant = doc.Get("Merchant");
        }

        public OfferLuceneIndex(string id, string title, string merchant)
        {
            Id = id;
            Title = title;
            Merchant = merchant;
        }

        public Document ToLuceneDocument()
        {
            var doc = new Document();
            doc.Add(new Field("_id", Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (Title.HasValue())
            {
                doc.Add(new Field("Title", Title, Field.Store.YES, Field.Index.ANALYZED));
            }
            if (Merchant.HasValue())
            {
                doc.Add(new Field("Merchant", Merchant, Field.Store.YES, Field.Index.ANALYZED));
            }
            return doc;
        }
    }
}