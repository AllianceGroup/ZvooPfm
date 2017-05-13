using System;
using Paralect.Config.Settings;
using MongoDB.Driver;
using System.IO;

namespace mPower.Framework
{
    public class MPowerSettings
    {
        
        [SettingsProperty("mPower.Mongo.ReadDatabaseConnectionString")]
        public string MongoReadDatabaseConnectionString { get; set; }

        public MongoUrl LocalReadMongoUrl
        {
            get { return MongoUrl.Create(MongoReadDatabaseConnectionString); }
        }

        public MongoUrl LocalWriteMongoUrl
        {
            get { return MongoUrl.Create(MongoWriteDatabaseConnectionString); }
        }

        public MongoUrl LocalYodleeMongoUrl { get { return MongoUrl.Create(MongoYodleeDatabaseConnectionString); } }


        [SettingsProperty("mPower.Mongo.WriteDatabaseConnectionString")]
        public string MongoWriteDatabaseConnectionString { get; set; }

        [SettingsProperty("mPower.Mongo.TempDatabaseConnectionString")]
        public string MongoTempDatabaseConnectionString { get; set; }

        [SettingsProperty("mPower.Mongo.YodleeDatabaseConnectionString")]
        public string MongoYodleeDatabaseConnectionString { get; set; }

        [SettingsProperty("mPower.Mongo.IntuitDatabaseConnectionString")]
        public string MongoIntuitDatabaseConnectionString { get; set; }

        [SettingsProperty("MPower.Mongo.TestReadDatabaseConnectionString")]
        public string MongoTestReadDatabaseConnectionString { get; set; }

        [SettingsProperty("MPower.Mongo.LogsCollectionName")]
        public string LogsCollectionName { get; set; }

        [SettingsProperty("MPower.Mongo.LogsDatabaseConnectionString")]
        public string LogsDatabaseConnectionString { get; set; }

        [SettingsProperty("MPower.Chargify.ApiKey")]
        public string ChargifyApiKey { get; set; }

        [SettingsProperty("MPower.Chargify.ApiPassword")]
        public string ChargifyApiPassword { get; set; }

        [SettingsProperty("MPower.Backup.PathToBackuper")]
        public string PathToBackuper { get; set; }

        [SettingsProperty("MPower.Backup.PathToRestorer")]
        public string PathToRestorer { get; set; }

        [SettingsProperty("MPower.Backup.Password")]
        public string BackupDbPassword { get; set; }

        [SettingsProperty("MPower.Backup.UserName")]
        public string BackupDbUserName { get; set; }

        [SettingsProperty("MPower.Mongo.ReadBackupFolder")]
        public string ReadBackupFolder { get; set; }

        [SettingsProperty("MPower.Mongo.WriteBackupFolder")]
        public string WriteBackupFolder { get; set; }

       
        
        [SettingsProperty("MPower.Mongo.YodleeBackupFolder")]
        public string YodleeBackupFolder { get; set; }

        [SettingsProperty("MPower.DefaultTenantAssembly")]
        public string DefaultTenantAssembly { get; set; }

        [SettingsProperty("MPower.JanrainApiBaseUrl")]
        public string JanrainApiBaseUrl { get; set; }

        [SettingsProperty("MPower.ZillowApiBaseUrl")]
        public string ZillowApiBaseUrl { get; set; }

        [SettingsProperty("MPower.ZillowWebServiceId")]
        public string ZillowWebServiceId { get; set; }

        /// <summary>
        /// For tests only, because of each tenant will have his own key 
        /// </summary>
        [SettingsProperty("MPower.Membership.ApiKey")]
        public string MembershipApiKey { get; set; }

        [SettingsProperty("MPower.Membership.BaseUrl")]
        public string MembershipBaseUrl { get; set; }

        [SettingsProperty("MPower.WebHashKey")]
        public string WebHashKey { get; set; }

        [SettingsProperty("EnvironmentType")]
        public string EnvironmentType { get; set; }

        public EnvironmentTypeEnum EnvironmentTypeEnum
        {
            get { return (EnvironmentTypeEnum)Enum.Parse(typeof(EnvironmentTypeEnum), EnvironmentType); }
        }

        [SettingsProperty("Mpower.HtmlToPdfToolPath")]
        public string HtmlToPdfToolPath { get; set; }

        [SettingsProperty("Mpower.DefaultEmailTemplatePath")]
        public string DefaultEmailTemplatePath { get; set; }

        [SettingsProperty("Mpower.NeverLengthInYears")]
        public string NeverLengthInYears { get; set; }

        [SettingsProperty("Mpower.WhiteEmailsList")]
        public string WhiteEmailsList { get; set; }

        [SettingsProperty("Mpower.WelcomeEventText")]
        public string WelcomeEventText { get; set; }

        // Queues' Names
        [SettingsProperty("Mpower.InputQueueName")]
        public string InputQueueName { get; set; }

        [SettingsProperty("Mpower.ErrorQueueName")]
        public string ErrorQueueName { get; set; }

        [SettingsProperty("Mpower.Scheduler.InputQueueName")]
        public string SchedulerInputQueueName { get; set; }

        [SettingsProperty("Mpower.Scheduler.ErrorQueueName")]
        public string SchedulerErrorQueueName { get; set; }

