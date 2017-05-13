using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework;
using mPower.Framework.Extensions;
using mPower.Framework.Geo;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class OfferGroupDocumentService : BaseTemporaryService<OfferGroupDocument, OfferFilter>, IOfferGroupDocumentService
    {


        public OfferGroupDocumentService(MongoTemp temp) : base(temp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.OfferGroups; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(OfferFilter filter)
        {
            if (filter.StartDateGTE.HasValue)
            {
                yield return Query.GTE("Offers.StartDate", filter.StartDateGTE.Value);
            }
            if (filter.EndDateGTE.HasValue)
            {
                yield return Query.GTE("Offers.EndDate", filter.EndDateGTE.Value);
            }
            if (filter.Declined.HasValue)
            {
                yield return Query.EQ("Offers.Declined", filter.Declined.Value);
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
                yield return Query.In("Offers.CategoryName", BsonArray.Create(filter.CategoryNameIn));
            }
            if (filter.CategoryName.HasValue())
            {
                yield return Query.EQ("Offers.CategoryName", filter.CategoryName);
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
                        Query.WithinCircle("Offers.GeoLocation", filter.GeoLocation.Value.Longitude,
                                   filter.GeoLocation.Value.Latitude, ((double)(filter.Radius)) / 3959, true);
                }
            }
            //if (filter.GeoLocation.HasValue)
            //{
            //    if (filter.Radius.HasValue)
            //    {
            //        yield return Query.Near("Offers.GeoLocation", filter.GeoLocation.Value.Longitude, filter.GeoLocation.Value.Latitude, ((double)(filter.Radius)) / 3959, true);
            //    }
            //    else
            //    {
            //        yield return Query.Near("Offers.GeoLocation", filter.GeoLocation.Value.Longitude, filter.GeoLocation.Value.Latitude);
            //    }
            //}
        }

        public IEnumerable<OfferGroupSearchItem> GetClosestByFilter(OfferFilter filter)
        {
            var paging = filter.PagingInfo;
            var queries = BuildFilterQuery(filter).ToList();
            if (!queries.Any())
                return null;
                //return new List<IGrouping<OfferDocument, OfferGroupDocument>>();


            var cursor = Items.FindAs<OfferGroupDocument>(Query.And(queries));
            IEnumerable<OfferGroupSearchItem> result = null;
            if (filter.GeoLocation.HasValue)
            {
                result = cursor.Select(x => x.Offers.Min<OfferDocument, OfferGroupSearchItem>
                                                (offer =>
                                                 Location.DisnatceInMiles(offer.GeoLocation,
                                                                          filter.GeoLocation.Value),
                                                 (doc, distance) => new OfferGroupSearchItem
                                                 {
                                                     Closest = doc,
                                                     Distance = distance,
                                                     Group = x
                                                 }));
                if (filter.Radius.HasValue)
                {
                    result = result.Where(x => x.Distance <= filter.Radius.Value);
                }
                result = result.OrderBy(x=> x.Distance).ToList();
            }
            paging.TotalCount = result.Count();
            filter.PagingInfo = paging;
            return result.Skip(paging.Skip).Take(paging.Take);
        }

        public OfferGroupDocument GetOfferGroup(string merchant, string title)
        {
            return GetByFilter(new OfferFilter() {MerchantName = merchant, Title = title}).SingleOrDefault();
        }

        public void UpsertOffer(OfferDocument offer)
        {
            var query = Query.And(Query<OfferGroupDocument>.EQ(x => x.Title, offer.Title),
                                  Query<OfferGroupDocument>.EQ(x => x.Merchant, offer.Merchant));
            var update = Update<OfferGroupDocument>.AddToSet(x => x.Offers, offer);
            Items.Update(query, update, UpdateFlags.Upsert);
        }

        public void UpsertMany(string title, string merchant, IEnumerable<OfferDocument> offerDocuments)
        {
            var query = Query.And(Query<OfferGroupDocument>.EQ(x => x.Title, title),
                                  Query<OfferGroupDocument>.EQ(x => x.Merchant, merchant));
            var update = Update<OfferGroupDocument>.AddToSetEach(x => x.Offers, offerDocuments);
            Items.Update(query, update, UpdateFlags.Upsert);
        }
    }

    public class OfferGroupSearchItem
    {
        public OfferDocument Closest { get; set; }

        public double Distance { get; set; }

        public OfferGroupDocument Group { get; set; }
    }

    public interface IOfferGroupDocumentService
    {
        void UpsertOffer(OfferDocument offer);
        void UpsertMany(string title, string merchant, IEnumerable<OfferDocument> offerDocuments);
    }
}