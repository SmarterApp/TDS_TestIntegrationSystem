USE OSS_Configs
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TesteeAttributeTable')
BEGIN
	DROP TYPE dbo.TesteeAttributeTable
END
GO
CREATE TYPE dbo.TesteeAttributeTable AS TABLE
(
	RtsName			varchar(50) NOT NULL,
	TdsId			varchar(50) NOT NULL,
	ClientName		varchar(100) NOT NULL,
	ReportName		varchar(100),
	[Type]			varchar(50),
	Label			varchar(50),
	AtLogin			varchar(25),
	SortOrder		int
)
GO
GRANT CONTROL ON TYPE::dbo.TesteeAttributeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TesteeRelationshipAttributeTable')
BEGIN
	DROP TYPE dbo.TesteeRelationshipAttributeTable
END
GO
CREATE TYPE dbo.TesteeRelationshipAttributeTable AS TABLE
(
	ClientName			varchar(50) NOT NULL,
	TdsId				varchar(50) NOT NULL,
	RtsName				varchar(50) NOT NULL,
	Label				varchar(50),
	ReportName			varchar(50),
	AtLogin				varchar(25),		
	SortOrder			int,
	RelationshipType	varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TesteeRelationshipAttributeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ClientTable')
BEGIN
	DROP TYPE dbo.ClientTable
END
GO
CREATE TYPE dbo.ClientTable AS TABLE
(
	[Name]				varchar(100) NOT NULL,
	Internationalize	bit NOT NULL,
	DefaultLanguage		varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.ClientTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TimeWindowTable')
BEGIN
	DROP TYPE dbo.TimeWindowTable
END
GO
CREATE TYPE dbo.TimeWindowTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	WindowId	bit NOT NULL,
	StartDate	datetime,
	EndDate		datetime
)
GO
GRANT CONTROL ON TYPE::dbo.TimeWindowTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FieldTestPriorityTable')
BEGIN
	DROP TYPE dbo.FieldTestPriorityTable
END
GO
CREATE TYPE dbo.FieldTestPriorityTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	TdsId		varchar(50) NOT NULL,
	[Priority]	int NOT NULL,
	TestId		varchar(200) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.FieldTestPriorityTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GradeTable')
BEGIN
	DROP TYPE dbo.GradeTable
END
GO
CREATE TYPE dbo.GradeTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	GradeCode	varchar(25) NOT NULL,
	Grade		varchar(64) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.GradeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SubjectTable')
BEGIN
	DROP TYPE dbo.SubjectTable
END
GO
CREATE TYPE dbo.SubjectTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	[Subject]	varchar(100) NOT NULL,
	SubjectCode	varchar(25) NULL
)
GO
GRANT CONTROL ON TYPE::dbo.SubjectTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AccommodationFamilyTable')
BEGIN
	DROP TYPE dbo.AccommodationFamilyTable
END
GO
CREATE TYPE dbo.AccommodationFamilyTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	Family		varchar(50) NOT NULL,
	Label		varchar(200) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.AccommodationFamilyTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestPropertiesTable')
BEGIN
	DROP TYPE dbo.TestPropertiesTable;
END
GO
CREATE TYPE dbo.TestPropertiesTable AS TABLE
(
	ClientName			varchar(100) NOT NULL,
	TestId				varchar(255) NOT NULL,
	IsSelectable		bit NOT NULL,
	Label				varchar(255) NOT NULL,
	SubjectName			varchar(100),
	MaxOpportunities	int,
	ScoreByTds			bit NOT NULL,
	AccommodationFamily	varchar(50),
	ReportingInstrument	varchar(50),
	TideId				varchar(100),
	GradeText			varchar(50)
)
GO
GRANT CONTROL ON TYPE::dbo.TestPropertiesTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestModeTable')
BEGIN
	DROP TYPE dbo.TestModeTable;
END
GO
CREATE TYPE dbo.TestModeTable AS TABLE
(
	ClientName		varchar(100) NOT NULL,
	TestId			varchar(200) NOT NULL,
	TestKey			varchar(250),
	Mode			varchar(50) NOT NULL,
	SessionType		int NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestModeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SegmentPropertiesTable')
BEGIN
	DROP TYPE dbo.SegmentPropertiesTable;
END
GO
CREATE TYPE dbo.SegmentPropertiesTable AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	SegmentId		varchar(255) NOT NULL, 
	SegmentPosition	int NOT NULL, 
	ParentTest		varchar(255) NOT NULL, 
	IsPermeable		int NOT NULL, 
	EntryApproval	int NOT NULL, 
	ExitApproval	int NOT NULL, 
	ItemReview		bit NOT NULL, 
	Label			varchar(255), 
	ModeKey			varchar(250)
)
GO
GRANT CONTROL ON TYPE::dbo.SegmentPropertiesTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestFormPropertiesTable')
BEGIN
	DROP TYPE dbo.TestFormPropertiesTable;
