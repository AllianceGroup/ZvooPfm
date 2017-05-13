using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Framework;
using mPower.Framework.Mongo;
using mPower.Framework.Utils;

namespace mPower.Web.Admin
{
    public class ProdDeploymentHelper
    {
        private readonly MPowerSettings _settings;
        private readonly DeploymentHelper _deploymentHelper;
        private readonly CommonUtil _commonUtil;
        private readonly MongoUtil _mongoUtil;
        private readonly IContainer _container;
        private readonly DeploySettings _s;

        public ProdDeploymentHelper(MPowerSettings settings, DeploymentHelper deploymentHelper,
            CommonUtil commonUtil, MongoUtil mongoUtil, IContainer container)
        {
            _settings = settings;
            _deploymentHelper = deploymentHelper;
            _commonUtil = commonUtil;
            _mongoUtil = mongoUtil;
            _container = container;
            _s = _settings.Deploy;
        }

        /// <summary>
        /// 1. Send Prod to read mode
        /// 2. Copy mpower_write from prod to stage
        /// 3. Rebuild read model on stage
        /// 4. Backup rebuilded read database and lucene_indexes
        /// Disable back up of generated data if RebuildProdReadModelIncrement call expacted later
        /// </summary>
        /// <returns></returns>
        public string RebuildProdReadModel(bool createBackup = true)
        {
            _deploymentHelper.SwitchReadMode(String.Format(_s.ProdReadModeUrl, true));
            Console.WriteLine("Production was switched to read mode");

            var prodWriteBackupFolder = _mongoUtil.Backup(_s.StageWriteBackupFolder, _s.ProdWriteMongoUrl);
            Console.WriteLine("Production backup was made");

            _mongoUtil.Restore(prodWriteBackupFolder, _s.RegeneratorWriteMongoUrl);
            Console.WriteLine("Production backup was restored on stage regenerator");

            // change LuceneIndexesDirectory to not rewrite indexes 
            var initialLuceneIndexesDirectory = _settings.LuceneIndexesDirectory;
            _settings.LuceneIndexesDirectory = _s.RegeneratorLuceneIndexesDirectory;
            _container.Configure(config => config.For<MPowerSettings>().Use(_settings));

            _deploymentHelper.RegenerateReadModel(_s.RegeneratorReadMongoUrl.Url, _s.RegeneratorWriteMongoUrl.Url);
            Console.WriteLine("Read model was rebuilt");

            var prodGeneragedDbBackup = string.Empty;
            if (createBackup)
            {
                prodGeneragedDbBackup = _mongoUtil.Backup(_s.StageReadBackupFolder, _s.RegeneratorReadMongoUrl);
                Console.WriteLine("Read model was backuped");
            }

            // restore LuceneIndexesDirectory
            _settings.LuceneIndexesDirectory = initialLuceneIndexesDirectory;
            _container.Configure(config => config.For<MPowerSettings>().Use(_settings));

            return prodGeneragedDbBackup;
        }

        /// <summary>
        /// Apply transitions that where generated during execution of RebuildProdReadModel.
        /// site and all other 'transitions generators' should be turned off
        /// </summary>
        /// <returns></returns>
        public string RebuildProdReadModelIncrement()
        {
            // change LuceneIndexesDirectory to not rewrite indexes 
            var initialLuceneIndexesDirectory = _settings.LuceneIndexesDirectory;
            _settings.LuceneIndexesDirectory = _s.RegeneratorLuceneIndexesDirectory;
            _container.Configure(config => config.For<MPowerSettings>().Use(_settings));

            _deploymentHelper.RegenerateReadModelIncremet(_s.RegeneratorReadMongoUrl.Url, _s.RegeneratorWriteMongoUrl.Url, _s.ProdWriteMongoUrl.Url);
            Console.WriteLine("Read model increment was rebuilt");

            var prodGeneragedDbBackup = _mongoUtil.Backup(_s.StageReadBackupFolder, _s.RegeneratorReadMongoUrl);
            Console.WriteLine("Read model was backuped");

            // restore LuceneIndexesDirectory
            _settings.LuceneIndexesDirectory = initialLuceneIndexesDirectory;
            _container.Configure(config => config.For<MPowerSettings>().Use(_settings));

            var prodLuceneIndexesDirectory = Path.Combine(_s.StageServerRootFolder, "lucene_indexes_prod");
            if (Directory.Exists(prodLuceneIndexesDirectory))
                Directory.Delete(prodLuceneIndexesDirectory, true);
            _commonUtil.CopyDirectory(_s.RegeneratorLuceneIndexesDirectory, prodLuceneIndexesDirectory);
            Console.WriteLine("Lucene indexes were backuped");

            return prodGeneragedDbBackup;
        }

        public void EnableWriteMode()
        {
            _deploymentHelper.SwitchReadMode(String.Format(_s.ProdReadModeUrl, false));
        }

