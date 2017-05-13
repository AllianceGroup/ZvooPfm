using System;
using System.IO;
using System.Reflection;
using mPower.Framework;
using mPower.Web.Admin;
using StructureMap.Attributes;

namespace mPower.DeployTool
{
    public class DeployModule
    {
        [SetterProperty]
        public MPowerSettings Settings { get; set; }

        [SetterProperty]
        public DeploymentHelper DeploymentHelper { get; set; }

        [SetterProperty]
        public ProdDeploymentHelper ProdDeploymentHelper { get; set; }

        public ExitCode RegenerateStageReadModel()
        {
            var exitCode = ExitCode.Success;

            try
            {
                DeploymentHelper.CreateStageBackup();
                DeploymentHelper.RegenerateReadModel(Settings.Deploy.StageReadMongoUrl.Url, Settings.Deploy.StageWriteMongoUrl.Url);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during read model regeneration: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorRegenerateReadModel;
            }

            return exitCode;
        }

        public ExitCode RegenerateProdReadModel()
        {
            var exitCode = ExitCode.Success;

            try
            {
                Console.WriteLine("Deploy: Production read model regeneration was started.");
                ProdDeploymentHelper.RebuildProdReadModel(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during production read model regeneration: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorRegenerateReadModel;
                ProdDeploymentHelper.EnableWriteMode();
            }

            return exitCode;
        }

        public ExitCode RegenerateProdReadModelIncrement()
        {
            var exitCode = ExitCode.Success;

            try
            {
                Console.WriteLine("Deploy: Production read model increment regeneration was started.");
                string backupFolder = ProdDeploymentHelper.RebuildProdReadModelIncrement();
                SaveBackupFolder(backupFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during production read model increment regeneration: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorRegenerateReadModel;
            }

            return exitCode;
        }

        public ExitCode CreateBackupPackageOfProd(out string backupPackage)
        {
            var exitCode = ExitCode.Success;
            backupPackage = String.Empty;
            try
            {
                backupPackage = ProdDeploymentHelper.CreateBackupPackageOfProd();

            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during building backup: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorCreateBackup;
            }

            return exitCode;
        }

        public ExitCode PublishAfterRegeneration()
        {
            var exitCode = ExitCode.Success;

            try
            {
                var backupFolder = ReadBackupFolder();
                if (Directory.Exists(backupFolder))
                {
                    ProdDeploymentHelper.RestoreDbPublishCopyIndexes(backupFolder);
                }
                else
                {
                    Console.WriteLine("Backup folder was not found.");
                    exitCode = ExitCode.ErrorPublish;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during app publishing: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorPublish;
            }

            return exitCode;
        }

        public ExitCode Publish()
        {
            var exitCode = ExitCode.Success;
            try
            {
                ProdDeploymentHelper.Publish();
            }
            catch (Exception e)
            {
                Console.WriteLine("Deploy: Error occured during app publishing: " + e + "/r/n" + e.StackTrace);
                exitCode = ExitCode.ErrorPublish;
            }

            return exitCode;
        }

        private static string GetConfigFilePath()
        {
            string path = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            var info = new DirectoryInfo(path);

            var fileName = Path.Combine(info.Parent.FullName, "backupPath.txt");
            return fileName;
        }

        private void SaveBackupFolder(string folder)
        {
            if (!File.Exists(GetConfigFilePath()))
            {
                using (File.Create(GetConfigFilePath())) { }
            }

            using (var sw = new StreamWriter(GetConfigFilePath()))
            {
                sw.WriteLine(folder);
            }
        }

        private string ReadBackupFolder()
        {
            var result = String.Empty;
            if (File.Exists(GetConfigFilePath()))
            {
                using (var sw = new StreamReader(GetConfigFilePath()))
                {
                    result = sw.ReadLine();
                }
            }

            return result;
        }
    }
}
