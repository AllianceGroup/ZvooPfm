using System;
using NLog.Config;
using StructureMap;

namespace mPower.DeployTool
{
    public class Program
    {
        static int Main(string[] args)
        {
            ExitCode exitCode;

            if (args.Length < 1)
            {
                exitCode = ExitCode.InvalidArgument;
            }
            else
            {

                ConfigurationItemFactory.Default.Targets.RegisterDefinition("MongoTarget", typeof(Framework.Mongo.NLogMongoTarget));
                Console.WriteLine("Deploy: Deploy tool started.");
                var container = InitializeContainer();
                var module = new DeployModule();
                container.BuildUp(module);

                var arg = args[0];
                switch (arg)
                {
                    case "regenerateStage":
                        Console.WriteLine("Deploy: Starting stage read model regeneration.");
                        exitCode = module.RegenerateStageReadModel();
                        Console.WriteLine("Deploy: Stage read model regeneration was finished.");
                        break;
                    case "regenerateProduction":
                        Console.WriteLine("Deploy: Starting production read model regeneration.");
                        exitCode = module.RegenerateProdReadModel();
                        Console.WriteLine("Deploy: Production read model regeneration was finished.");
                        break;
                    case "regenerateProductionIncrement":
                        Console.WriteLine("Deploy: Starting production read model increment regeneration.");
                        exitCode = module.RegenerateProdReadModelIncrement();
                        Console.WriteLine("Deploy: Production read model increment regeneration was finished.");
                        break;
                    case "backup":
                        Console.WriteLine("Deploy: Starting create backup package.");
                        string backupPackage;
                        exitCode = module.CreateBackupPackageOfProd(out backupPackage);
                        Console.WriteLine("Deploy: Backup package was created at: " + backupPackage);
                        break;
                    case "publishAfterRegeneration":
                        Console.WriteLine("Deploy: Starting publishing to production after read model regeneration.");
                        exitCode = module.PublishAfterRegeneration();
                        Console.WriteLine("Deploy: Publish to production was finished.");
                        break;
                    case "publish":
                        Console.WriteLine("Deploy: Publish to production started.");
                        exitCode = module.Publish();
                        Console.WriteLine("Deploy: Publish to production finished.");
                        break;
                    default:
                        exitCode = ExitCode.InvalidArgument;
                        break;
                }
            }

            return (int)exitCode;
        }

        private static Container InitializeContainer()
        {
            var container = new Container();
            new Web.Admin.Bootstrapper().BootstrapStructureMap(container);
            return container;
        }
    }
}
