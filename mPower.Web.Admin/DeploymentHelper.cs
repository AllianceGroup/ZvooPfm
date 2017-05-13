using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;
using StructureMap.Configuration.DSL;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Patches;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using mPower.Framework.Mongo;
using mPower.Framework.Registries;
using NLog;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;

namespace mPower.Web.Admin
{
    public class InMemoryDocumsntServicesRegistry : Registry
    {
        public InMemoryDocumsntServicesRegistry()
        {
            For<InMemoryDatabase>().Singleton();
            //For<LedgerDocumentService>().Use<InMemoryLedgerDocumentService>();
            //For<UserDocumentService>().Use<InMemoryUserDocumentService>();
        }
    }

    public class MongoDocumsntServicesRegistry : Registry
    {
        public MongoDocumsntServicesRegistry()
        {
            //For<LedgerDocumentService>().Use<LedgerDocumentService>();
            //For<UserDocumentService>().Use<UserDocumentService>();
        }
    }

    public class DeploymentHelper
    {
        private static readonly Logger _logger = MPowerLogManager.CurrentLogger;

        private MongoRead _read;
        private readonly IContainer _container;
        private readonly MongoUtil _mongoUtil;
        private TransactionLuceneService _transactionLuceneSerivce;
        private SendMailGroupLuceneService _sendMailGroupLuceneService;
        private readonly MPowerSettings _settings;
        private ITransitionRepository _transitionRepository;


        public DeploymentHelper(MPowerSettings settings, MongoRead read, MongoWrite write, IContainer container, MongoUtil mongoUtil,
            TransactionLuceneService transactionLuceneSerivce, SendMailGroupLuceneService sendMailGroupLuceneService)
        {
            _read = read;
            _container = container;
            _mongoUtil = mongoUtil;
            _transactionLuceneSerivce = transactionLuceneSerivce;
            _sendMailGroupLuceneService = sendMailGroupLuceneService;
            _settings = settings;
        }

        public void SwitchReadMode(string url)
        {
            using (var client = new WebClient())
            {
                var enc = new UTF8Encoding();
                var data = client.DownloadData(url);
                var status = enc.GetString(data);
            }
        }

        #region Read Model Generation

        public void RegenerateReadModel(string readConnection, string writeConnection, bool useInMemoryRegeneration = false)
        {

            RunWriteModelPatches(readConnection, writeConnection);

            var sw = new Stopwatch();
            sw.Start();
            _logger.Info("Read model regeneration started.");
            Console.WriteLine("Read model regeneration started.");

            // flush lucene documents to disc after each 2000 of modifications
            _transactionLuceneSerivce = new TransactionLuceneService(_settings);
            _transactionLuceneSerivce.SetFlushAfter(10000);
            _transactionLuceneSerivce.RemoveIndexFromDisc();

            _container.Configure(
                config => config.For<TransactionLuceneService>().Singleton().Use(_transactionLuceneSerivce));

            _sendMailGroupLuceneService.RemoveIndexFromDisc();
            // flush lucene documents to disc after each 2000 of modifications
            _sendMailGroupLuceneService = new SendMailGroupLuceneService(_settings);
            _sendMailGroupLuceneService.SetFlushAfter(2000);
            _container.Configure(
                config => config.For<SendMailGroupLuceneService>().Singleton().Use(_sendMailGroupLuceneService));

            ReconfigureMongos(readConnection, writeConnection);

            if (useInMemoryRegeneration)
            {
                _container.Configure(config => config.AddRegistry<InMemoryDocumsntServicesRegistry>());
            }
            else
            {
                _container.Configure(config => config.AddRegistry<MongoDocumsntServicesRegistry>());
            }

            _read.Database.Drop();
            _read.EnsureIndexes();

            _logger.Info("Old read model data was removed.");
            Console.WriteLine("Old read model data was removed.");

            int counter = 0;
            WorkWithTransitions(readConnection, writeConnection, (transitions, dispatcher, transitionRepository) =>
            {
                foreach (var transition in transitions)
                {
                    foreach (var evnt in transition.Events)
                    {
                        try
                        {
                            dispatcher.Dispatch(evnt.Data);
                        }
                        catch
                        {
                            Console.WriteLine("Last dispatched event: \n {0}",Json.Encode(evnt.Data));
                            throw;
                        }
                       
                    }

                    counter++;
                    if (counter % 10000 == 0)
                    {
                        Console.WriteLine(String.Format("{0} of transitions were regenerated", counter));
                    }
                }
            });

            //flush rest of documents
            if(useInMemoryRegeneration)
            {
                var database = _container.GetInstance<InMemoryDatabase>();
                database.FlushAll();
                _container.EjectAllInstancesOf<InMemoryDatabase>();
                //Maybe IDisposable implementation is needed
            }
            _transactionLuceneSerivce.Flush(true);
            _sendMailGroupLuceneService.Flush(true);
            sw.Stop();
            Console.WriteLine(string.Format("Read model regenerated in: {0} ({1} transitions)", sw.Elapsed.ToString(), counter));
            _logger.Info(string.Format("Read model regenerated in: {0} ({1} transitions)", sw.Elapsed.ToString(), counter));
        }

