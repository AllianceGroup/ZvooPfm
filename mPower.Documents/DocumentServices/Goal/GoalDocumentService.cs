using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Goal;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Goal
{
    public class GoalDocumentService : BaseDocumentService< GoalDocument , GoalFilter>
    {
        public GoalDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Goals; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(GoalFilter filter)
        {           
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                yield return Query.EQ("UserId", filter.UserId);
            }
        }

        public void UpdateCalculatedData(string userId, string goalId, int monthsAheadNumber, DateTime calDate)
        {
            var query = Query.And(Query.EQ("UserId", userId), Query.EQ("_id", goalId));

            var update = Update<GoalDocument>
                .Set(x => x.MonthsAheadNumber, monthsAheadNumber)
                .Set(x => x.CalcDate, calDate);

            Update(query, update);
        }
    }
}