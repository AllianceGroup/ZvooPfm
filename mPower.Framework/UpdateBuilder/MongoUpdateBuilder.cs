using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Moq;
using NUnit.Framework;

namespace mPower.Framework.UpdateBuilder
{
    public class TestDocument
    {
        public string Name { get; set; }

        public List<ItemDocument> Items { get; set; }

        public List<String> Srtings { get; set; }

        public ItemDocument InnerDoc { get; set; }

        public int Value { get; set; }
    }

    public class ItemDocument
    {
        public int Value { get; set; }
    }

    public abstract class UpdateBuilder<T>
    {
        public abstract UpdateBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value);
        public abstract UpdateBuilder<T> Push<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value);
        public abstract UpdateBuilder<T> Pull<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value);
        public abstract UpdateBuilder<T> AddToSet<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value);
    }


    public class MongoUpdateBuilder<T> : UpdateBuilder<T> where T : class
    {
        public MongoDB.Driver.Builders.UpdateBuilder ResultQuery
        {
            get { return query; }
        }

        private MongoDB.Driver.Builders.UpdateBuilder query = new MongoDB.Driver.Builders.UpdateBuilder();

        public override UpdateBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
        {
            var selectQuery = BuildSelectQuery(expression);
            //var type = typeof (T);
            //var selectQuery = "";
            //for (int i = 1; i < parts.Length; i++)
            //{
            //    if (parts.Length != i + 1)
            //    {
            //        selectQuery += ".";

            //        if (typeof(IEnumerable).IsAssignableFrom(type))
            //        {
            //            selectQuery += "$.";
            //        }
            //    }
            //    selectQuery += parts[i];
            //    var prop = type.GetProperty(parts[i]);
            //    type = prop.PropertyType;
            //}
            query = query.Set(selectQuery, BuildBsonValue(value));
            return this;
        }

        private static string BuildSelectQuery<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body.ToString().Replace("get_Item(", "").Replace("IsAny()", "$").Replace(')', '.');
            var parts = body.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                return parts[1];
            }
            var list = new List<string>(parts);
            list.RemoveAt(0);
            return string.Join(".", list);
        }

        public override UpdateBuilder<T> Push<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value)
        {
            var selectQuery = BuildSelectQuery(expression);
            query = query.Push(selectQuery, BuildBsonValue(value));
            return this;
        }

        public override UpdateBuilder<T> Pull<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value)
        {
            var selectQuery = BuildSelectQuery(expression);
            query = query.Pull(selectQuery, BuildBsonValue(value));
            return this;
        }

        public override UpdateBuilder<T> AddToSet<TProperty>(Expression<Func<T, List<TProperty>>> expression, TProperty value)
        {
            var selectQuery = BuildSelectQuery(expression);
            query = query.AddToSet(selectQuery, BuildBsonValue(value));
            return this;
        }

        private static BsonValue BuildBsonValue<TProperty>(TProperty value)
        {
            BsonValue bsonValue;
            return BsonTypeMapper.TryMapToBsonValue(value, out bsonValue) ? bsonValue : value.ToBsonDocument();
        }
    }

    public class InMemoryUpdateBuilder<T> where T : class
    {
        public string Result { get; set; }

        public List<Expression> expressions;

        public InMemoryUpdateBuilder<T> Set<TProperty>(Expression<Func<TestDocument, TProperty>> expression, TProperty value)
        {
            Result = expression.ToString();
            var doc = new TestDocument();
            ParameterExpression targetExp = Expression.Parameter(typeof(T), "target");
            ParameterExpression valueExp = Expression.Parameter(typeof(TProperty), "value");
            ParameterExpression param = Expression.Parameter(typeof(TProperty), "val");
            FieldInfo field = typeof(T).GetField("fieldName");
            MemberExpression fieldExp = Expression.Field(targetExp, field);

            BinaryExpression assignExp = Expression.Assign(fieldExp, param);
            var setter = Expression.Lambda<Action<TestDocument, TProperty>>
    (assignExp, targetExp, valueExp).Compile();

            setter(doc, value);
            expressions.Add(expression);
            return this;
        }
    }

    [TestFixture]
    public class MongoUpdateBuilderMultipleMethodsTest
    {
        [Test]
        public void test()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            var doc = new ItemDocument() { Value = 5 };
            updater.Set(x => x.Name, "Name").Set(x => x.InnerDoc, doc);
            Assert.AreEqual(updater.ResultQuery.ToString(),
                Update.Set("Name", "Name").Set("InnerDoc", doc.ToBsonDocument()).ToString());
        }
    }

    [TestFixture]
    public class MongoUpdateBuilderSetMetodTest
    {
        [Test]
        public void it_build_set_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            updater.Set(x => x.Name, "Name");
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("Name", "Name").ToString());
        }


        [Test]
        public void it_build_set_array_inner_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            updater.Set(x => x.Items[It.IsAny<int>()].Value, 5);
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("Items.$.Value", 5).ToString());
        }

        [Test]
        public void it_build_set_array_positionaly_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            updater.Set(x => x.Items[101].Value, 5);
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("Items.101.Value", 5).ToString());
        }

        [Test]
        public void it_build_set_array_doc_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            var doc = new ItemDocument() { Value = 5 };
            updater.Set(x => x.Items[It.IsAny<int>()], doc);
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("Items.$", doc.ToBsonDocument()).ToString());
        }

        [Test]
        public void it_build_set_array_value_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            updater.Set(x => x.Srtings[It.IsAny<int>()], "Value");
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("Srtings.$", "Value").ToString());
        }

        [Test]
        public void it_build_set_inner_doc()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            var doc = new ItemDocument() { Value = 5 };
            updater.Set(x => x.InnerDoc, doc);
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Set("InnerDoc", doc.ToBsonDocument()).ToString());
        }
    }

    [TestFixture]
    public class MongoUpdateBuilderPushMetodTest
    {
        [Test]
        public void it_build_set_query()
        {
            var updater = new MongoUpdateBuilder<TestDocument>();
            updater.Push(x => x.Srtings, "Name");
            Assert.AreEqual(updater.ResultQuery.ToString(), Update.Push("Srtings", "Name").ToString());
        }
    }
}