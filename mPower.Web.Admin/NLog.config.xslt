<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="c"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:c="http://paralect.com/config">
  <xsl:template match="/">
    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <extensions>
        <add assembly="mPower.Framework"/>
      </extensions>
      <targets>
        <target name="MongoTarget" xsi:type="MongoTarget"/>
      </targets>
      <rules>
        <logger name="*" minLevel="Info" appendTo="MongoTarget"/>
      </rules>
    </nlog>
  </xsl:template>
</xsl:stylesheet>

