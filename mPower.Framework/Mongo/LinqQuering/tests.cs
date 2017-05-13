using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using mPower.Framework.UpdateBuilder;
using MongoDB.Driver.Linq;

namespace mPower.Framework.Mongo.LinqQuering
{
    [TestFixture]
    public abstract class BaseMongoLinqTest<T>
    {
        public abstract IQueryable<T> BuildLinqQuery(IQueryable<T> queryable);
        public abstract IMongoQuery BuildMongoQuery();

        [Test]
        public void Test()
        {
            var colletion = MongoServer.Create("mongodb://admin(admin):admin@localhost:27020").GetDatabase("integration_tests").GetCollection("test");
            var query = BuildLinqQuery(colletion.AsQueryable<T>());
            var translatedQuery = MongoQueryTranslator.Translate(query);
            var result = ((SelectQuery)translatedQuery).BuildQuery();
            Assert.AreEqual(BuildMongoQuery().ToString().Trim(), result.ToString().Trim());
        }
    }

    [TestFixture]
    public abstract class BaseMongoLinqTest
    {
        public void AssertQueries<T>(Func<IQueryable<T>, IQueryable<T>> buildLinqQuery, IMongoQuery mongoQuery)
        {
            var colletion = MongoServer.Create("mongodb://admin(admin):admin@localhost:27020").GetDatabase("integration_tests").GetCollection("test");
            var query = buildLinqQuery(colletion.AsQueryable<T>());
            var translatedQuery = MongoQueryTranslator.Translate(query);
            var result = ((SelectQuery)translatedQuery).BuildQuery();
            Assert.AreEqual(mongoQuery.ToString().Trim(), result.ToString().Trim());
        }
    }

    [TestFixture]
    public class MongoWhereQueriesTest:BaseMongoLinqTest
    {
        [Test]
        public void OneEqual()
        {
            AssertQueries<TestDocument>((q) => q.Where(x => x.Name == "name"), Query.EQ("Name", "name"));
        }

        [Test]
        public void TwoAndEqual()
        {
            AssertQueries<TestDocument>((q) => q.Where(x => x.Name == "name" && x.Value == 5), Query.And(Query.EQ("Name", "name"),Query.EQ("Value",5)));
        }

        [Test]
        public void TwoOrEqual()
        {
            AssertQueries<TestDocument>((q) => q.Where(x => x.Name == "name" || x.Value == 5), Query.Or(Query.EQ("Name", "name"), Query.EQ("Value", 5)));
        }
    }

    [TestFixture]
    public class MongoWhereQueriesTestgeneric : BaseMongoLinqTest<TestDocument>
    {
        public override IQueryable<TestDocument> BuildLinqQuery(IQueryable<TestDocument> queryable)
        {
            return queryable.Where(x => x.Name == "name");
        }

        public override IMongoQuery BuildMongoQuery()
        {
            return Query.EQ("Name", "name");
        }
    }
}