        public void RegenerateReadModelIncremet(string readConnection, string writeConnection, string writeIncrementConnection)
        {
            // we are not able to apply patch to new thransitions because patch can require whole set of transitions to work correctly
            // one more full regeneration required to affect this transitions

            var sw = new Stopwatch();
            sw.Start();
            _logger.Info("Read model incremental regeneration started.");
            Console.WriteLine("Read model incremental regeneration started.");

            // flush lucene documents to disc after each 2000 of modifications
            _transactionLuceneSerivce = new TransactionLuceneService(_settings);
            _transactionLuceneSerivce.SetFlushAfter(10000);
            _container.Configure(config => config.For<TransactionLuceneService>().Singleton().Use(_transactionLuceneSerivce));

            // flush lucene documents to disc after each 2000 of modifications
            _sendMailGroupLuceneService = new SendMailGroupLuceneService(_settings);
            _sendMailGroupLuceneService.SetFlushAfter(2000);
            _container.Configure(config => config.For<SendMailGroupLuceneService>().Singleton().Use(_sendMailGroupLuceneService));

            int counter = 0;
            WorkWithTransitionsIncrementally(readConnection, writeConnection, writeIncrementConnection, (transitions, dispatcher) =>
            {
                foreach (var transition in transitions)
                {
                    foreach (var evnt in transition.Events)
                    {
                        dispatcher.Dispatch(evnt.Data);
                        
                    }

                    counter++;
                    if (counter % 10000 == 0)
                    {
                        Console.WriteLine(String.Format("{0} of transitions were regenerated incrementally", counter));
                    }
                }
            });

            //flush rest of documents
            _transactionLuceneSerivce.Flush(true);
            _sendMailGroupLuceneService.Flush(true);
            sw.Stop();
            Console.WriteLine(string.Format("Read model increment regenerated in: {0} ({1} transitions)", sw.Elapsed.ToString(), counter));
            _logger.Info(string.Format("Read model increment regenerated in: {0} ({1} transitions)", sw.Elapsed.ToString(), counter));
        }

        public void RunWriteModelPatches(string readConnection, string writeConnection)
        {
            var patchesString = _settings.Deploy.PatchesBeforeRegeneration;
            if (!String.IsNullOrEmpty(patchesString))
            {
                var patches = _container.GetAllInstances<IPatch>();

                var patchIds = patchesString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

                var patchesToRun = patches.Where(x => patchIds.Contains(x.Id)).OrderBy(x => x.Id).ToList();

                foreach (var patch in patchesToRun)
                {
                    var msg = string.Format("Running patch: {0} - {1}", patch.Id, patch.Name);
                    _logger.Info(msg);
                    Console.WriteLine(msg);
                    WorkWithTransitions(readConnection, writeConnection, patch.Apply, patch.UseIncomeTransitions);
                    msg = string.Format("Completed with patch: {0} - {1}", patch.Id, patch.Name);
                    _logger.Info(msg);
                    Console.WriteLine(msg);
                }
            }
        }

