/****** Object:  Table [dbo].[ComputationRuleParameters]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComputationRuleParameters](
	[_Key] [uniqueidentifier] NOT NULL,
	[ComputationRule] [varchar](256) NOT NULL,
	[ParameterName] [varchar](128) NOT NULL,
	[ParameterPosition] [int] NOT NULL,
	[IndexType] [varchar](16) NULL,
	[Type] [varchar](16) NOT NULL,
 CONSTRAINT [PK_ComputationRuleParameters] PRIMARY KEY CLUSTERED 
(
	[_Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ComputationLocations]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComputationLocations](
	[Location] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[Location] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client](
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConversionTables]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ConversionTables](
	[TableName] [varchar](128) NOT NULL,
	[InValue] [int] NOT NULL,
	[OutValue] [float] NULL,
	[clientname] [varchar](100) NOT NULL,
 CONSTRAINT [PK_ConversionTables] PRIMARY KEY CLUSTERED 
(
	[TableName] ASC,
	[InValue] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConversionTableDesc]    Script Date: 01/16/2015 10:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ConversionTableDesc](
	[_Key] [varchar](36) NOT NULL,
	[TableName] [varchar](1000) NOT NULL,
	[_fk_Client] [varchar](100) NOT NULL,
 CONSTRAINT [pk_conversionTableDesc] PRIMARY KEY CLUSTERED 
(
	[_Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ComputationRules]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComputationRules](
	[Rulename] [varchar](255) NOT NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_ComputationRule] PRIMARY KEY CLUSTERED 
(
	[Rulename] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Test]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Test](
	[clientname] [varchar](100) NOT NULL,
	[testID] [varchar](150) NOT NULL,
	[_efk_Subject] [varchar](100) NULL,
	[_efk_StandardsPublication] [varchar](100) NULL,
	[ReportingInstrument] [varchar](25) NULL,
	[DoRRBInstrument] [varchar](50) NULL,
	[TIStoScore] [bit] NOT NULL,
 CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[testID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestScoreFeature]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestScoreFeature](
	[_Key] [uniqueidentifier] NOT NULL,
	[clientname] [varchar](100) NOT NULL,
	[TestID] [varchar](150) NOT NULL,
	[MeasureOf] [varchar](150) NOT NULL,
	[MeasureLabel] [varchar](150) NOT NULL,
	[IsScaled] [bit] NOT NULL,
	[ComputationRule] [varchar](255) NOT NULL,
	[ComputationOrder] [int] NOT NULL,
	[ReportingTransform] [varchar](32) NULL,
	[ReportingInstrument] [varchar](50) NULL,
	[ReportingGrade] [varchar](16) NULL,
	[ReportingSubject] [varchar](5) NULL,
	[ReportingScale] [varchar](255) NULL,
	[ReportingMeasureType] [varchar](25) NULL,
 CONSTRAINT [PK_Testscorefeature] PRIMARY KEY CLUSTERED 
(
	[_Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestGrades]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestGrades](
	[clientname] [varchar](100) NOT NULL,
	[TestID] [varchar](150) NOT NULL,
	[reportingGrade] [varchar](25) NOT NULL,
 CONSTRAINT [PK_TestGrades] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[TestID] ASC,
	[reportingGrade] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Feature_ComputationLocation]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Feature_ComputationLocation](
	[_fk_TestScoreFeature] [uniqueidentifier] NOT NULL,
	[Location] [varchar](50) NOT NULL,
 CONSTRAINT [PK_FeatureLocation] PRIMARY KEY CLUSTERED 
(
	[_fk_TestScoreFeature] ASC,
	[Location] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ComputationRuleParameterValue]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComputationRuleParameterValue](
	[_fk_TestScoreFeature] [uniqueidentifier] NOT NULL,
	[_fk_Parameter] [uniqueidentifier] NOT NULL,
	[Index] [varchar](256) NOT NULL,
	[Value] [varchar](256) NOT NULL,
 CONSTRAINT [PK_ComputationRuleParameterValue] PRIMARY KEY CLUSTERED 
(
	[_fk_TestScoreFeature] ASC,
	[_fk_Parameter] ASC,
	[Index] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Default [DF__Computatio___Key__014935CB]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[ComputationRuleParameters] ADD  CONSTRAINT [DF__Computatio___Key__014935CB]  DEFAULT (newid()) FOR [_Key]
GO
/****** Object:  Default [DF__Test__TIStoScore__797EC228]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[Test] ADD  CONSTRAINT [DF__Test__TIStoScore__797EC228]  DEFAULT ((0)) FOR [TIStoScore]
GO
/****** Object:  Default [DF__TestScoreF___Key__08EA5793]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[TestScoreFeature] ADD  CONSTRAINT [DF__TestScoreF___Key__08EA5793]  DEFAULT (newid()) FOR [_Key]
GO
/****** Object:  ForeignKey [FK_RuleParm_Rule]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[ComputationRuleParameterValue]  WITH CHECK ADD  CONSTRAINT [FK_RuleParm_Rule] FOREIGN KEY([_fk_Parameter])
REFERENCES [dbo].[ComputationRuleParameters] ([_Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ComputationRuleParameterValue] CHECK CONSTRAINT [FK_RuleParm_Rule]
GO
/****** Object:  ForeignKey [FK_RuleParm_ScoreFeatuer]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[ComputationRuleParameterValue]  WITH CHECK ADD  CONSTRAINT [FK_RuleParm_ScoreFeatuer] FOREIGN KEY([_fk_TestScoreFeature])
REFERENCES [dbo].[TestScoreFeature] ([_Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ComputationRuleParameterValue] CHECK CONSTRAINT [FK_RuleParm_ScoreFeatuer]
GO
/****** Object:  ForeignKey [FK_Feature_Location]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[Feature_ComputationLocation]  WITH CHECK ADD  CONSTRAINT [FK_Feature_Location] FOREIGN KEY([Location])
REFERENCES [dbo].[ComputationLocations] ([Location])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feature_ComputationLocation] CHECK CONSTRAINT [FK_Feature_Location]
GO
/****** Object:  ForeignKey [FK_Location_Feature]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[Feature_ComputationLocation]  WITH CHECK ADD  CONSTRAINT [FK_Location_Feature] FOREIGN KEY([_fk_TestScoreFeature])
REFERENCES [dbo].[TestScoreFeature] ([_Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feature_ComputationLocation] CHECK CONSTRAINT [FK_Location_Feature]
GO
/****** Object:  ForeignKey [FK_Grades_Test]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[TestGrades]  WITH CHECK ADD  CONSTRAINT [FK_Grades_Test] FOREIGN KEY([clientname], [TestID])
REFERENCES [dbo].[Test] ([clientname], [testID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestGrades] CHECK CONSTRAINT [FK_Grades_Test]
GO
/****** Object:  ForeignKey [FK_Client]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[TestScoreFeature]  WITH CHECK ADD  CONSTRAINT [FK_Client] FOREIGN KEY([clientname])
REFERENCES [dbo].[Client] ([name])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestScoreFeature] CHECK CONSTRAINT [FK_Client]
GO
/****** Object:  ForeignKey [FK_ScoreFeature_Rule]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[TestScoreFeature]  WITH CHECK ADD  CONSTRAINT [FK_ScoreFeature_Rule] FOREIGN KEY([ComputationRule])
REFERENCES [dbo].[ComputationRules] ([Rulename])
GO
ALTER TABLE [dbo].[TestScoreFeature] CHECK CONSTRAINT [FK_ScoreFeature_Rule]
GO
/****** Object:  ForeignKey [FK_Scorefeature_Test]    Script Date: 01/16/2015 11:44:40 ******/
ALTER TABLE [dbo].[TestScoreFeature]  WITH CHECK ADD  CONSTRAINT [FK_Scorefeature_Test] FOREIGN KEY([clientname], [TestID])
REFERENCES [dbo].[Test] ([clientname], [testID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TestScoreFeature] CHECK CONSTRAINT [FK_Scorefeature_Test]
GO
/****** Object:  ForeignKey [conversiontabledesc_client]    Script Date: 01/16/2015 10:52:03 ******/
ALTER TABLE [dbo].[ConversionTableDesc]  WITH CHECK ADD  CONSTRAINT [conversiontabledesc_client] FOREIGN KEY([_fk_Client])
REFERENCES [dbo].[Client] ([name])
GO
ALTER TABLE [dbo].[ConversionTableDesc] CHECK CONSTRAINT [conversiontabledesc_client]
GO
