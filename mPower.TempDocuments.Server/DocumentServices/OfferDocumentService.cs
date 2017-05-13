using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework;
using mPower.Framework.Geo;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class OfferDocumentService : BaseTemporaryService<OfferDocument, OfferFilter>, IOfferDocumentService
    {
        public OfferDocumentService(MongoTemp temp) : base(temp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Offers; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(OfferFilter filter)
        {
            if (filter.Ids != null && filter.Ids.Any())
            {
                yield return Query.In("_id", BsonArray.Create(filter.Ids));
            }
            if (filter.StartDateGTE.HasValue)
            {
                yield return Query.GTE("StartDate", filter.StartDateGTE.Value);
            }
            if (filter.EndDateGTE.HasValue)
            {
                yield return Query.GTE("EndDate", filter.EndDateGTE.Value);
            }
            if (filter.Declined.HasValue)
            {
                yield return Query.EQ("Declined", filter.Declined.Value);
            }
            if (filter.MerchantNameIn != null && filter.MerchantNameIn.Any())
            {
                yield return Query.In("Merchant", BsonArray.Create(filter.MerchantNameIn));
            }
            if (filter.MerchantName.HasValue())
            {
                yield return Query.EQ("Merchant", filter.MerchantName);
            }
            if (filter.Title.HasValue())
            {
                yield return Query.EQ("Title", filter.Title);
            }
            if (filter.CategoryNameIn != null && filter.CategoryNameIn.Any())
            {
                yield return Query.In("CategoryName", BsonArray.Create(filter.CategoryNameIn));
            }
            if (filter.CategoryName.HasValue())
            {
                yield return Query.EQ("CategoryName", filter.CategoryName);
            }
            if (filter.SearchTerm.HasValue())
            {
                yield return
                    Query.Or(
                        Query.Matches("Title", BsonRegularExpression.Create(filter.SearchTerm, "i")),
                        Query.Matches("Merchant", BsonRegularExpression.Create(filter.SearchTerm, "i")));
            }
            if (filter.GeoLocation.HasValue)
            {
                if (filter.Radius.HasValue)
                {
                    yield return
                        Query.WithinCircle("GeoLocation", filter.GeoLocation.Value.Longitude,
                                   filter.GeoLocation.Value.Latitude, ((double)(filter.Radius)) / 3959, true);
                }
            }
            //if (filter.GeoLocation.HasValue)
            //{
            //    yield return Query.Near("GeoLocation", filter.GeoLocation.Value.Longitude, filter.GeoLocation.Value.Latitude, ((double)(filter.Radius ?? 1000000)) / 3959, true);
            //}
        }

        public override List<OfferDocument> GetByFilter(OfferFilter filter)
        {
            var result = base.GetByFilter(filter);
            if (filter.GeoLocation.HasValue)
            {
                result = result.OrderBy(x => Location.DisnatceInMiles(x.GeoLocation,filter.GeoLocation.Value)).ToList();
            }
            return result;
        }

        public GeoNearResult<OfferDocument>.GeoNearHits GetNearByFilter(OfferFilter filter, double latitude, double longitude, int limit)
        {
            var queries = BuildFilterQuery(filter).ToList();
            var query = queries.Any() ? Query.And(queries) : Query.Null;
            var cursor = Items.GeoNearAs<OfferDocument>(query, latitude, longitude, limit);
            return cursor.Hits;
        }

        public IEnumerable<OfferDocument> GetOffersByCategory(string categoryName)
        {
            var query = Query.EQ("CategoryName", categoryName);
            return GetByQuery(query);
        }

        public IEnumerable<OfferDocument> GetForAccounts(params string[] names)
        {
            var query = Query.In("CategoryName", BsonArray.Create(names));
            return GetByQuery(query); 
        }

        public IEnumerable<string> GetAllOffersIds()
        {
            return Items.FindAllAs<OfferDocument>().SetFields("_id").Select(x => x.Id).ToList();
        }

        public IEnumerable<OfferDocument> GetByIds(Location location,int radius, params string[] ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<OfferDocument>();
            }
            var filter = new OfferFilter
                             {
                                 Ids = ids,
                                 Declined = false,
                                 Radius = radius,
                                 GeoLocation = location,
                             };
            return GetByFilter(filter);
        }

        public IEnumerable<OfferDocument> GetForMerchants(Location location, int radius, params string[] names)
        {
            if (names == null || !names.Any())
            {
                return new List<OfferDocument>();
            }
            var filter = new OfferFilter
            {
                Declined = false,
                Radius = radius,
                GeoLocation = location,
            };
            var filterQuery = BuildFilterQuery(filter).ToList();
            filterQuery.Add(Query.Or(BuildMerchantIndexQueries(names)));
            var query = Query.And(filterQuery);
            return Items.FindAs<OfferDocument>(query);
        }

        private IEnumerable<IMongoQuery> BuildMerchantIndexQueries(params string[] names)
        {
            foreach (var name in names)
            {
                var currentIndex = new StringValueIndex(name);
                yield return Query<OfferDocument>.EQ(x => x.MerchantIndex.First5, currentIndex.First5);
            }
        }

        public IEnumerable<OfferDocument> GetGroup(string merchantName, string title)
        {
            return GetByFilter(new OfferFilter {MerchantName = merchantName, Title = title});
        }
    }

    public class OfferFilter : BaseFilter
    {
        public DateTime? StartDateGTE { get; set; }
        public DateTime? EndDateGTE { get; set; }
        public bool? Declined { get; set; }
        public string SearchTerm { get; set; }
        public int? Radius { get; set; }
        public virtual Location? GeoLocation { get; set; }
        public List<string> MerchantNameIn { get; set; } 
        public List<string> CategoryNameIn { get; set; }
        public string CategoryName { get; set; }
        public string MerchantName { get; set; }
        public string Title { get; set; }
        public string[] Ids { get; set; }
    }

    public interface IOfferDocumentService
    {
        void InsertMany(IEnumerable<OfferDocument> doc);
        IEnumerable<string> GetAllOffersIds();
        void Insert(OfferDocument offerDocument);
    }
}