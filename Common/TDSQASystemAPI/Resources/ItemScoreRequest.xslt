<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!--<xsl:strip-space elements="*"/>-->
  <xsl:output method="xml" version="1.0" indent="yes" encoding="utf-8" omit-xml-declaration="no" cdata-section-elements="Response Message" />
  
  <xsl:param name="callbackUrl"></xsl:param>

  <xsl:template match="/">
    <ItemScoreRequest callbackUrl="{$callbackUrl}">
      <xsl:apply-templates/>
    </ItemScoreRequest>
  </xsl:template>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>