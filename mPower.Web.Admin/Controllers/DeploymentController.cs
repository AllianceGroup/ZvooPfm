using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.Domain;
using Paralect.Transitions;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using mPower.Framework.Mongo;
using mPower.Web.Admin.Models;

namespace mPower.Web.Admin.Controllers
{
    public class DeploymentController : BaseAdminController
    {
        private readonly MPowerSettings _settings;
        private readonly DeploymentHelper _deploymentHelper;
        private readonly MongoUtil _mongoUtil;
        private readonly ProdDeploymentHelper _prodDeployment;
        private readonly MongoWrite _write;
        private readonly ITransitionRepository _transitionRepository;
        private readonly AccountsService _accountsService;
        private readonly DeploySettings _s;

        public DeploymentController(MPowerSettings settings, DeploymentHelper deploymentHelper,
            MongoUtil mongoUtil, ProdDeploymentHelper prodDeployment, MongoWrite write,
            ITransitionRepository transitionRepository, AccountsService accountsService)
        {
            _settings = settings;
            _deploymentHelper = deploymentHelper;
            _mongoUtil = mongoUtil;
            _prodDeployment = prodDeployment;
            _write = write;
            _transitionRepository = transitionRepository;
            _accountsService = accountsService;
            _s = _settings.Deploy;

        }

        public string GeneratedReadDatabasePath
        {
            get
            {
                var rawValue = Session["GeneratedReadDatabasePath"];

                return rawValue == null ? String.Empty : rawValue.ToString();
            }
            set { Session["GeneratedReadDatabasePath"] = value; }
        }

        public ActionResult Index(string message, string backupFolder, string package)
        {
            var model = new DeploymentModel {Message = message, BackupFolder = backupFolder, Package = package};
            var packages = _prodDeployment.GetAllPackages();
            var list = new List<SelectListItem> {new SelectListItem {Text = "Please, select", Value = "", Selected = String.IsNullOrEmpty(package)}};
            list.AddRange(packages.Select(x => new SelectListItem {Text = x, Value = x, Selected = x == package}));
            model.Packages = list;

            return View(model);
        }

        public ActionResult RebuildProdReadModel()
        {
            var prodGeneragedDbBackup = _prodDeployment.RebuildProdReadModel();

            GeneratedReadDatabasePath = prodGeneragedDbBackup;

            return RedirectToAction("Index", new { message = "Done without errors. You can test app. Generated read database folder: " + prodGeneragedDbBackup, backupFolder = prodGeneragedDbBackup });
        }

        public ActionResult BackupAndPublishProdApp(string backupFolder)
        {
            var package = _prodDeployment.CreateBackupPackageOfProd();
            _prodDeployment.RestoreDbPublishCopyIndexes(backupFolder);

            return RedirectToAction("Index", new {package, message = "Deployment process was finished"});
        }

        public ActionResult OneClickDeploymentToProd()
        {
            RebuildProdReadModel();
            return BackupAndPublishProdApp(GeneratedReadDatabasePath);
        }

        public ActionResult RollbackFromPackage(string package)
        {
            string msg;
            if (string.IsNullOrEmpty(package))
            {
                msg = "Please, select package";
                ModelState.AddModelError("package", msg);
            }
            else
            {
                _prodDeployment.RollbackFromPackage(package);
                msg = "Prod app was restored from package: " + package;
            }

            return RedirectToAction("Index", new {message = msg});
        }

        public ActionResult SendProdToReadMode()
        {
            _deploymentHelper.SwitchReadMode(String.Format(_s.ProdReadModeUrl, true));

            return RedirectToAction("Index", new { message = "Production app was sent to read mode." });
        }

        public ActionResult SendProdToWriteMode()
        {
            _deploymentHelper.SwitchReadMode(String.Format(_s.ProdReadModeUrl, false));

            return RedirectToAction("Index", new { message = "Production app was sent to write mode." });
        }

        public ActionResult CreateBackupPackage()
        {
            var package = _prodDeployment.CreateBackupPackageOfProd();

            return RedirectToAction("Index", new { message = "Package was created. Folder: " + package });
        }

        public ActionResult CopyProdDatabases()
        {
            var msg = "Production database was copied to your local machine";
            try
            {
                //var prodReadBackup = _mongoUtil.Backup(_settings.ReadBackupFolder, _settings.Deploy.ProdReadMongoUrl);
                //_mongoUtil.Restore(prodReadBackup, _settings.LocalReadMongoUrl);

                var prodWriteBackup = _mongoUtil.Backup(_settings.WriteBackupFolder, _settings.Deploy.ProdWriteMongoUrl);
                _mongoUtil.Restore(prodWriteBackup, _settings.LocalWriteMongoUrl);

                //var yodleeBackup = _mongoUtil.Backup(_settings.YodleeBackupFolder, _settings.Deploy.ProdYodleeMongoUrl);
                //_mongoUtil.Restore(yodleeBackup, _settings.LocalYodleeMongoUrl);
            } 
            catch (Exception ex)
            {
                msg = string.Format("Error: {0}", ex.Message);
            }

            return RedirectToAction("Index", new {message = msg});
        }

        public ActionResult AddNewDefaultPfmAccounts()
        {
            var msg = "New default PFM accounts were successufully added.";
            try
            {
                var addAccountCmds = new List<ICommand>();
                var newExpenseAccounts = new List<string> { "Business", "Credit Card", "Insurance" };

                var start = 0;
                const int count = 100000;
                List<Transition> transitions;
                do
                {
                    transitions = _transitionRepository.GetTransitions(start, count);
                    foreach (var transition in transitions)
                    {
                        foreach (var rawEvent in transition.Events)
                        {
                            var @event = rawEvent.Data as Ledger_CreatedEvent;
                            if (@event != null && @event.TypeEnum == LedgerTypeEnum.Personal)
                            {
                                var query = Query.And(
                                    Query.EQ("_id.StreamId", transition.Id.StreamId),
                                    Query.ElemMatch("Events", Query.And(
                                        Query.Matches("TypeId", new BsonRegularExpression(".*Ledger_Account_AddedEvent.*")),
                                        Query.EQ("Data.AccountId", BaseAccounts.Salary)
                                    ))
                                );

                                if (_write.Transitions.Count(query) == 0)
                                {
                                    addAccountCmds.Add(_accountsService.CreatePersonalBaseAccounts(@event.LedgerId)
                                        .Where(x => x is Ledger_Account_CreateCommand)
                                        .Cast<Ledger_Account_CreateCommand>()
                                        .First(x => x.AccountId == BaseAccounts.Salary));

                                    var expenseAccountsToAdd = _accountsService.CommonPersonalExpenseAccounts().Where(x => newExpenseAccounts.Contains(x.Name));
                                    foreach (var expenseAccount in expenseAccountsToAdd)
                                    {
                                        addAccountCmds.AddRange(expenseAccount.GetCreateAccountCommands(IIdGenerator, @event.LedgerId));
                                    }
                                }
                            }
                        }
                    }
                    start += count;
                } while (transitions.Count >= count); // transitions.Count < count means that there are no more transitions

                

                if (addAccountCmds.Any())
                {
                    Send(addAccountCmds.ToArray());
                }
            }
            catch (Exception ex)
            {
                msg = string.Format("Error: {0}", ex.Message);
            }

            return RedirectToAction("Index", new {message = msg});
        }
    }
}
