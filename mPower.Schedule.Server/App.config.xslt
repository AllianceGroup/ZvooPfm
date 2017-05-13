<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="c"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:c="http://paralect.com/config">
  <xsl:template match="/">
    <configuration>
      <configSections>
        <sectionGroup name="common">
          <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
        </sectionGroup>
        <section name="quartz" type="System.Configuration.NameValueSectionHandler"/>
      </configSections>
      <quartz>
        <add key="mpower.schedule.server.serviceName" value="{c:Value('Mpower.Scheduler.ServiceName')}"/>
        <add key="mpower.schedule.server.serviceDisplayName" value="MPower Schedule Server {c:Value('Mpower.Scheduler.ServiceName')}"/>
        <add key="mpower.schedule.server.serviceDescription" value="Mpower Job Scheduling Server"/>
      </quartz>
      <appSettings>
        <xsl:value-of select="c:ApplicationSettings()" disable-output-escaping="yes" />
      </appSettings>
      <common>
        <logging>
          <factoryAdapter type="Common.Logging.NLog.NLogLoggerFactoryAdapter, Common.Logging.NLog">
            <arg key="configType" value="FILE" />
            <arg key="configFile" value="NLog.config" />
          </factoryAdapter>
        </logging>
      </common>

      <!-- 
    We use quartz.config for this server, you can always use configuration section if you want to.
    Configuration section has precedence here.  
  -->
      <!--
  <quartz >
  </quartz>
  -->
      <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
      </startup>

      <!--way to not send email on dev, stage and others environments, but store them in local folder-->
      <xsl:if test="c:Value('EnvironmentType') != 'Prod'">
        <system.net>
          <mailSettings>
            <smtp deliveryMethod="SpecifiedPickupDirectory">
              <network host="ignored" />
              <specifiedPickupDirectory pickupDirectoryLocation="{c:Value('Mpower.saveEmailsAtTestEnvironmentTo')}" />
            </smtp>
          </mailSettings>
        </system.net>
      </xsl:if>
    </configuration>
  </xsl:template>
</xsl:stylesheet>

