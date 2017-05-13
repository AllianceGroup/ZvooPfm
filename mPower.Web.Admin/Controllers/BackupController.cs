using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Driver;
using mPower.Framework;
using mPower.Web.Admin.Models;
using mPower.Framework.Mongo;

namespace mPower.Web.Admin.Controllers
{
    public class BackupController : BaseAdminController
    {
        private readonly MPowerSettings _settings;
        private readonly MongoUtil _mongoUtil;

        public BackupController(MPowerSettings settings, MongoUtil mongoUtil)
        {
            _settings = settings;
            _mongoUtil = mongoUtil;
        }

        public ActionResult Index()
        {
            var readUrl = MongoUrl.Create(_settings.MongoReadDatabaseConnectionString);
            var writeUrl = MongoUrl.Create(_settings.MongoWriteDatabaseConnectionString);

            var model = new BackupModel();
            model.ReadDbName = readUrl.DatabaseName;
            model.ReadHost = readUrl.Server.ToString();
            model.ReadFolder = _settings.ReadBackupFolder;

            model.WriteDbName = writeUrl.DatabaseName;
            model.WriteHost = readUrl.Server.ToString();
            model.WriteFolder = _settings.WriteBackupFolder;

            var readBackUps = Directory.GetDirectories(_settings.ReadBackupFolder);

            var writeBackUps = Directory.GetDirectories(_settings.WriteBackupFolder);

            model.WriteBackups = writeBackUps.ToList();
            model.ReadBackups = readBackUps.ToList();

            return View(model);
        }

        public ActionResult Backup(string host, string dbName, string folder)
        {
            _mongoUtil.Backup(folder, host, dbName, _settings.BackupDbUserName, _settings.BackupDbPassword);

            return RedirectToAction("Index");
        }

        public ActionResult Restore(string host, string dbName, string folder)
        {
            _mongoUtil.Restore(folder, dbName, host, _settings.BackupDbUserName, _settings.BackupDbPassword);

            return RedirectToAction("Index");
        }
    }
}
