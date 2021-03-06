/****** Object:  Table [dbo].[Client_TestMode]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestMode](
	[clientname] [varchar](100) NOT NULL,
	[testID] [varchar](200) NOT NULL,
	[mode] [varchar](50) NOT NULL,
	[algorithm] [varchar](50) NULL,
	[formTIDESelectable] [bit] NOT NULL,
	[isSegmented] [bit] NOT NULL,
	[maxopps] [int] NOT NULL,
	[requireRTSForm] [bit] NOT NULL,
	[requireRTSFormWindow] [bit] NOT NULL,
	[requireRTSformIfExists] [bit] NOT NULL,
	[sessionType] [int] NOT NULL,
	[testkey] [varchar](250) NULL,
	[_key] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Client_TestMode] PRIMARY KEY CLUSTERED 
(
	[_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ClientTestmode] ON [dbo].[Client_TestMode] 
(
	[clientname] ASC,
	[testkey] ASC,
	[sessionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TestMode] ON [dbo].[Client_TestMode] 
(
	[clientname] ASC,
	[testID] ASC,
	[sessionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Testmode_Test] ON [dbo].[Client_TestMode] 
(
	[testID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TestModeKey] ON [dbo].[Client_TestMode] 
(
	[testkey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestGrades]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestGrades](
	[clientname] [varchar](100) NOT NULL,
	[TestID] [varchar](150) NOT NULL,
	[grade] [varchar](25) NOT NULL,
	[RequireEnrollment] [bit] NOT NULL,
	[EnrolledSubject] [varchar](100) NULL,
 CONSTRAINT [PK_TestGrades] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[TestID] ASC,
	[grade] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Testgrade_Test] ON [dbo].[Client_TestGrades] 
(
	[TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestformProperties]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestformProperties](
	[clientname] [varchar](100) NOT NULL,
	[_efk_TestForm] [varchar](50) NOT NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[Language] [varchar](25) NOT NULL,
	[FormID] [varchar](200) NULL,
	[TestID] [varchar](150) NOT NULL,
	[testkey] [varchar](250) NULL,
	[clientFormID] [varchar](25) NULL,
	[accommodations] [varchar](max) NULL,
 CONSTRAINT [PK_Testform] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[_efk_TestForm] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Form_Testkey] ON [dbo].[Client_TestformProperties] 
(
	[testkey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestEligibility]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestEligibility](
	[_key] [uniqueidentifier] NOT NULL,
	[Clientname] [varchar](100) NOT NULL,
	[TestID] [varchar](150) NOT NULL,
	[RTSName] [varchar](100) NOT NULL,
	[enables] [bit] NOT NULL,
	[disables] [bit] NOT NULL,
	[RTSValue] [varchar](400) NOT NULL,
	[_efk_entityType] [bigint] NOT NULL,
	[eligibilityType] [varchar](50) NULL,
	[matchType] [int] NOT NULL,
 CONSTRAINT [PK_Client_TestEligibility] PRIMARY KEY CLUSTERED 
(
	[_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_TestEligibility] ON [dbo].[Client_TestEligibility] 
(
	[Clientname] ASC,
	[TestID] ASC,
	[enables] ASC,
	[RTSName] ASC,
	[RTSValue] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TesteeRelationshipAttribute]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TesteeRelationshipAttribute](
	[clientname] [varchar](50) NOT NULL,
	[RelationshipType] [varchar](50) NOT NULL,
	[RTSName] [varchar](50) NOT NULL,
	[TDS_ID] [varchar](50) NOT NULL,
	[Label] [varchar](50) NULL,
	[atLogin] [varchar](25) NULL,
	[sortOrder] [int] NULL,
	[reportName] [varchar](50) NULL,
 CONSTRAINT [PK_Client_TesteeRelationshipAttribute] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[TDS_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_TesteeAttribute]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TesteeAttribute](
	[RTSName] [varchar](50) NOT NULL,
	[TDS_ID] [varchar](50) NOT NULL,
	[clientname] [varchar](100) NOT NULL,
	[reportName] [varchar](100) NULL,
	[type] [varchar](50) NOT NULL,
	[Label] [varchar](50) NULL,
	[atLogin] [varchar](25) NULL,
	[sortOrder] [int] NULL,
	[isLatencySite] [bit] NOT NULL,
	[showOnProctor] [bit] NULL,
 CONSTRAINT [PK_TesteeAttribute] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[TDS_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_Test_Itemtypes]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_Test_Itemtypes](
	[ClientName] [varchar](100) NOT NULL,
	[TestID] [varchar](255) NOT NULL,
	[Itemtype] [varchar](25) NOT NULL,
	[origin] [varchar](50) NULL,
	[source] [varchar](50) NULL,
 CONSTRAINT [PK_Itemtypes] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[TestID] ASC,
	[Itemtype] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_Test_ItemConstraint]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_Test_ItemConstraint](
	[ClientName] [varchar](100) NOT NULL,
	[TestID] [varchar](255) NOT NULL,
	[Propname] [varchar](100) NOT NULL,
	[PropValue] [varchar](100) NOT NULL,
	[ToolType] [nvarchar](255) NOT NULL,
	[ToolValue] [nvarchar](255) NOT NULL,
	[item_in] [bit] NOT NULL,
 CONSTRAINT [PK_ItemConstraint] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[TestID] ASC,
	[Propname] ASC,
	[PropValue] ASC,
	[item_in] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_Subject]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_Subject](
	[SubjectCode] [varchar](25) NULL,
	[Subject] [varchar](100) NOT NULL,
	[ClientName] [varchar](100) NOT NULL,
	[origin] [varchar](50) NULL,
 CONSTRAINT [PK_Client_Subject_1] PRIMARY KEY CLUSTERED 
(
	[Subject] ASC,
	[ClientName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_SegmentProperties]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_SegmentProperties](
	[IsPermeable] [int] NOT NULL,
	[clientname] [varchar](100) NOT NULL,
	[entryApproval] [int] NOT NULL,
	[exitApproval] [int] NOT NULL,
	[itemReview] [bit] NOT NULL,
	[segmentID] [varchar](255) NOT NULL,
	[segmentPosition] [int] NOT NULL,
	[parentTest] [varchar](255) NOT NULL,
	[FTStartDate] [datetime] NULL,
	[FTEndDate] [datetime] NULL,
	[Label] [varchar](255) NULL,
	[modekey] [varchar](250) NULL,
	[restart] [int] NULL,
	[gracePeriodMinutes] [int] NULL,
 CONSTRAINT [PK_SegmentProps] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[parentTest] ASC,
	[segmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_Language]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_Language](
	[ClientName] [varchar](100) NOT NULL,
	[Language] [varchar](100) NOT NULL,
	[LanguageCode] [varchar](25) NOT NULL,
	[origin] [varchar](50) NULL,
 CONSTRAINT [PK_Client_Language] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[LanguageCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_Grade]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_Grade](
	[GradeCode] [varchar](25) NOT NULL,
	[Grade] [varchar](64) NOT NULL,
	[ClientName] [varchar](100) NOT NULL,
	[origin] [varchar](50) NULL,
 CONSTRAINT [PK_Client_Grade] PRIMARY KEY CLUSTERED 
(
	[GradeCode] ASC,
	[ClientName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_FieldtestPriority]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_FieldtestPriority](
	[TDS_ID] [varchar](50) NOT NULL,
	[clientname] [varchar](100) NOT NULL,
	[priority] [int] NOT NULL,
	[testID] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ClientFT] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[testID] ASC,
	[TDS_ID] ASC,
	[priority] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_AccommodationFamily]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_AccommodationFamily](
	[clientname] [varchar](100) NOT NULL,
	[family] [varchar](50) NOT NULL,
	[label] [varchar](200) NOT NULL,
 CONSTRAINT [PK_AccomFamily] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[family] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client](
	[Name] [varchar](100) NOT NULL,
	[origin] [varchar](50) NULL,
	[internationalize] [bit] NOT NULL,
	[defaultLanguage] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_TestscoreFeatures]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestscoreFeatures](
	[ClientName] [varchar](100) NOT NULL,
	[TestID] [varchar](255) NOT NULL,
	[MeasureOf] [varchar](250) NOT NULL,
	[MeasureLabel] [varchar](200) NOT NULL,
	[ReportToStudent] [bit] NOT NULL,
	[ReportToProctor] [bit] NOT NULL,
	[ReportToParticipation] [bit] NOT NULL,
	[UseForAbility] [bit] NOT NULL,
	[ReportLabel] [varchar](max) NULL,
	[ReportOrder] [int] NULL,
 CONSTRAINT [PK_ScoreFeatures] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[TestID] ASC,
	[MeasureOf] ASC,
	[MeasureLabel] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_TestToolType]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestToolType](
	[ClientName] [varchar](100) NOT NULL,
	[ToolName] [nvarchar](255) NOT NULL,
	[AllowChange] [bit] NOT NULL,
	[TIDESelectable] [bit] NULL,
	[RTSFieldName] [nvarchar](100) NULL,
	[IsRequired] [bit] NOT NULL,
	[TIDESelectableBySubject] [bit] NOT NULL,
	[IsSelectable] [bit] NOT NULL,
	[IsVisible] [bit] NOT NULL,
	[StudentControl] [bit] NOT NULL,
	[ToolDescription] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL,
	[dateEntered] [datetime] NOT NULL,
	[origin] [varchar](50) NULL,
	[source] [varchar](50) NULL,
	[ContextType] [varchar](50) NOT NULL,
	[Context] [varchar](255) NOT NULL,
	[DependsOnToolType] [varchar](50) NULL,
	[DisableOnGuestSession] [bit] NOT NULL,
	[IsFunctional] [bit] NOT NULL,
	[TestMode] [varchar](25) NOT NULL,
 CONSTRAINT [PK_ClientTestTool] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[Context] ASC,
	[ContextType] ASC,
	[ToolName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[_BuildTable]    Script Date: 01/16/2015 08:56:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--description: build a table from a delimiter list
--example: select * from [dbo]._BuildTable('a', '|')
CREATE FUNCTION [dbo].[_BuildTable](@list nvarchar(MAX),@delimiter varchar(5))
returns @tbl  TABLE (idx int IDENTITY(1,1) PRIMARY KEY, record varchar(255))
as
begin 
	declare
		@I int, 			-- Current place in delimited string
		@StrLen int, 		-- Length of delimited string
		@StrStart int, 		-- Start point of current record
		@record varchar (255),	-- Current record
		@DelLen int -- delimter length	

	set @DelLen = len(@delimiter)	
	--make sure the list does not ends with a delimiter
	if right(@list,@DelLen) = @delimiter
		set @list=substring(@list, 1, len(@list) - @DelLen)
	set @I = 1
	set @StrLen = len(@list)
	--Insert strings separated by delimiter as individual records
	while (@I <= @StrLen)
	begin
 		set @StrStart = (select charindex(@delimiter, @list, @I))	
		if (@StrStart < @I)	begin
			set @record=substring(@list, @I, @StrLen)	
			set @I = @StrLen + @DelLen
		end
		else begin
 			set @record=substring(@list, @I, (@StrStart) - @I)
			set @I = @StrStart + @DelLen
		end
		insert into @tbl (record)
			select @record
	end	
	return
end
GO
/****** Object:  Table [dbo].[TDS_TestToolType]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TDS_TestToolType](
	[ToolName] [nvarchar](255) NOT NULL,
	[AllowChange] [bit] NOT NULL,
	[TIDESelectable] [bit] NULL,
	[RTSFieldName] [nvarchar](100) NULL,
	[IsRequired] [bit] NOT NULL,
	[TIDESelectableBySubject] [bit] NOT NULL,
 CONSTRAINT [PK_ToolType] PRIMARY KEY CLUSTERED 
(
	[ToolName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDS_TestTool]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TDS_TestTool](
	[Type] [nvarchar](255) NOT NULL,
	[Code] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[ValueDescription] [nvarchar](255) NULL,
	[AllowCombine] [bit] NOT NULL,
	[AllowAdd] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_TestTool] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDS_TesteeRelationshipAttribute]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TDS_TesteeRelationshipAttribute](
	[RelationshipType] [varchar](50) NOT NULL,
	[RTSName] [varchar](50) NOT NULL,
	[TDS_ID] [varchar](50) NOT NULL,
	[Label] [varchar](50) NULL,
	[atLogin] [varchar](25) NULL,
	[sortOrder] [int] NULL,
	[reportName] [varchar](50) NULL,
 CONSTRAINT [PK_TDS_TesteeRelationshipAttribute] PRIMARY KEY CLUSTERED 
(
	[TDS_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TDS_TesteeAttribute]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TDS_TesteeAttribute](
	[RTSName] [varchar](50) NOT NULL,
	[TDS_ID] [varchar](50) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[Label] [varchar](50) NULL,
	[atLogin] [varchar](25) NULL,
	[sortOrder] [int] NULL,
	[reportName] [varchar](50) NULL,
	[isLatencySite] [bit] NOT NULL,
	[showOnProctor] [bit] NULL,
 CONSTRAINT [PK_TDS_TesteeAttribute] PRIMARY KEY CLUSTERED 
(
	[TDS_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TDS_FieldtestPriority]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TDS_FieldtestPriority](
	[TDS_ID] [varchar](50) NOT NULL,
	[priority] [int] NOT NULL,
 CONSTRAINT [PK_TDS_FTAttribute] PRIMARY KEY CLUSTERED 
(
	[TDS_ID] ASC,
	[priority] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[SubjectCodes]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SubjectCodes]
as
select 'Mathematics' as Subject, 'MA' as SubjectCode 
union select  'Reading' as Subject, 'RE' as SubjectCode 
union select  'Science' as Subject, 'SC' as SubjectCode 
union select  'Social Sciences' as Subject, 'SS' as SubjectCode 
union select  'Social Studies' as Subject, 'SS' as SubjectCode 
union select  'ELA' as Subject, 'EL' as SubjectCode 
union select  'Writing' as Subject, 'WR' as SubjectCode 
union select  'ELPA Speaking' as Subject, 'ELPA' as SubjectCode 
union select  'ELPA' as Subject, 'ELPA' as SubjectCode
GO
/****** Object:  Table [dbo].[Client_TimeWindow]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TimeWindow](
	[clientname] [varchar](100) NOT NULL,
	[windowID] [varchar](50) NOT NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[description] [varchar](200) NULL,
 CONSTRAINT [PK_TimeWindow] PRIMARY KEY CLUSTERED 
(
	[clientname] ASC,
	[windowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Client_TimeLimits]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TimeLimits](
	[_key] [uniqueidentifier] NOT NULL,
	[_efk_TestID] [nvarchar](255) NULL,
	[OppExpire] [int] NOT NULL,
	[OppRestart] [int] NOT NULL,
	[OppDelay] [int] NOT NULL,
	[InterfaceTimeout] [int] NULL,
	[ClientName] [varchar](100) NULL,
	[IsPracticeTest] [bit] NOT NULL,
	[RefreshValue] [int] NULL,
	[TAInterfaceTimeout] [int] NULL,
	[TACheckInTime] [int] NULL,
	[DateChanged] [datetime] NULL,
	[DatePublished] [datetime] NULL,
	[environment] [varchar](100) NULL,
	[sessionExpire] [int] NOT NULL,
	[requestInterfaceTimeout] [int] NOT NULL,
	[RefreshValueMultiplier] [int] NOT NULL,
 CONSTRAINT [PK_Client_TimeLimits] PRIMARY KEY CLUSTERED 
(
	[_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ClientTimeLimits] ON [dbo].[Client_TimeLimits] 
(
	[ClientName] ASC,
	[_efk_TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestWindow]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestWindow](
	[clientname] [varchar](100) NOT NULL,
	[TestID] [varchar](200) NOT NULL,
	[window] [int] NOT NULL,
	[numopps] [int] NOT NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[origin] [varchar](100) NOT NULL,
	[source] [varchar](100) NOT NULL,
	[windowID] [varchar](50) NULL,
	[_Key] [uniqueidentifier] NOT NULL,
	[sessionType] [int] NOT NULL,
	[sortOrder] [int] NULL,
 CONSTRAINT [PK_TestWindow] PRIMARY KEY CLUSTERED 
(
	[_Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ClientTestWindow] ON [dbo].[Client_TestWindow] 
(
	[clientname] ASC,
	[TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Testwindow_Test] ON [dbo].[Client_TestWindow] 
(
	[TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestTool]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestTool](
	[ClientName] [varchar](100) NOT NULL,
	[Type] [nvarchar](255) NOT NULL,
	[Code] [nvarchar](255) NOT NULL,
	[Value] [varchar](128) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[AllowCombine] [bit] NOT NULL,
	[ValueDescription] [nvarchar](255) NULL,
	[Context] [varchar](255) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[origin] [varchar](50) NULL,
	[source] [varchar](50) NULL,
	[ContextType] [varchar](50) NOT NULL,
	[TestMode] [varchar](25) NOT NULL,
	[EquivalentClientCode] [nvarchar](255) NULL,
 CONSTRAINT [PK_Client_ToolValues] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[Context] ASC,
	[ContextType] ASC,
	[Type] ASC,
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ClientToolTestID] ON [dbo].[Client_TestTool] 
(
	[Context] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client_TestProperties]    Script Date: 01/16/2015 08:56:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Client_TestProperties](
	[ClientName] [varchar](100) NOT NULL,
	[TestID] [varchar](255) NOT NULL,
	[MaxOpportunities] [int] NULL,
	[HandScoreProject] [int] NULL,
	[Prefetch] [int] NOT NULL,
	[DateChanged] [datetime] NULL,
	[IsPrintable] [bit] NOT NULL,
	[IsSelectable] [bit] NOT NULL,
	[Label] [varchar](255) NULL,
	[PrintItemTypes] [varchar](1000) NULL,
	[ScoreByTDS] [bit] NOT NULL,
	[BatchModeReport] [bit] NOT NULL,
	[SubjectName] [varchar](100) NULL,
	[origin] [varchar](50) NULL,
	[source] [varchar](50) NULL,
	[maskItemsBySubject] [bit] NOT NULL,
	[initialAbilityBySubject] [bit] NOT NULL,
	[FTStartDate] [datetime] NULL,
	[FTEndDate] [datetime] NULL,
	[AccommodationFamily] [varchar](50) NULL,
	[SortOrder] [int] NULL,
	[RTSFormField] [varchar](50) NOT NULL,
	[RTSWindowField] [varchar](50) NOT NULL,
	[WindowTIDESelectable] [bit] NOT NULL,
	[requireRTSWindow] [bit] NOT NULL,
	[ReportingInstrument] [varchar](50) NULL,
	[TIDE_ID] [varchar](100) NULL,
	[forceComplete] [bit] NOT NULL,
	[RTSModeField] [varchar](50) NOT NULL,
	[modeTIDESelectable] [bit] NOT NULL,
	[requireRTSMode] [bit] NOT NULL,
	[requireRTSmodeWindow] [bit] NOT NULL,
	[deleteUnansweredItems] [bit] NOT NULL,
	[abilitySlope] [float] NOT NULL,
	[abilityIntercept] [float] NOT NULL,
	[validateCompleteness] [bit] NOT NULL,
	[GradeText] [varchar](50) NULL,
	[proctorEligibility] [int] NOT NULL,
	[Category] [varchar](50) NULL,
 CONSTRAINT [PK_Client_TestProperties] PRIMARY KEY CLUSTERED 
(
	[ClientName] ASC,
	[TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Testprops_Test] ON [dbo].[Client_TestProperties] 
(
	[TestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[TIDETestWindows]    Script Date: 01/16/2015 08:56:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TIDETestWindows]
as
select distinct W.clientname,T.ReportingInstrument, T.Subjectname as subject, T.TIDE_ID, 
                WindowID,  T.RTSWindowField as RTSFieldName, WindowTIDESelectable as IsSelectable, W.sortOrder, 
				T.AccommodationFamily, AF.Label AS AccommodationFamilyLabel
from Client_TestWindow W, Client_TestProperties T, client_accommodationfamily AF
where  W.clientname = T.Clientname and W.TestID = T.TestID AND T.ClientName=AF.ClientName and T.AccommodationFamily=AF.family and (requireRTSWindow = 1 )
AND T.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.
GO
/****** Object:  View [dbo].[TIDETests]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[TIDETests]
as
select distinct G.clientname, G.TestID, T.TIDE_ID, T.SubjectName, G.grade, 
				T.AccommodationFamily, AF.Label AS AccommodationFamilyLabel
from Client_TestProperties T, Client_TestGrades G, client_accommodationfamily AF
where T.clientname = G.clientname and T.TestID = G.TestID AND T.ClientName=AF.ClientName and T.AccommodationFamily=AF.family
AND T.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.
GO
/****** Object:  View [dbo].[TIDETestModes]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TIDETestModes]
as
select distinct M.clientname, T.ReportingInstrument, T.subjectname as subject, T.TIDE_ID, T.Label, M.Mode, RTSModeField as RTSFieldName, ModeTIDESelectable as IsSelectable,
    requireRTSMode, requireRTSmodeWindow, T.AccommodationFamily, AF.Label AS AccommodationFamilyLabel
from Client_TestMode M, Client_TestProperties T, client_accommodationfamily AF
where M.clientname = T.clientname and M.testID = T.TestID and TIDE_ID is not NULL AND T.ClientName=AF.ClientName and T.AccommodationFamily=AF.family
AND T.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.
GO
/****** Object:  View [dbo].[TIDETestForms]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TIDETestForms]
as
select distinct F.clientname, T.ReportingInstrument, T.subjectname as subject, T.TestID, T.Label, T.TIDE_ID, FormID, _efk_TestForm as FormKey, Language, 
        RTSFormField as RTSFieldName, M.FormTIDESelectable as IsSelectable, requireRTSFormWindow, clientFormID, accommodations, T.AccommodationFamily, 
		AF.Label AS AccommodationFamilyLabel
from Client_TestformProperties F, Client_TestProperties T, Client_TestMode M , client_accommodationfamily AF
where --(RequireRTSForm = 1 or requireRTSFormWindow = 1 or requireRTSformIfExists = 1) and  --removed per jeremy requested 1/09/2012
F.clientname = T.clientname and F.TestID = T.TestID
and M.clientname = T.clientname and M.TestID = T.testID
AND T.ClientName=AF.ClientName and T.AccommodationFamily=AF.family
AND T.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.
GO
/****** Object:  Trigger [TestDelete]    Script Date: 01/16/2015 08:56:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hoai-Anh Ngo
-- Create date: <Create Date,,>
-- Description:	Remove orphan tests config from other tables
-- =============================================
CREATE TRIGGER [dbo].[TestDelete] ON  [dbo].[Client_TestProperties]
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	delete from Client_TimeLimits where exists 
		(select * from Deleted where ClientName=Client_TimeLimits.ClientName and TestID=Client_TimeLimits._efk_TestID);	
	
	delete from dbo.Client_Test_Itemtypes where exists 
		(select * from Deleted where ClientName=Client_Test_Itemtypes.ClientName and TestID=Client_Test_Itemtypes.TestID);	

	delete from dbo.Client_PilotSchools where exists 
		(select * from Deleted where ClientName=Client_PilotSchools.ClientName and TestID=Client_PilotSchools._efk_TestID);	
	
	delete from dbo.Client_TestTool where exists 
		(select * from Deleted where ClientName=Client_TestTool.ClientName and ContextType = 'TEST' and TestID=Client_TestTool.Context);	
	
	delete from dbo.Client_TestToolType where exists 
		(select * from Deleted where ClientName=Client_TestToolType.ClientName and ContextType = 'TEST' and TestID=Client_TestToolType.Context);
	
	delete from dbo.Client_TestScoreFeatures where exists 
		(select * from Deleted where ClientName=Client_TestScoreFeatures.ClientName and TestID=Client_TestScoreFeatures.TestID);	

    delete from dbo.Client_FieldTestPriority where exists 
		(select * from Deleted where ClientName=Client_FieldTestPriority.ClientName and TestID=Client_FieldTestPriority.TestID);	

    delete from dbo.Client_SegmentProperties where exists 
		(select * from Deleted where ClientName=Client_SegmentProperties.ClientName and TestID=Client_SegmentProperties.parentTest);	

    delete from dbo.Client_TestWindow where exists 
		(select * from Deleted where ClientName=Client_TestWindow.ClientName and TestID=Client_TestWindow.TestID);	

    delete from dbo.Client_TestMode where exists
        (select * from Deleted where clientname = Client_TestMode.Clientname and TestID = CLient_TestMode.TestID);

    delete from dbo.Client_TestformProperties where exists
        (select * from Deleted where ClientName=Client_TestformProperties.ClientName and TestID=Client_TestformProperties.TestID);	

    delete from dbo.Client_TestGrades where exists
        (select * from Deleted where Clientname=Client_TestGrades.clientname and testID = Client_TestGrades.TestID);

end
GO
/****** Object:  View [dbo].[AccommodationFamilyValues]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AccommodationFamilyValues] as
select distinct T.Clientname, Family as FamilyKey, F.Label as FamilyName, ToolName, RTSFieldName, Code, Value, IsDefault, AllowCombine, L.SortOrder, L.TestMode,L.EquivalentClientCode
from Client_TestProperties P, Client_TestToolType T, Client_AccommodationFamily F, Client_TestTool L
where P.clientname =  T.clientname  and F.clientname = P.clientname and L.ClientName = T.CLientname 
and T.contextType = 'TEST' and (T.Context = P.TestID or T.Context = '*')
and T.TIDESelectable = 1 and  P.TestID not like '%FAKEFORM%'
and L.Type = T.ToolName and L.ContextType = 'TEST' and (L.Context = P.TestID or L.Context = '*')
and AccommodationFamily is not null and P.AccommodationFamily = Family
AND P.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.

UNION

select distinct T.Clientname, Family as FamilyKey, F.Label as FamilyName, ToolName, RTSFieldName, Code, Value, IsDefault, AllowCombine, L.SortOrder, L.TestMode,L.EquivalentClientCode
from Client_TestToolType T, Client_AccommodationFamily F, Client_TestTool L
where F.clientname = T.clientname and L.ClientName = T.CLientname 
and T.contextType = 'FAMILY' and T.Context = F.Family
and T.TIDESelectable = 1 
and L.Type = T.ToolName and L.ContextType = 'FAMILY' and L.Context = F.Family
GO
/****** Object:  View [dbo].[AccommodationFamilies]    Script Date: 01/16/2015 08:56:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AccommodationFamilies] as
select distinct T.Clientname, Family as FamilyKey, F.Label as FamilyName, ToolName, RTSFieldName, Code, Value, IsDefault, AllowCombine, L.TestMode,L.EquivalentClientCode
from Client_TestProperties P, Client_TestToolType T, Client_AccommodationFamily F, Client_TestTool L
where P.clientname =  T.clientname  and F.clientname = P.clientname and L.ClientName = T.CLientname 
and T.contextType = 'TEST' and (T.Context = P.TestID or T.Context = '*')
and T.TIDESelectable = 1 and P.IsSelectable = 1 and P.TestID not like '%FAKEFORM%'
and L.Type = T.ToolName and L.ContextType = 'TEST' and (L.Context = P.TestID or L.Context = '*')
and AccommodationFamily is not null and P.AccommodationFamily = Family
AND P.TIDE_ID IS NOT NULL --JF: Added at Shailesh's request to hide unneeded data from TIDE.  Approved by Larry.

UNION

select distinct T.Clientname, Family as FamilyKey, F.Label as FamilyName, ToolName, RTSFieldName, Code, Value, IsDefault, AllowCombine, L.TestMode,L.EquivalentClientCode
from Client_TestToolType T, Client_AccommodationFamily F, Client_TestTool L
where F.clientname = T.clientname and L.ClientName = T.CLientname 
and T.contextType = 'FAMILY' and T.Context = F.Family
and T.TIDESelectable = 1 
and L.Type = T.ToolName and L.ContextType = 'FAMILY' and L.Context = F.Family
GO
/****** Object:  Default [DF__Client__internat__0EF836A4]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client] ADD  CONSTRAINT [DF__Client__internat__0EF836A4]  DEFAULT ((1)) FOR [internationalize]
GO
/****** Object:  Default [DF__Client__defaultL__0FEC5ADD]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client] ADD  CONSTRAINT [DF__Client__defaultL__0FEC5ADD]  DEFAULT ('ENU') FOR [defaultLanguage]
GO
/****** Object:  Default [DF__Client_Gr__origi__5D56B96F]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_Grade] ADD  CONSTRAINT [DF__Client_Gr__origi__5D56B96F]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_La__origi__5F3F01E1]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_Language] ADD  CONSTRAINT [DF__Client_La__origi__5F3F01E1]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Se__itemR__793DFFAF]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_SegmentProperties] ADD  CONSTRAINT [DF__Client_Se__itemR__793DFFAF]  DEFAULT ((1)) FOR [itemReview]
GO
/****** Object:  Default [DF__Client_Su__origi__61274A53]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_Subject] ADD  CONSTRAINT [DF__Client_Su__origi__61274A53]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__origi__5A7A4CC4]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_Test_Itemtypes] ADD  CONSTRAINT [DF__Client_Te__origi__5A7A4CC4]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__sourc__5B6E70FD]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_Test_Itemtypes] ADD  CONSTRAINT [DF__Client_Te__sourc__5B6E70FD]  DEFAULT (db_name()) FOR [source]
GO
/****** Object:  Default [DF__Client_Te__isLat__4C0144E4]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TesteeAttribute] ADD  CONSTRAINT [DF__Client_Te__isLat__4C0144E4]  DEFAULT ((0)) FOR [isLatencySite]
GO
/****** Object:  Default [DF__Client_Te__showO__1411F17C]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TesteeAttribute] ADD  CONSTRAINT [DF__Client_Te__showO__1411F17C]  DEFAULT ((1)) FOR [showOnProctor]
GO
/****** Object:  Default [DF__Client_Tes___key__34E8D562]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestEligibility] ADD  CONSTRAINT [DF__Client_Tes___key__34E8D562]  DEFAULT (newid()) FOR [_key]
GO
/****** Object:  Default [DF__Client_Te__match__36D11DD4]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestEligibility] ADD  CONSTRAINT [DF__Client_Te__match__36D11DD4]  DEFAULT ((0)) FOR [matchType]
GO
/****** Object:  Default [DF__Client_Te__Requi__1209AD79]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestGrades] ADD  CONSTRAINT [DF__Client_Te__Requi__1209AD79]  DEFAULT ((0)) FOR [RequireEnrollment]
GO
/****** Object:  Default [DF_dbo_Client_TestMode_mode]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF_dbo_Client_TestMode_mode]  DEFAULT ('online') FOR [mode]
GO
/****** Object:  Default [DF__Client_Te__formT__4E1E9780]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__formT__4E1E9780]  DEFAULT ((0)) FOR [formTIDESelectable]
GO
/****** Object:  Default [DF__Client_Te__isSeg__4B422AD5]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__isSeg__4B422AD5]  DEFAULT ((0)) FOR [isSegmented]
GO
/****** Object:  Default [DF__Client_Te__maxop__3B0BC30C]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__maxop__3B0BC30C]  DEFAULT ((50)) FOR [maxopps]
GO
/****** Object:  Default [DF__Client_Te__requi__4C364F0E]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__requi__4C364F0E]  DEFAULT ((0)) FOR [requireRTSForm]
GO
/****** Object:  Default [DF__Client_Te__requi__4D2A7347]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__requi__4D2A7347]  DEFAULT ((0)) FOR [requireRTSFormWindow]
GO
/****** Object:  Default [DF__Client_Te__requi__4F12BBB9]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__requi__4F12BBB9]  DEFAULT ((1)) FOR [requireRTSformIfExists]
GO
/****** Object:  Default [DF__Client_Te__sessi__41B8C09B]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF__Client_Te__sessi__41B8C09B]  DEFAULT ((-1)) FOR [sessionType]
GO
/****** Object:  Default [DF_Client_TestMode__key]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestMode] ADD  CONSTRAINT [DF_Client_TestMode__key]  DEFAULT (newid()) FOR [_key]
GO
/****** Object:  Default [DF_Client_TestProperties_Prefetch]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF_Client_TestProperties_Prefetch]  DEFAULT ((2)) FOR [Prefetch]
GO
/****** Object:  Default [DF_Client_TestProperties_DateChanged]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF_Client_TestProperties_DateChanged]  DEFAULT (getdate()) FOR [DateChanged]
GO
/****** Object:  Default [DF__Client_Te__IsPri__2057CCD0]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__IsPri__2057CCD0]  DEFAULT ((0)) FOR [IsPrintable]
GO
/****** Object:  Default [DF__Client_Te__IsSel__214BF109]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__IsSel__214BF109]  DEFAULT ((1)) FOR [IsSelectable]
GO
/****** Object:  Default [DF__Client_Te__Print__22401542]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__Print__22401542]  DEFAULT ('') FOR [PrintItemTypes]
GO
/****** Object:  Default [DF__Client_Te__Score__251C81ED]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__Score__251C81ED]  DEFAULT ((1)) FOR [ScoreByTDS]
GO
/****** Object:  Default [DF__Client_Te__Batch__2610A626]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__Batch__2610A626]  DEFAULT ((0)) FOR [BatchModeReport]
GO
/****** Object:  Default [DF__Client_Te__origi__51E506C3]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__origi__51E506C3]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__sourc__52D92AFC]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__sourc__52D92AFC]  DEFAULT (db_name()) FOR [source]
GO
/****** Object:  Default [DF__Client_Te__match__752E4300]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__match__752E4300]  DEFAULT ((1)) FOR [maskItemsBySubject]
GO
/****** Object:  Default [DF__Client_Te__initi__7D8E7ED7]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__initi__7D8E7ED7]  DEFAULT ((1)) FOR [initialAbilityBySubject]
GO
/****** Object:  Default [DF__Client_Te__RTSFo__27F8EE98]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__RTSFo__27F8EE98]  DEFAULT ('TDS-TestForm') FOR [RTSFormField]
GO
/****** Object:  Default [DF__Client_Te__RTSWi__28ED12D1]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__RTSWi__28ED12D1]  DEFAULT ('TDS-TestWindow') FOR [RTSWindowField]
GO
/****** Object:  Default [DF__Client_Te__Windo__2AD55B43]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__Windo__2AD55B43]  DEFAULT ((0)) FOR [WindowTIDESelectable]
GO
/****** Object:  Default [DF__Client_Te__requi__2BC97F7C]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__requi__2BC97F7C]  DEFAULT ((0)) FOR [requireRTSWindow]
GO
/****** Object:  Default [DF__Client_Te__force__36470DEF]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__force__36470DEF]  DEFAULT ((1)) FOR [forceComplete]
GO
/****** Object:  Default [DF__Client_Te__RTSMo__467D75B8]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__RTSMo__467D75B8]  DEFAULT ('TDS-TestMode') FOR [RTSModeField]
GO
/****** Object:  Default [DF__Client_Te__modeT__4A4E069C]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__modeT__4A4E069C]  DEFAULT ((0)) FOR [modeTIDESelectable]
GO
/****** Object:  Default [DF__Client_Te__requi__477199F1]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__requi__477199F1]  DEFAULT ((0)) FOR [requireRTSMode]
GO
/****** Object:  Default [DF__Client_Te__requi__4865BE2A]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__requi__4865BE2A]  DEFAULT ((0)) FOR [requireRTSmodeWindow]
GO
/****** Object:  Default [DF__Client_Te__delet__67DE6983]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__delet__67DE6983]  DEFAULT ((0)) FOR [deleteUnansweredItems]
GO
/****** Object:  Default [DF__Client_Te__abili__4830B400]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__abili__4830B400]  DEFAULT ((1)) FOR [abilitySlope]
GO
/****** Object:  Default [DF__Client_Te__abili__4924D839]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__abili__4924D839]  DEFAULT ((0)) FOR [abilityIntercept]
GO
/****** Object:  Default [DF__Client_Te__valid__52AE4273]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__valid__52AE4273]  DEFAULT ((0)) FOR [validateCompleteness]
GO
/****** Object:  Default [DF__Client_Te__proct__664B26CC]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__proct__664B26CC]  DEFAULT ((0)) FOR [proctorEligibility]
GO
/****** Object:  Default [DF__Client_Te__Categ__150615B5]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestProperties] ADD  CONSTRAINT [DF__Client_Te__Categ__150615B5]  DEFAULT (NULL) FOR [Category]
GO
/****** Object:  Default [DF__Client_Te__Repor__11D4A34F]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestscoreFeatures] ADD  CONSTRAINT [DF__Client_Te__Repor__11D4A34F]  DEFAULT ((0)) FOR [ReportToStudent]
GO
/****** Object:  Default [DF__Client_Te__Repor__12C8C788]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestscoreFeatures] ADD  CONSTRAINT [DF__Client_Te__Repor__12C8C788]  DEFAULT ((0)) FOR [ReportToProctor]
GO
/****** Object:  Default [DF__Client_Te__Repor__13BCEBC1]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestscoreFeatures] ADD  CONSTRAINT [DF__Client_Te__Repor__13BCEBC1]  DEFAULT ((0)) FOR [ReportToParticipation]
GO
/****** Object:  Default [DF__Client_Te__UseFo__14B10FFA]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestscoreFeatures] ADD  CONSTRAINT [DF__Client_Te__UseFo__14B10FFA]  DEFAULT ((0)) FOR [UseForAbility]
GO
/****** Object:  Default [DF_Client_TestTool_SortOrder]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestTool] ADD  CONSTRAINT [DF_Client_TestTool_SortOrder]  DEFAULT ((0)) FOR [SortOrder]
GO
/****** Object:  Default [DF__Client_Te__origi__579DE019]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestTool] ADD  CONSTRAINT [DF__Client_Te__origi__579DE019]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__sourc__58920452]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestTool] ADD  CONSTRAINT [DF__Client_Te__sourc__58920452]  DEFAULT (db_name()) FOR [source]
GO
/****** Object:  Default [DF__Client_Te__TestM__5E1FF51F]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestTool] ADD  CONSTRAINT [DF__Client_Te__TestM__5E1FF51F]  DEFAULT ('ALL') FOR [TestMode]
GO
/****** Object:  Default [DF__Client_Te__Allow__28B808A7]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__Allow__28B808A7]  DEFAULT ((1)) FOR [AllowChange]
GO
/****** Object:  Default [DF__Client_Te__IsReq__29AC2CE0]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__IsReq__29AC2CE0]  DEFAULT ((0)) FOR [IsRequired]
GO
/****** Object:  Default [DF__Client_Te__TIDES__2AA05119]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__TIDES__2AA05119]  DEFAULT ((0)) FOR [TIDESelectableBySubject]
GO
/****** Object:  Default [DF__Client_Te__IsSel__2B947552]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__IsSel__2B947552]  DEFAULT ((1)) FOR [IsSelectable]
GO
/****** Object:  Default [DF__Client_Te__IsVis__2C88998B]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__IsVis__2C88998B]  DEFAULT ((1)) FOR [IsVisible]
GO
/****** Object:  Default [DF__Client_Te__Stude__2D7CBDC4]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__Stude__2D7CBDC4]  DEFAULT ((1)) FOR [StudentControl]
GO
/****** Object:  Default [DF_Client_TestToolType_SortOrder]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF_Client_TestToolType_SortOrder]  DEFAULT ((0)) FOR [SortOrder]
GO
/****** Object:  Default [DF__Client_Te__dateE__39E294A9]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__dateE__39E294A9]  DEFAULT (getdate()) FOR [dateEntered]
GO
/****** Object:  Default [DF__Client_Te__origi__54C1736E]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__origi__54C1736E]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__sourc__55B597A7]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__sourc__55B597A7]  DEFAULT (db_name()) FOR [source]
GO
/****** Object:  Default [DF_Client_TestToolType_DisableOnGuestSession]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF_Client_TestToolType_DisableOnGuestSession]  DEFAULT ((0)) FOR [DisableOnGuestSession]
GO
/****** Object:  Default [DF_Client_TestToolType_IsFunctional]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF_Client_TestToolType_IsFunctional]  DEFAULT ((1)) FOR [IsFunctional]
GO
/****** Object:  Default [DF__Client_Te__TestM__5B438874]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestToolType] ADD  CONSTRAINT [DF__Client_Te__TestM__5B438874]  DEFAULT ('ALL') FOR [TestMode]
GO
/****** Object:  Default [DF_dbo_Client_TestWindow_window]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF_dbo_Client_TestWindow_window]  DEFAULT ((1)) FOR [window]
GO
/****** Object:  Default [DF__Client_Te__origi__2645B050]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF__Client_Te__origi__2645B050]  DEFAULT (db_name()) FOR [origin]
GO
/****** Object:  Default [DF__Client_Te__sourc__2739D489]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF__Client_Te__sourc__2739D489]  DEFAULT (db_name()) FOR [source]
GO
/****** Object:  Default [DF__Client_Tes___Key__382F5661]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF__Client_Tes___Key__382F5661]  DEFAULT (newid()) FOR [_Key]
GO
/****** Object:  Default [DF__Client_Te__sessi__39237A9A]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF__Client_Te__sessi__39237A9A]  DEFAULT ((-1)) FOR [sessionType]
GO
/****** Object:  Default [DF__Client_Te__sortO__75F77EB0]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow] ADD  CONSTRAINT [DF__Client_Te__sortO__75F77EB0]  DEFAULT ((1)) FOR [sortOrder]
GO
/****** Object:  Default [DF_Client_TimeLimits__key]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF_Client_TimeLimits__key]  DEFAULT (newid()) FOR [_key]
GO
/****** Object:  Default [DF_dbo_Client_TimeLimits_IsPracticeTest]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF_dbo_Client_TimeLimits_IsPracticeTest]  DEFAULT ((0)) FOR [IsPracticeTest]
GO
/****** Object:  Default [DF__Client_Ti__DateC__3D2915A8]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF__Client_Ti__DateC__3D2915A8]  DEFAULT (getdate()) FOR [DateChanged]
GO
/****** Object:  Default [DF__Client_Ti__sessi__09A971A2]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF__Client_Ti__sessi__09A971A2]  DEFAULT ((8)) FOR [sessionExpire]
GO
/****** Object:  Default [DF__Client_Ti__reque__0C85DE4D]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF__Client_Ti__reque__0C85DE4D]  DEFAULT ((120)) FOR [requestInterfaceTimeout]
GO
/****** Object:  Default [DF_Client_TimeLimits_RefreshValueMultiplier]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TimeLimits] ADD  CONSTRAINT [DF_Client_TimeLimits_RefreshValueMultiplier]  DEFAULT ((2)) FOR [RefreshValueMultiplier]
GO
/****** Object:  ForeignKey [FK_ClientTool_Tooltype]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestTool]  WITH NOCHECK ADD  CONSTRAINT [FK_ClientTool_Tooltype] FOREIGN KEY([ClientName], [Context], [ContextType], [Type])
REFERENCES [dbo].[Client_TestToolType] ([ClientName], [Context], [ContextType], [ToolName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Client_TestTool] CHECK CONSTRAINT [FK_ClientTool_Tooltype]
GO
/****** Object:  ForeignKey [FK_TimeWindow]    Script Date: 01/16/2015 08:56:18 ******/
ALTER TABLE [dbo].[Client_TestWindow]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeWindow] FOREIGN KEY([clientname], [windowID])
REFERENCES [dbo].[Client_TimeWindow] ([clientname], [windowID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Client_TestWindow] CHECK CONSTRAINT [FK_TimeWindow]
GO
