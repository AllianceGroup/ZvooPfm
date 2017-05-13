using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework;
using mPower.Framework.Environment;

namespace mPower.Documents.Segments
{
    public class SegmentAggregationService
    {
        private readonly MongoRead _mongoRead;
        private readonly IIdGenerator _idGenerator;
        private readonly UserDocumentService _userService;
        private readonly SegmentQueryBuilder _builder;


        public SegmentAggregationService(MongoRead mongoRead, IIdGenerator idGenerator, UserDocumentService userService, SegmentQueryBuilder builder)
        {
            _mongoRead = mongoRead;
            _idGenerator = idGenerator;
            _userService = userService;
            _builder = builder;
        }

        private MongoCollection UserSegments
        {
            get { return _mongoRead.UserSegments; }
        }

        private List<T> GetByQuery<T>(MongoCollection collection, IMongoQuery query, Action<MongoCursor<T>> action)
        {
            var cursor = collection.FindAs<T>(query);
            action(cursor);
            return cursor.ToList();
        }

        private IMongoQuery EnsureSegmentCreated(UserSegmentTypeEnum segmentType, DateTime date, string userId, string merchant, string spendingCategory, string affiliateId, string affiliateName)
        {
            var searchQuery = _builder.GetSegmentQuery(segmentType, date, userId, merchant, spendingCategory);

            bool exists = GetByQuery<UserSegment>(UserSegments, searchQuery, c => c.SetFields("_id").SetLimit(1)).Any();
            if (!exists)
            {
                if (string.IsNullOrEmpty(affiliateId) || string.IsNullOrEmpty(affiliateName))
                {
                    GetAffiliateInfo(userId, out affiliateId, out affiliateName);
                }

                UserSegments.Insert(new UserSegment
                {
                    Id = _idGenerator.Generate(),
                    UserId = userId,
                    UserSegmentType = segmentType,
                    Date = date.Date,
                    FormattedDate = _builder.FormatSegmentDate(segmentType, date),
                    AffiliateId = affiliateId,
                    AffiliateName = affiliateName,
                    AggregateData =
                    {
                        Merchant = (merchant ?? "").ToLowerInvariant(),
                        SpendingCategory = (spendingCategory ?? "").ToLowerInvariant(),
                    },
                });
            }

            return searchQuery;
        }

        private void GetAffiliateInfo(string userId, out string affiliateId, out string affiliateName)
        {
            var user = _userService.GetById(userId);
            if (user != null)
            {
                affiliateId = user.ApplicationId;
                affiliateName = user.AffiliateName;
            }
            else
            {
                affiliateId = "";
                affiliateName = "";
            }
        }

        public void UpdateSegment(UserSegmentTypeEnum segmentType, DateTime eventDate, string userId, IMongoUpdate update, string affiliateId = null, string affiliateName = null)
        {
            if (segmentType == UserSegmentTypeEnum.Expense)
            {
                throw new ArgumentOutOfRangeException("segmentType", "For segments of type 'Expense' use 'UpdateExpenseSegment' method.");
            }

            var searchQuery = EnsureSegmentCreated(segmentType, eventDate, userId, "", "", affiliateId, affiliateName);

            UserSegments.Update(searchQuery, update);
        }

        public void UpdateExpenseSegment(DateTime eventDate, string userId, IMongoUpdate update, string merchant, string spendingCategory, string affiliateId = null, string affiliateName = null)
        {
            var searchQuery = EnsureSegmentCreated(UserSegmentTypeEnum.Expense, eventDate, userId, merchant, spendingCategory, affiliateId, affiliateName);

            UserSegments.Update(searchQuery, update);
        }

        public void UpdateAllUserSegments(string userId, IMongoUpdate update)
        {
            var searchQuery = Query.EQ("UserId", userId);

            UserSegments.Update(searchQuery, update, UpdateFlags.Multi);
        }

        public void UpdateSegments(IMongoQuery query, IMongoUpdate update)
        {
            UserSegments.Update(query, update, UpdateFlags.Multi);
        }

        public void UpdateMortgage(UserSegmentTypeEnum segmentType, DateTime eventDate, string userId, MortgageData mortgageData)
        {
            var query = Query.And(
                Query.EQ("UserId", userId),
                Query.EQ("UserSegmentType", (int) segmentType),
                Query.EQ("AggregateData.Mortgages.Id", mortgageData.Id));

            var update = Update
                .Set("AggregateData.Mortgages.$.AmountInCents", mortgageData.AmountInCents)
                .Set("AggregateData.Mortgages.$.InterestRate", mortgageData.InterestRate);
            UserSegments.Update(query, update);
            
            var update2 = Update<UserSegment>
                .Set(x => x.Date, eventDate.Date)
                .Set(x => x.FormattedDate, _builder.FormatSegmentDate(segmentType, eventDate));
            UserSegments.Update(query, update2);
        }

        public List<SegmentUserData> GetSegmentUser(SegmentData segment, DateTime? onDate = null)
        {
            var collectionName = "mr_" + _idGenerator.Generate();

            UserSegments.MapReduce(new MapReduceArgs
            {
                Query = _builder.BuildInitialFilterQuery(segment, onDate),
                MapFunction = new BsonJavaScript(_builder.GetMapFunction(segment)),
                ReduceFunction = new BsonJavaScript(_builder.GetReduceFunction(segment)),
                FinalizeFunction = new BsonJavaScript(_builder.Finalize),
                OutputMode = MapReduceOutputMode.Replace,
                OutputCollectionName = collectionName
            });
            var items = _mongoRead.GetCollection(collectionName);

            var searchQuery = _builder.BuildSegmentSearchQuery(segment);
            var result = items.FindAs<BsonDocument>(searchQuery).SetFields("_id", "value.AffiliateId").ToList();
            items.Drop();

            return result.Select(item => 
                new SegmentUserData
                {
                    Id = item.GetElement("_id").Value.AsBsonDocument.GetElement("userId").Value.AsString,
                    AffiliateId = item.GetElement("value").Value.AsBsonDocument.GetElement("AffiliateId").Value.AsString,
                }).ToList();
        }
    }
}

