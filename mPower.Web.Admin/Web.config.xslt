<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="c"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:c="http://paralect.com/config">
  <xsl:template match="/">
    
    <configuration>
      
      <appSettings>
        <add key="webpages:Version" value="2.0.0.0"/>
        <add key="ClientValidationEnabled" value="true"/>
        <add key="UnobtrusiveJavaScriptEnabled" value="true"/>
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
            <add assembly="System.Web.Helpers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            <add assembly="System.Web.WebPages, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
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

