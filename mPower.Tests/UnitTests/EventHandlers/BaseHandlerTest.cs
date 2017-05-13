using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Framework;
using mPower.Tests.Environment;
using NUnit.Framework;
using Paralect.Domain;
using Paralect.ServiceBus.Dispatching;
using StructureMap;

namespace mPower.Tests.UnitTests.EventHandlers
{
    public abstract class BaseHandlerTest
    {
        protected string _id = ObjectId.GenerateNewId().ToString();
        protected static DateTime CurrentDate = new DateTime(2011, 11, 11, 10, 10, 0, 0);

        protected IContainer _container;
        protected Dispatcher _dispatcher;

        public abstract IEnumerable<Object> Given();
        public virtual IEnumerable<Object> ShouldBeDeleted()
        {
            return null;
        }
        public abstract IEnumerable<IEvent> When();
        public abstract IEnumerable<Object> Expected();

        public virtual IEnumerable<Object> GivenLucene()
        {
            yield break;
        }
        public virtual IEnumerable<Object> ExpectedLucene()
        {
            yield break;
        }
        public virtual IEnumerable<Object> ShouldBeDeletedLucene()
        {
            yield break;
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.BootstrapStructureMap();
            _dispatcher = bootstrapper.BootsrapDispatcher();
            _container = bootstrapper.Container;
        }

        public void Dispatch(Action  finalAction = null,IgnoreList ignoreList = null)
        {
            var createNewData = true;
            var sessionId = String.Format("_{0}", ObjectId.GenerateNewId());
            var settings = _container.GetInstance<MPowerSettings>();
            MongoRead mongoRead = null;

            if (createNewData)
            {
                mongoRead = new MongoRead(settings.MongoReadDatabaseConnectionString + sessionId);
                _container.Configure(x => x.For<MongoRead>().Singleton().Use(mongoRead));

                settings.LuceneIndexesDirectory += sessionId;
                _container.Configure(x => x.For<MPowerSettings>().Singleton().Use(settings));
            }

            try
            {
                //
                // Given()
                // 

                var givenDocs = Given();

                if (givenDocs != null)
                {
                    foreach (Object givenDoc in givenDocs)
                    {
                        // Check that id was specified
                        if (GetId(givenDoc) == null)
                            throw new Exception(String.Format("Please specify Id of the {0} document in Given()",
                                                              givenDoc));

                        var collection = GetCollection(givenDoc, mongoRead);
                        collection.Insert(givenDoc);
                    }
                }


                var givenLucene = GivenLucene();

                foreach (var objDoc in givenLucene)
                {
                    if (objDoc.GetType() == typeof (EntryDocument))
                    {
                        var entryDocument = (EntryDocument) objDoc;

                        var luceneService = _container.GetInstance<TransactionLuceneService>();

                        luceneService.Insert(entryDocument);
                    }
                }


                //
                // When()
                //

                // Dispatch all commands/messages
                var events = When();
                foreach (var @event in events)
                    _dispatcher.Dispatch(@event);


                //
                // Expected()
                //
                var expectedDocs = Expected();

                if (expectedDocs != null)
                {
                    // Check that id was specified
                    foreach (Object expectedDoc in expectedDocs)
                    {
                        var id = GetId(expectedDoc);

                        if (id == null)
                            throw new Exception(String.Format("Please specify Id of the {0} document in Expected()",
                                                              expectedDoc));

                        var collection = GetCollection(expectedDoc, mongoRead);
                        var actualDocumentBson =
                            collection.FindOneAs<BsonDocument>(Query.EQ("_id", BsonValue.Create(id)));
                        var actualDocument = BsonSerializer.Deserialize(actualDocumentBson, expectedDoc.GetType());
                        var result = ObjectComparer.AreObjectsEqual(actualDocument, expectedDoc, ignoreList);

                        if (!result)
                            throw new Exception(
                                String.Format("Actual document ({0}, {1}) has different fields than expected document",
                                              actualDocument.GetType().FullName, id));
                    }
                }

                var expectedLucene = ExpectedLucene();

                foreach (var objDoc in expectedLucene)
                {
                    if (objDoc.GetType() == typeof (EntryDocument))
                    {
                        var expectedDoc = (EntryDocument) objDoc;

                        var luceneService = _container.GetInstance<TransactionLuceneService>();

                        var actualDoc = luceneService.GetById("_id", expectedDoc.Id);

                        var result = ObjectComparer.AreObjectsEqual(actualDoc, expectedDoc, ignoreList);

                        if (!result)
                            throw new Exception(
                                String.Format(
                                    "Actual lucene document ({0}, {1}) has different fields than expected document",
                                    actualDoc.GetType().FullName, expectedDoc.Id));
                    }
                }

                //
                // ShouldBeDeleted()
                //
                var shouldBeDeletedDocs = ShouldBeDeleted();
                if (shouldBeDeletedDocs != null)
                {
                    foreach (Object shouldBeDeletedDoc in shouldBeDeletedDocs)
                    {
                        var id = GetId(shouldBeDeletedDoc);

                        if (id == null)
                            throw new Exception(String.Format("Please specify Id of the {0} document in Expected()",
                                                              shouldBeDeletedDoc));

                        var collection = GetCollection(shouldBeDeletedDoc, mongoRead);
                        var actualDocumentBson =
                            collection.FindOneAs<BsonDocument>(Query.EQ("_id", BsonValue.Create(id)));

                        if (actualDocumentBson != null)
                        {
                            var actualDocument = BsonSerializer.Deserialize(actualDocumentBson,
                                                                            shouldBeDeletedDoc.GetType());
                            throw new Exception(
                                String.Format("Actual document is presenting in DB, but it should be deleted: {0}",
                                              actualDocument.GetType().FullName));
                        }
                    }
                }

                var shouldBeDeletedLucene = ShouldBeDeletedLucene();

                foreach (var objDoc in shouldBeDeletedLucene)
                {
                    if (objDoc.GetType() == typeof(EntryDocument))
                    {
                        var shouldBeDeletedDoc = (EntryDocument)objDoc;

                        var luceneService = _container.GetInstance<TransactionLuceneService>();

                        var actualDoc = luceneService.GetById("_id", shouldBeDeletedDoc.Id);

                        if (actualDoc != null)
                        {
                            throw new Exception(
                                String.Format("Actual lucene document is presenting in indexes, but it should be deleted: {0}",
                                              actualDoc.GetType().FullName));
                        }
                    }
                }

                if (finalAction != null)
                {
                    finalAction();
                }
            }
            finally
            {
                if (createNewData)
                {
                    mongoRead.Database.Drop();
                    if (Directory.Exists(settings.LuceneIndexesDirectory))
                        Directory.Delete(settings.LuceneIndexesDirectory, true);
                }
            }
        }

        public static MongoCollection GetCollection(Object document, MongoRead read)
        {
            var map = new Dictionary<Type, MongoCollection>
            {
                { typeof(TransactionDocument), read.Transactions},
                { typeof(EntryDocument), read.Entries},
                { typeof(TransactionsStatisticDocument), read.TransactionsStatistics},
                { typeof(BudgetDocument), read.Budgets},
                { typeof(LedgerDocument), read.Ledgers},
                { typeof(UserDocument), read.Users},
            };

            return map[document.GetType()];
        }

        public Object GetId(Object document)
        {
            var id = document.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(BsonIdAttribute))).FirstOrDefault();

            if (id == null)
                throw new Exception(String.Format("Id for document of type {0} not mapped", document.GetType().FullName));

            return id.GetValue(document, null);
        }
    }
}