END
GO
CREATE TYPE dbo.TestFormPropertiesTable AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	TestFormKey		varchar(50) NOT NULL, 
	FormId			varchar(200), 
	TestId			varchar(150) NOT NULL, 
	[Language]		varchar(25) NOT NULL, 
	StartDate		datetime, 
	EndDate			datetime, 
	TestKey			varchar(250)
)
GO
GRANT CONTROL ON TYPE::dbo.TestFormPropertiesTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestWindowTable')
BEGIN
	DROP TYPE dbo.TestWindowTable;
END
GO
CREATE TYPE dbo.TestWindowTable AS TABLE
(
	ClientName	varchar(100) NOT NULL, 
	TestId		varchar(200) NOT NULL, 
	WindowId	varchar(200), 
	NumOpps		int NOT NULL, 
	StartDate	datetime, 
	EndDate		datetime
)
GO
GRANT CONTROL ON TYPE::dbo.TestWindowTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestGradeTable')
BEGIN
	DROP TYPE dbo.TestGradeTable;
END
GO
CREATE TYPE dbo.TestGradeTable AS TABLE
(
	ClientName	varchar(100) NOT NULL, 
	TestId		varchar(150) NOT NULL, 
	Grade	varchar(25) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestGradeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestEligibilityTable')
BEGIN
	DROP TYPE dbo.TestEligibilityTable;
END
GO
CREATE TYPE dbo.TestEligibilityTable AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	TestId			varchar(150) NOT NULL, 
	RtsName			varchar(100) NOT NULL,
	Enables			bit NOT NULL,
	Disables		bit NOT NULL,
	RtsValue		varchar(400) NOT NULL,
	EntityType		bigint NOT NULL,
	EligibilityType	varchar(50),
	MatchType		int NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestEligibilityTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestItemTypeTable')
BEGIN
	DROP TYPE dbo.TestItemTypeTable;
END
GO
CREATE TYPE dbo.TestItemTypeTable AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	TestId			varchar(150) NOT NULL, 
	ItemType		varchar(25) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestItemTypeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestItemConstraintTable')
BEGIN
	DROP TYPE dbo.TestItemConstraintTable;
END
GO
CREATE TYPE dbo.TestItemConstraintTable AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	TestId			varchar(255) NOT NULL, 
	PropName		varchar(100) NOT NULL,
	PropValue		varchar(100) NOT NULL,
	ToolType		nvarchar(255) NOT NULL,
	ToolValue		nvarchar(255) NOT NULL,
	ItemIn			bit NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestItemConstraintTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestToolTypeTable')
BEGIN
	DROP TYPE dbo.TestToolTypeTable;
END
GO
CREATE TYPE dbo.TestToolTypeTable AS TABLE
(
	ClientName				varchar(100) NOT NULL, 
	Context					varchar(255) NOT NULL,
	ContextType				varchar(50) NOT NULL,
	ToolName				varchar(255) NOT NULL,
	AllowChange				bit NOT NULL,
	IsSelectable			bit NOT NULL,
	IsVisible				bit NOT NULL,
	StudentControl			bit NOT NULL,
	IsFunctional			bit NOT NULL,
	RtsFieldName			nvarchar(100),
	IsRequired				bit NOT NULL,
	TideSelectable			bit,
	TideSelectableBySubject	bit NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestToolTypeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestToolTable')
BEGIN
	DROP TYPE dbo.TestToolTable;
END
GO
CREATE TYPE dbo.TestToolTable AS TABLE
(
	ClientName			varchar(100) NOT NULL, 
	[Type]				nvarchar(255) NOT NULL,
	Code				nvarchar(255) NOT NULL,
	[Value]				varchar(128) NOT NULL,
	IsDefault			bit NOT NULL,
	AllowCombine		bit NOT NULL,
	ValueDescription	nvarchar(255),
	Context				varchar(255) NOT NULL,
	ContextType			varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestToolTable TO public
GO