        [SettingsProperty("Mpower.LuceneIndexesDirectory")]
        public string LuceneIndexesDirectory { get; set; }

        [SettingsProperty("Mpower.SendToErrorEmails")]
        public string SendToErrorEmails { get; set; }

        [SettingsProperty]
        public DeploySettings Deploy { get; set; }

        [SettingsProperty("Mpower.UploadRootPath")]
        public string UploadRootPath { get; set; }
		
        [SettingsProperty("Mpower.AccessDataFtpUrl")]
        public string AccessDataFtpUrl { get; set; }

        [SettingsProperty("Mpower.AccessDataUsername")]
        public string AccessDataUsername { get; set; }

        [SettingsProperty("Mpower.AccessDataPassword")]
        public string AccessDataPassword { get; set; }

        [SettingsProperty("Mpower.AccessDataLocalPath")]
        public string AccessDataLocalPath { get; set; }

        [SettingsProperty("Mpower.SqlTempDatabase")]
        public string SqlTempDatabase { get; set; }

        public bool IsProd
        {
            get { return EnvironmentType == "Prod"; }
        }

        public bool IsStage
        {
            get { return EnvironmentType == "Stage"; }
        }
    }

    public class DeploySettings
    {
        #region Production Settings

        [SettingsProperty("Deploy.Prod.YodleeConnectionString")]
        public string ProdYodleeConnectionString { get; set; }

        public MongoUrl ProdYodleeMongoUrl
        {
            get { return MongoUrl.Create(ProdYodleeConnectionString); }
        }

        [SettingsProperty("Deploy.Prod.ReadModeUrl")]
        public string ProdReadModeUrl { get; set; }

        [SettingsProperty("Deploy.Prod.WriteConnectionString")]
        public string ProdWriteConnectionString { get; set; }

        public MongoUrl ProdWriteMongoUrl
        {
            get { return MongoUrl.Create(ProdWriteConnectionString); }
        }

        [SettingsProperty("Deploy.Prod.ReadConnectionString")]
        public string ProdReadConnectionString { get; set; }

        public MongoUrl ProdReadMongoUrl
        {
            get { return MongoUrl.Create(ProdReadConnectionString); }
        
        }

        [SettingsProperty("Deploy.Prod.IntuitConnectionString")]
        public string ProdIntuitConnectionString { get; set; }

        public MongoUrl ProdIntuitMongoUrl
        {
            get { return MongoUrl.Create(ProdIntuitConnectionString); }
        }

        [SettingsProperty("Deploy.Prod.ServerRootFolder")]
        public string ProdServerRootFolder { get; set; }

        #endregion

        #region Stage Settings

        [SettingsProperty("Deploy.Stage.BackupFolder")]
        public string StageBackupFolder { get; set; }

        [SettingsProperty("Deploy.Stage.PackagesFolder")]
        public string StagePackagesFolder { get; set; }


        [SettingsProperty("Deploy.Stage.PublisherPath")]
        public string StagePublisherPath { get; set; }

        [SettingsProperty("Deploy.Stage.ProdWebBuildFolder")]
        public string StageProdWebBuildFolder { get; set; }

        [SettingsProperty("Deploy.Stage.ServerRootFolder")]
        public string StageServerRootFolder { get; set; }

        public string StageReadBackupFolder
        {
            get { return Path.Combine(StageBackupFolder, "read"); }
        }

        public string StageWriteBackupFolder
        {
            get { return Path.Combine(StageBackupFolder, "write"); }
        }

        public string StageIntuitBackupFolder
        {
            get { return Path.Combine(StageBackupFolder, "intuit"); }
        }

        [SettingsProperty("Deploy.Stage.WriteConnectionString")]
        public string StageWriteConnectionString { get; set; }

        public MongoUrl StageWriteMongoUrl
        {
            get { return MongoUrl.Create(StageWriteConnectionString); }
        }


        [SettingsProperty("Deploy.Stage.ReadConnectionString")]
        public string StageReadConnectionString { get; set; }

        public MongoUrl StageReadMongoUrl
        {
            get { return MongoUrl.Create(StageReadConnectionString); }
        }


        [SettingsProperty("Deploy.Stage.IntuitConnectionString")]
        public string StageIntuitConnectionString { get; set; }

        public MongoUrl StageIntuitMongoUrl
        {
            get { return MongoUrl.Create(StageIntuitConnectionString); }
        }

        #endregion

        #region Production Regenerator Settings

        [SettingsProperty("Deploy.Regenerator.WriteConnectionString")]
        public string RegeneratorWriteConnectionString { get; set; }

        public MongoUrl RegeneratorWriteMongoUrl
        {
            get { return MongoUrl.Create(RegeneratorWriteConnectionString); }
        }

        [SettingsProperty("Deploy.Regenerator.ReadConnectionString")]
        public string RegeneratorReadConnectionString { get; set; }

        public MongoUrl RegeneratorReadMongoUrl
        {
            get { return MongoUrl.Create(RegeneratorReadConnectionString); }
        }

        [SettingsProperty("Deploy.Regenerator.LuceneIndexesDirectory")]
        public string RegeneratorLuceneIndexesDirectory { get; set; }

        #endregion

        [SettingsProperty("Deploy.RunPatchesBeforeRegeneration")]
        public string PatchesBeforeRegeneration { get; set; }
    }
}
