using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using mPower.Framework;
using mPower.Framework.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Version = Lucene.Net.Util.Version;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class CustomFilter : Filter
    {
    }

    public abstract class LuceneIndexService<T> where T : class
    {
        /// <summary>
        /// Should be okay
        /// </summary>
        private static readonly object SyncRoot = new Object();
        private readonly List<T> _insertDocuments;
        private readonly List<T> _updateDocuments;
        private readonly List<string> _idsToDelete;

        private readonly string _indexesDirectory;
        private int _flushAfter;
        private string _indexName;

        private readonly Version _version = Version.LUCENE_29;

        protected LuceneIndexService(MPowerSettings settings)
        {
            _indexesDirectory = settings/*.Local*/.LuceneIndexesDirectory;
            _flushAfter = 1;
            _insertDocuments = new List<T>();
            _updateDocuments = new List<T>();
            _idsToDelete = new List<string>();

            NameOfIdField = "_id";
        }

        public int FlushAfter
        {
            get { return _flushAfter; }
        }

        public bool IsImmediateFlush
        {
            get { return _flushAfter == 1; }
        }

        public string NameOfIdField { get; set; }

        public List<T> UpdateDocuments
        {
            get { return _updateDocuments; }
        }

        public List<T> InsertDocuments
        {
            get { return _insertDocuments; }
        }

        public void RemoveIndexFromDisc()
        {
            var indexDirectoryPath = Path.Combine(_indexesDirectory, _indexName);
            if (System.IO.Directory.Exists(_indexesDirectory))
            {
                System.IO.Directory.Delete(indexDirectoryPath, true);
                System.IO.Directory.CreateDirectory(indexDirectoryPath);
            }
        }

        public void SetFlushAfter(int count)
        {
            _flushAfter = count;
        }

        public void WillDoManualFlush()
        {
            _flushAfter = int.MaxValue;
            _willDoManulFlush = true;
        }

        public bool IsManualFlush
        {
            get { return _willDoManulFlush; }
        }

        protected void SetIndexName(string indexName)
        {
            _indexName = indexName;
            var indexDirectoryPath = Path.Combine(_indexesDirectory, _indexName);
            if (!System.IO.Directory.Exists(indexDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(indexDirectoryPath);
                Flush(true);
            }
        }

        public virtual void Insert(params T[] documents)
        {
            lock (SyncRoot)
            {
                _insertDocuments.AddRange(documents);
            }
            Flush();
        }

        public virtual void Update(params T[] documents)
        {
            lock (SyncRoot)
            {
                _updateDocuments.AddRange(documents);
            }
            Flush();
        }

        protected virtual void Delete(params string[] ids)
        {
            lock (SyncRoot)
            {
                _idsToDelete.AddRange(ids);
            }
            Flush();
        }

        public void Flush(bool flushAll = false)
        {
            lock (SyncRoot)
            {
                if (_insertDocuments.Count + _updateDocuments.Count + _idsToDelete.Count < _flushAfter && !flushAll)
                    return;

                var writer = CreateWriter();

                foreach (var document in _insertDocuments.Select(MapToLucene))
                {
                    writer.AddDocument(document);
                }

                foreach (var document in _updateDocuments.Select(MapToLucene))
                {
                    writer.UpdateDocument(new Term(NameOfIdField, document.Get(NameOfIdField)), document);
                }

                var terms = _idsToDelete.Select(x => new Term(NameOfIdField, x)).Where(x => x != null).ToArray();

                writer.DeleteDocuments(terms);

                writer.Optimize();
                writer.Commit();
                writer.Close(true);

                _insertDocuments.Clear();
                _updateDocuments.Clear();
                _idsToDelete.Clear();
            }
        }

        /// <summary>
        /// Dummy thing, if filter null i can't find way how to sort elements
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="paging"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        protected List<T> Search(Query query, Filter filter = null, Sort sort = null, PagingInfo paging = null, int maxResults = 10000000)
        {
            var items = new List<Document>();

            var searcher = CreateSearcher();
            if (filter != null)
            {
                var result = searcher.Search(query, filter, maxResults, sort);

                var itemsFound = result.totalHits;

                paging = paging ?? new PagingInfo();

                for (int i = paging.IndexOfFirstItem - 1; i <= paging.CurrentPage * paging.ItemsPerPage - 1 && i < itemsFound; i++)
                {
                    int docId = result.scoreDocs[i].doc;
                    float score = result.scoreDocs[i].score;
                    var doc = searcher.Doc(docId);

                    items.Add(doc);
                }
                paging.TotalCount = itemsFound;
            }
            else
            {

                var result = searcher.Search(query, maxResults);

                var itemsFound = result.totalHits;

                paging = paging ?? new PagingInfo();

                for (int i = paging.IndexOfFirstItem - 1; i <= paging.CurrentPage * paging.ItemsPerPage - 1 && i < itemsFound; i++)
                {
                    int docId = result.scoreDocs[i].doc;
                    float score = result.scoreDocs[i].score;
                    var doc = searcher.Doc(docId);

                    items.Add(doc);
                }
                paging.TotalCount = itemsFound;
            }

            searcher.Close();

            return items.Select(MapFromLucene).ToList();
        }

        public T GetById(string fieldName, string value)
        {
            return Search(BuildMatchQuery(fieldName, value)).FirstOrDefault();
        }

        protected abstract Document MapToLucene(T item);
        protected abstract T MapFromLucene(Document doc);



        #region private

        private IndexWriter CreateWriter()
        {
            var directory = GetIndexDirectory(_indexName);
            var analyzer = CreateAnalyzer();
            return new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Analyzer CreateAnalyzer()
        {
            return new StandardAnalyzer(_version);
        }

        private IndexSearcher CreateSearcher()
        {
            var dir = GetIndexDirectory(_indexName);

            return new IndexSearcher(dir, true);
        }

        private Lucene.Net.Store.Directory GetIndexDirectory(string indexName)
        {
            var dir = new DirectoryInfo(Path.Combine(_indexesDirectory, indexName));

            return FSDirectory.Open(dir);
        }

        protected string[] SplitSearchKey(string searchKey)
        {
            var words = searchKey.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Where(t => t.Length >= 2).ToArray();
            var result = new List<string>();
            foreach (var word in words)
            {
                if (word.ToLower() == "of")
                    continue;
                //Lucene removing all punctuation except dot that follow after whitespace (I've skipped this). We are doing same thing
                result.Add(word.Replace(".", String.Empty).Replace(",", String.Empty));
            }

            return result.ToArray();
        }

        #endregion

        #region Queries Builders

        protected Query JoinQueriesOr(params Query[] queries)
        {
            var joinedQuery = new BooleanQuery();

            foreach (var query in queries)
            {
                joinedQuery.Add(query, BooleanClause.Occur.SHOULD);
            }

            return joinedQuery;
        }

        protected Query JoinQueriesAnd(params Query[] queries)
        {
            var joinedQuery = new BooleanQuery();

            foreach (var query in queries)
            {
                joinedQuery.Add(query, BooleanClause.Occur.MUST);
            }

            return joinedQuery;
        }

        protected Query JoinQuriesNot(params Query[] queries)
        {
            var joinedQuery = new BooleanQuery();

            foreach (var query in queries)
            {
                joinedQuery.Add(query, BooleanClause.Occur.MUST_NOT);
            }

            return joinedQuery;
        }

        protected NumericRangeQuery BuildNumericRangeQuery(string field, Int64? min, Int64? max)
        {
            return NumericRangeQuery.NewLongRange(field, (min ?? Int64.MinValue), (max ?? Int64.MaxValue), true, true);
        }

        protected PrefixQuery BuildPrefixQuery(string field, string value)
        {
            var query = new PrefixQuery(new Term(field, value));

            return query;
        }

        protected FuzzyQuery BuildFuzzyQuery(string field, string value)
        {
            var query = new FuzzyQuery(new Term(field, value), 0.5f);

            return query;
        }

        protected Query BuildPrefixQueryAnd(string field, params string[] values)
        {
            var queries = values.Select(value => BuildPrefixQuery(field, value)).ToArray();
            return JoinQueriesAnd(queries);
        }

        protected Query BuildFuzzyQueryAnd(string field, params string[] values)
        {
            var queries = values.Select(value => BuildFuzzyQuery(field, value)).ToArray();
            return JoinQueriesAnd(queries);
        }

        protected PhraseQuery BuildPhraseQuery(string field, string value)
        {
            var query = new PhraseQuery();
            query.Add(new Term(field, value));
            return query;
        }

        protected TermQuery BuildMatchQuery(string field, string text)
        {
            return new TermQuery(new Term(field, text));
        }

        protected NumericRangeQuery BuildDateRangeQuery(string field, DateTime? min, DateTime? max)
        {
            min = min ?? DateTime.MinValue;
            max = max ?? DateTime.MaxValue;

            return NumericRangeQuery.NewLongRange(field, LuceneDateFormatter.ConvertToLucene(min.Value), LuceneDateFormatter.ConvertToLucene(max.Value), true, true);
        }

        #endregion

        public bool _willDoManulFlush { get; set; }
    }
}
