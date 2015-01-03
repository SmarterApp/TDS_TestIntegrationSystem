<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!--<xsl:strip-space elements="*"/>-->
  <xsl:output method="xml" version="1.0" indent="yes" encoding="utf-8" omit-xml-declaration="no" cdata-section-elements="response" />

  <xsl:param name="dateprocessed"></xsl:param>
  
  <xsl:template match="opportunity">
    <test projectID="{//test/@handscoreproject}" name="{//test/@name}" opportunityID="{@oppid}" subject="{//test/@subject}" grade="{//test/@grade}" status="{@status}" statusdate="{@statusdate}" datecompleted="{@datecompleted}" dateprocessed="{$dateprocessed}" operational="{//testee/@isdemo}">
      <xsl:apply-templates select="//testeerelationship" />
      <xsl:apply-templates select="segment" />
      <xsl:apply-templates select="item" />
    </test>
  </xsl:template>

  <xsl:template match="//testeerelationship">
    <xsl:if test="@name = 'SchoolID' and @value and @value != ''">
      <demographic context="{@context}" name="{@name}" value="{@value}" />
    </xsl:if>
  </xsl:template>

  <xsl:template match="segment">
    <xsl:if test="@formID and @formID != ''">
      <segment position="{@position}" formID="{@formID}" />
    </xsl:if>
  </xsl:template>

  <xsl:template match="item">
    <!-- only write out appealed items -->
    <xsl:if test="@scorestatus = 'APPEALED'">
      <item ID="{@airkey}" type="{@Format}" format="{@mimetype}" selected="{@IsSelected}" dropped="{@dropped}">
        <xsl:choose>
          <xsl:when test="./response">
            <response>
              <xsl:value-of select="./response" />
            </response>
          </xsl:when>
          <xsl:otherwise>
            <response>
              <xsl:value-of select="@response" />
            </response>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:apply-templates select="./scorerationale/ScoreInfo" />
      </item>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ScoreInfo">
    <xsl:copy-of select="."/>
  </xsl:template>

  <xsl:template match="comment">
    <!-- do not write out comments -->
  </xsl:template>

  <xsl:template match="testee">
    <!-- do not write out testee data unless explicitly directed above -->
  </xsl:template>
</xsl:stylesheet>