        /// <summary>
        /// 1. Restore read database from generated from prod write db on stage
        /// 2. Copy lucene indexes files to prod
        /// 3. Publish web app
        /// </summary>
        /// <param name="backupFolder"></param>
        /// <returns>Path to backup package</returns>
        public void RestoreDbPublishCopyIndexes(string backupFolder)
        {
            _mongoUtil.Restore(backupFolder, _s.ProdReadMongoUrl);
            Console.WriteLine("Deploy: Production read database was restore from folder: " + backupFolder);

            var prodIndexesDir = Path.Combine(_s.ProdServerRootFolder, "lucene_indexes");
            foreach (var subDirectory in new DirectoryInfo(prodIndexesDir).GetDirectories())
            {
                if (!subDirectory.Name.Equals(IntutitInstitutionLuceneService.IndexName, StringComparison.InvariantCultureIgnoreCase))
                {
                    Directory.Delete(subDirectory.FullName, true);
                }
            }
            Console.WriteLine("Deploy: Production lucene indexes was removed from dir: " + prodIndexesDir);
            var stageGeneratedIndexesDir = Path.Combine(_s.StageServerRootFolder, "lucene_indexes_prod");
            _commonUtil.CopyDirectory(stageGeneratedIndexesDir, prodIndexesDir);
            Console.WriteLine("Deploy: Production lucene indexes was copied from: " + stageGeneratedIndexesDir + " to: " + prodIndexesDir);
            Publish();
        }

        public void Publish()
        {
            var arguments = string.Format(" \"{0}\" \"{1}\"", _s.StageProdWebBuildFolder, Path.Combine(_s.ProdServerRootFolder, "Web"));
            Console.WriteLine("Deploy: Starting publish. batch. Path to batch is: " + _s.StagePublisherPath + ". Arguments is: " +
                         arguments);
            _commonUtil.RunProcess(_s.StagePublisherPath, arguments);
        }

        /// <summary>
        /// 1. Restore read database from backup package
        /// 2. Restore write database from backup package
        /// 3. Restore lucene indexes 
        /// 4. Restore web app
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public void RollbackFromPackage(string package)
        {
            var readBackupPath = Path.Combine(package, "read");
            var writeBackupPath = Path.Combine(package, "write");
            _mongoUtil.Restore(writeBackupPath, _s.ProdWriteMongoUrl);
            _mongoUtil.Restore(readBackupPath, _s.ProdReadMongoUrl);
            _commonUtil.CopyDirectory(Path.Combine(package, "lucene_indexes"), Path.Combine(_s.ProdServerRootFolder, "lucene_indexes"));
            _commonUtil.RunProcess(_s.StagePublisherPath, string.Format(" \"{0}\" \"{1}\"", Path.Combine(package, "web"), Path.Combine(_s.ProdServerRootFolder, "web")));

        }

        public List<string> GetAllPackages()
        {
            return Directory.GetDirectories(_s.StagePackagesFolder).ToList();
        }

        public string CreateBackupPackageOfProd()
        {
            var packageFolder = Path.Combine(_s.StagePackagesFolder, "package_" + _commonUtil.GenerateUniqueDateName());

            Console.Write("  Start copying of application... ");
            var appBackupFolder = Path.Combine(packageFolder, "web");
            //move web app folder to package
            _commonUtil.CopyDirectory(Path.Combine(_s.ProdServerRootFolder, "web"), appBackupFolder);
            Console.WriteLine("succeed");

            Console.Write("  Start copying of lucene indexes... ");
            var indexesBackupFolder = Path.Combine(packageFolder, "lucene_indexes");
            _commonUtil.CopyDirectory(Path.Combine(_s.ProdServerRootFolder, "lucene_indexes"), indexesBackupFolder);
            Console.WriteLine("succeed");

            Console.Write("  Start copying of write database... ");
            var writeDbBackupFolder = Path.Combine(packageFolder, "write");
            var writeBackup = _mongoUtil.Backup(_s.StageWriteBackupFolder, _s.ProdWriteMongoUrl);
            _commonUtil.CopyDirectory(writeBackup, writeDbBackupFolder);
            Console.WriteLine("succeed");

            Console.Write("  Start copying of read database... ");
            var readDbBackupFolder = Path.Combine(packageFolder, "read");
            var readBackup = _mongoUtil.Backup(_s.StageReadBackupFolder, _s.ProdReadMongoUrl);
            _commonUtil.CopyDirectory(readBackup, readDbBackupFolder);
            Console.WriteLine("succeed");

            Console.Write("  Start copying of intuit database... ");
            var intuitDbBackupFolder = Path.Combine(packageFolder, "intuit");
            var intuitBackup = _mongoUtil.Backup(_s.StageIntuitBackupFolder, _s.ProdIntuitMongoUrl);
            _commonUtil.CopyDirectory(intuitBackup, intuitDbBackupFolder);
            Console.WriteLine("succeed");

            Console.WriteLine("Deploy: Backup package was created in folder: " + packageFolder);

            return packageFolder;
        }
    }
}