using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.DocumentServices.Credit.Filter;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Credit
{
    public class CreditIdentityDocumentService : BaseDocumentService<CreditIdentityDocument, CreditIdentityFilter>
    {
        public CreditIdentityDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.CreditIdentities; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(CreditIdentityFilter filter)
        {
           yield return Query.EQ("UserId", filter.UserId);
        }

        public virtual List<CreditIdentityDocument> GetCreditIdentitiesByUserId(string userId)
        {
            return GetByFilter(new CreditIdentityFilter { UserId = userId });
        }

        public void AddCreditReport(string clientKey, CreditReportDocument doc)
        {
            var query = Query.EQ("_id", clientKey);
            var update = MongoDB.Driver.Builders.Update.PushWrapped("CreditReports", doc);

            Update(query, update);
        }

        public void AddCreditScore(string clientKey, CreditScoreDocument doc)
        {
            var query = Query.EQ("_id", clientKey);
            var update = MongoDB.Driver.Builders.Update.PushWrapped("CreditScores", doc);

            Update(query, update);
        }

        public void SetValidationQuestions(string clientKey, List<VerificationQuestionDocument> questions)
        {
            var query = Query.EQ("_id", clientKey);
            var update = MongoDB.Driver.Builders.Update.SetWrapped("Questions", questions);

            Update(query, update);
        }

        public List<VerificationQuestionDocument> GetValidationQuestions(string clientKey)
        {
            var creditIdentity = GetById(clientKey);
            return creditIdentity == null ? null : creditIdentity.Questions;
        }

        public void AddAlerts(string clientKey, List<AlertDocument> alerts)
        {
            var bsonArray = new BsonArray();
            foreach (var alert in alerts)
            {
                bsonArray.Add(alert.ToBsonDocument());
            }

            var query = Query.EQ("_id", clientKey);

            var update = MongoDB.Driver.Builders.Update.AddToSetEach("Alerts", bsonArray.Values);

            Update(query, update);
        }

        public void IncreaseQuestionaryAttempts(string clientKey)
        {
            var query = Query.EQ("_id", clientKey);
            var update = MongoDB.Driver.Builders.Update.Inc("QuestionaryAttempts", 1);

            Update(query, update);
        }

        public void MarkAsVerified(string clientKey, DateTime date, string ipAddress)
        {
            var query = Query.EQ("_id", clientKey);
            var update = MongoDB.Driver.Builders.Update<CreditIdentityDocument>.Set(x => x.IsVerified, true)
                .Set(x => x.VerificationDate, date)
                .Set(x => x.VerificationIpAddress, ipAddress);

            Update(query, update);
        }
    }
}
