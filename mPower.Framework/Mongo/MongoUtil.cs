using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using mPower.Framework.Utils;

namespace mPower.Framework.Mongo
{
    public class MongoUtil
    {
        private readonly string _pathToBackuper;
        private readonly string _pathToRestorer;
        private readonly CommonUtil _commonUtil;

        public MongoUtil(string pathToBackuper, string pathToRestorer, CommonUtil commonUtil)
        {
            _pathToBackuper = pathToBackuper;
            _pathToRestorer = pathToRestorer;
            _commonUtil = commonUtil;
        }

        public string Backup(string folder, string host, string dbName, string userName, string password)
        {
            var backUpFolder = Path.Combine(folder, _commonUtil.GenerateUniqueDateName());
            Directory.CreateDirectory(backUpFolder);

            var startIfo =
                new ProcessStartInfo(_pathToBackuper);
            startIfo.Arguments = String.Format(" --host {0} --db {1} --out {2} -u {3} -p {4} ", host, dbName, backUpFolder, userName, password);
            startIfo.UseShellExecute = true;


            var process = Process.Start(startIfo);
            process.WaitForExit();

            // move backup files to backup folder (without database name in path)
            var backupFilesDirectory = Path.Combine(backUpFolder, dbName);
            _commonUtil.CopyDirectory(backupFilesDirectory, backUpFolder);
            Directory.Delete(backupFilesDirectory, true);

            return backUpFolder;
        }

        public string Backup(string folder, MongoUrl url)
        {
            return Backup(folder, url.Server.ToString(), url.DatabaseName,
                  url.Username,
                  url.Password);
        }

        //folder with backup (database name) should be same as database name for restore
        public string Restore(string folder, string dbName, string host, string userName, string password)
        {
            var arguments = String.Format("--host {0} --drop --db {1} -u {3} -p {4} {2}", host, dbName, folder, userName, password);
            _commonUtil.RunProcess(_pathToRestorer, arguments);

            return folder;
        }

        public string Restore(string folder, MongoUrl url)
        {
            return Restore(folder, url.DatabaseName, url.Server.ToString(),
                   url.Username,
                   url.Password);
        }
    }
}
