﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="c"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:c="http://paralect.com/config">
  <xsl:template match="/">
    
    <configuration>
      
      <appSettings>
        <add key="webpages:Version" value="3.0.0.0"/>
        <add key="ClientValidationEnabled" value="true"/>
        <add key="UnobtrusiveJavaScriptEnabled" value="true"/>
        <add key="MPower.Mongo.ReadDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/mpower_read?authSource=admin" />
        <add key="MPower.Mongo.WriteDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/mpower_write?authSource=admin" />
        <add key="MPower.Mongo.YodleeDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/mpower_yodlee?authSource=admin" />
        <add key="MPower.Mongo.IntuitDatabaseConnectionString" value="mongodb://admin:admin@localhost:27020/mpower_intuit?authSource=admin" />
        <add key="MPower.Mongo.TempDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/mpower_temp?authSource=admin" />
        <add key="MPower.Mongo.TestReadDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/?authSource=admin" />
        <add key="MPower.Mongo.SessionServer" value="mongodb://admin(admin):admin@localhost:27017/" />
        <add key="MPower.Mongo.LogsDatabaseConnectionString" value="mongodb://admin:admin@localhost:27017/logs?authSource=admin" />
        <add key="MPower.Mongo.ReadBackupFolder" value="d:\temp\backup\read" />
        <add key="MPower.Mongo.WriteBackupFolder" value="d:\temp\backup\write" />
        <add key="MPower.Mongo.LogsCollectionName" value="logs" />
        <add key="MPower.Backup.PathToBackuper" value="C:\Program Files\MongoDB\Server\3.4\bin\mongodump.exe" />
        <add key="MPower.Backup.PathToRestorer" value="C:\Program Files\MongoDB\Server\3.4\bin\mongorestore.exe" />
        <add key="MPower.Backup.Password" value="admin" />
        <add key="MPower.Backup.UserName" value="admin" />
        <add key="MPower.Membership.ApiKey" value="F89DA1C7DC2B40319802BCF4F6B2C121" />
        <add key="MPower.Membership.BaseUrl" value="http://localhost:8080/api/membership" />
        <add key="MPower.DefaultTenantAssembly" value="Default, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
        <add key="MPower.Chargify.ApiKey" value="JuUlZ9aKklPOLf2hG3" />
        <add key="MPower.Chargify.ApiPassword" value="x" />
        <add key="MPower.JanrainApiBaseUrl" value="https://rpxnow.com/api/v2/" />
        <add key="MPower.ZillowApiBaseUrl" value="https://www.zillow.com/webservice/" />
        <add key="MPower.ZillowWebServiceId" value="X1-ZWz1d8296ljj7v_34dhf" />
        <add key="MPower.WebHashKey" value="W57tCwbjgVr8" />
        <add key="EnvironmentType" value="Dev" />
        <add key="Mpower.NeverLengthInYears" value="5" />
        <add key="Mpower.WelcomeEventText" value="Welcome! We've got you started with this sample event. Now it's your turn..." />
        <add key="Mpower.InputQueueName" value="mpower.server" />
        <add key="Mpower.ErrorQueueName" value="mpower.server.errors" />
        <add key="Mpower.Scheduler.InputQueueName" value="mpower.scheduler.server.local" />
        <add key="Mpower.Scheduler.ErrorQueueName" value="mpower.scheduler.server.local.errors" />
        <add key="Mpower.Scheduler.ServiceName" value="stage.mPower.Schedule.Server" />
        <add key="Mpower.LuceneIndexesDirectory" value="c:\temp\indexes" />
        <add key="Mpower.SendToErrorEmails" value="alex.shkor@paralect.com" />
        <add key="Mpower.UiTestsBaseUrl" value="http://localhost:8080" />
        <add key="Mpower.UploadRootPath" value="c:\temp\uploads" />
        <add key="Mpower.AccessDataUsername" value="savplat" />
        <add key="Mpower.AccessDataPassword" value="$4F6hzUnHgEg" />
        <add key="Mpower.AccessDataFtpUrl" value="ftp.adchosted.com" />
        <add key="Mpower.AccessDataLocalPath" value="c:\temp\access_data" />
        <add key="Mpower.SqlTempDatabase" value="Data Source=DESKTOP-2EKG2LE\SQLEXPRESS;Integrated Security=True" />
        <add key="Deploy.RunPatchesBeforeRegeneration" value="8" />
        <add key="Mpower.Tests.SendUiResultsTo" value="" />
        <add key="Mpower.saveEmailsAtTestEnvironmentTo" value="c:\temp\sentEmails" />
        <add key="IntuitEndpointUrl" value="https://ccqa.intuit.com/CustomerCentral/api" />
        <add key="PartnerId" value="32" />
        <add key="AdminId" value="mpowering_login" />
        <add key="AdminPassword" value="go" />
        <xsl:value-of select="c:ApplicationSettings()" disable-output-escaping="yes" />
      </appSettings>

      <connectionStrings>
        <add name="mPowerConnectionString" connectionString="Data Source=204.232.149.51;Initial Catalog=mPower;User ID=IISSQL;Password=2bdS4RpkLPz9;Network Library =dbmssocn;Connect Timeout=90" providerName="System.Data.SqlClient" />
        <add name="LegacyEntities" connectionString="metadata=res://*/Storage.LegacyDB.csdl|res://*/Storage.LegacyDB.ssdl|res://*/Storage.LegacyDB.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=204.232.149.51;initial catalog=mPower;user id=IISSQL;password=2bdS4RpkLPz9;connect timeout=90;network library=dbmssocn;multipleactiveresultsets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
		  <add name="LegacyCreditEntities" connectionString="metadata=res://*/Storage.LegacyCreditDB.csdl|res://*/Storage.LegacyCreditDB.ssdl|res://*/Storage.LegacyCreditDB.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=204.232.149.51;Initial Catalog=TUICCS;Persist Security Info=True;User ID=TUICCSIIS;Password=mPowerTUICCS002;multipleactiveresultsets=True&quot;" providerName="System.Data.EntityClient" />
      </connectionStrings>
      
      <system.web>
        <xsl:if test="c:Value('EnvironmentType') = 'Stage'">
          <identity impersonate="true" userName="remote_admin" password="mPowerWeb0mN1rPRw1aassdd1!@" />
        </xsl:if>
        <compilation debug="true" targetFramework="4.6">
          <assemblies>
            <add assembly="System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.Helpers, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.Mvc, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.WebPages, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
          </assemblies>
        </compilation>
        
        
        
        <pages>
          <namespaces>
            <add namespace="System.Web.Helpers" />
            <add namespace="System.Web.Mvc" />
            <add namespace="System.Web.Mvc.Ajax" />
            <add namespace="System.Web.Mvc.Html" />
            <add namespace="System.Web.Routing" />
            <add namespace="System.Web.WebPages" />
          </namespaces>
        </pages>
        
      </system.web>
      
      <system.webServer>
        <validation validateIntegratedModeConfiguration="false" />
        <modules runAllManagedModulesForAllRequests="true">
          <add name="ErrorLoggingModule" type="mPower.Framework.Modules.ErrorLoggingModule, mPower.Framework" />
        </modules>
      </system.webServer>
      
      <runtime>
        
        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
          <dependentAssembly>
            <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
            <bindingRedirect oldVersion="1.0.0.0-4.0.0.1" newVersion="4.0.0.0" />
          </dependentAssembly>
          <dependentAssembly>
            <assemblyIdentity name="Microsoft.Practices.ServiceLocation" publicKeyToken="31bf3856ad364e35" />
            <bindingRedirect oldVersion="1.0.0.0-4.0.0.1" newVersion="1.3.0.0" />
          </dependentAssembly>
        </assemblyBinding>
        
      </runtime>
      
    </configuration>    
    
  </xsl:template>
</xsl:stylesheet>