        public void CreateStageBackup()
        {
            Console.Write("  Start copying of write database... ");
            _mongoUtil.Backup(_settings.Deploy.StageWriteBackupFolder, _settings.Deploy.StageWriteMongoUrl);
            Console.WriteLine("succeed");

            Console.Write("  Start copying of intuit database... ");
            _mongoUtil.Backup(_settings.Deploy.StageIntuitBackupFolder, _settings.Deploy.StageIntuitMongoUrl);
            Console.WriteLine("succeed");
        }

        public void WorkWithTransitions(string readConnection, string writeConnection, Action<List<Transition>, Dispatcher, ITransitionRepository> action, bool loadTransitions = true)
        {
            ReconfigureMongos(readConnection, writeConnection);

            ReconfigureTransitions(writeConnection);

            _transitionRepository = _container.GetInstance<ITransitionRepository>();
            _transitionRepository.EnsureIndexes();

            var dispatcher = Dispatcher.Create(d => d
                    .SetServiceLocator(new StructureMapServiceLocator(_container))
                    .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly)// mPower.EventHandlers assembly
                );

            if (loadTransitions)
            {
                var start = 0;
                const int count = 100000;
                List<Transition> transitions;
                do
                {
                    transitions = _transitionRepository.GetTransitions(start, count);
                    action(transitions, dispatcher, _transitionRepository);
                    start += count;
                } while (transitions.Count >= count); // transitions.Count < count means that there are no more transitions
            }
            else
            {
                action(new List<Transition>(), dispatcher, _transitionRepository);
            }

            //restore old connection strings
            new MongoRegistry(_container);
            ReconfigureMongos(_settings.MongoReadDatabaseConnectionString, _settings.MongoWriteDatabaseConnectionString);
            ReconfigureTransitions(_settings.MongoWriteDatabaseConnectionString);
        }

        public void WorkWithTransitionsIncrementally(string readConnection, string writeConnection, string writeIncrementConnection, Action<List<Transition>, Dispatcher> action)
        {
            ReconfigureMongos(readConnection, writeConnection);
            ReconfigureTransitions(writeConnection);

            _transitionRepository = _container.GetInstance<ITransitionRepository>();

            var applyedTransitionsCount = _transitionRepository.CountTransitions();

            // retrive transitions increment
            ReconfigureTransitions(writeIncrementConnection);
            _transitionRepository = _container.GetInstance<ITransitionRepository>();
            var newTransitions = _transitionRepository.GetTransitions((int)applyedTransitionsCount, int.MaxValue);
            ReconfigureTransitions(writeConnection);

            var dispatcher = Dispatcher.Create(d => d
                .SetServiceLocator(new StructureMapServiceLocator(_container))
                .AddHandlers(typeof (CreditIdentityAlertDocumentEventHandler).Assembly) // mPower.EventHandlers assembly
            );

            action(newTransitions, dispatcher);

            //restore old connection strings
            new MongoRegistry(_container);
            ReconfigureMongos(_settings.MongoReadDatabaseConnectionString, _settings.MongoWriteDatabaseConnectionString);
            ReconfigureTransitions(_settings.MongoWriteDatabaseConnectionString);
        }

        private void ReconfigureMongos(string read, string write)
        {
            _container.Configure(config =>
            {
                // Mongo Read database
                config.For<MongoRead>().Singleton().Use(() =>
                                                        new MongoRead(read));

                // Mongo Write database
                config.For<MongoWrite>().Singleton().Use(() =>
                                                        new MongoWrite(write));
            });
            _read = _container.GetInstance<MongoRead>();
        }

        private void ReconfigureTransitions(string writeConnectionString)
        {
            #region Transitionas Configuration

            // 
            // Domain and Event store configuration
            //
            var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            var transitionsRepository = new MongoTransitionRepository(
                new AssemblyQualifiedDataTypeRegistry(),
                writeConnectionString);

            var transitionsStorage = new TransitionStorage(transitionsRepository);

            _container.Configure(config =>
            {
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<ISnapshotRepository>().Singleton().Use(new MongoSnapshotRepository(writeConnectionString));
                config.For<ITransitionRepository>().Singleton().Use(transitionsRepository);
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<IEventBus>().Use<ParalectServiceBusEventBus>();

                // We are using default implementation of repository
                config.For<IRepository>().Use<Repository>();
            });

            #endregion
        }

        #endregion
    }
}