USE OSS_Configs
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TesteeAttributeType')
BEGIN
	DROP TYPE dbo.TesteeAttributeType
END
GO
CREATE TYPE dbo.TesteeAttributeType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.TesteeAttributeType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TesteeRelationshipAttributeType')
BEGIN
	DROP TYPE dbo.TesteeRelationshipAttributeType
END
GO
CREATE TYPE dbo.TesteeRelationshipAttributeType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.TesteeRelationshipAttributeType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ClientType')
BEGIN
	DROP TYPE dbo.ClientType
END
GO
CREATE TYPE dbo.ClientType AS TABLE
(
	[Name]				varchar(100) NOT NULL,
	Internationalize	bit NOT NULL,
	DefaultLanguage		varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.ClientType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TimeWindowType')
BEGIN
	DROP TYPE dbo.TimeWindowType
END
GO
CREATE TYPE dbo.TimeWindowType AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	WindowId	bit NOT NULL,
	StartDate	datetime,
	EndDate		datetime
)
GO
GRANT CONTROL ON TYPE::dbo.TimeWindowType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FieldTestPriorityType')
BEGIN
	DROP TYPE dbo.FieldTestPriorityType
END
GO
CREATE TYPE dbo.FieldTestPriorityType AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	TdsId		varchar(50) NOT NULL,
	[Priority]	int NOT NULL,
	TestId		varchar(200) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.FieldTestPriorityType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GradeType')
BEGIN
	DROP TYPE dbo.GradeType
END
GO
CREATE TYPE dbo.GradeType AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	GradeCode	varchar(25) NOT NULL,
	Grade		varchar(64) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.GradeType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SubjectType')
BEGIN
	DROP TYPE dbo.SubjectType
END
GO
CREATE TYPE dbo.SubjectType AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	[Subject]	varchar(100) NOT NULL,
	SubjectCode	varchar(25) NULL
)
GO
GRANT CONTROL ON TYPE::dbo.SubjectType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AccommodationFamilyType')
BEGIN
	DROP TYPE dbo.AccommodationFamilyType
END
GO
CREATE TYPE dbo.AccommodationFamilyType AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	Family		varchar(50) NOT NULL,
	Label		varchar(200) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.AccommodationFamilyType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestPropertiesType')
BEGIN
	DROP TYPE dbo.TestPropertiesType;
END
GO
CREATE TYPE dbo.TestPropertiesType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.TestPropertiesType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestModeType')
BEGIN
	DROP TYPE dbo.TestModeType;
END
GO
CREATE TYPE dbo.TestModeType AS TABLE
(
	ClientName		varchar(100) NOT NULL,
	TestId			varchar(200) NOT NULL,
	TestKey			varchar(250),
	Mode			varchar(50) NOT NULL,
	SessionType		int NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestModeType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SegmentPropertiesType')
BEGIN
	DROP TYPE dbo.SegmentPropertiesType;
END
GO
CREATE TYPE dbo.SegmentPropertiesType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.SegmentPropertiesType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestFormPropertiesType')
BEGIN
	DROP TYPE dbo.TestFormPropertiesType;
END
GO
CREATE TYPE dbo.TestFormPropertiesType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.TestFormPropertiesType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestWindowType')
BEGIN
	DROP TYPE dbo.TestWindowType;
END
GO
CREATE TYPE dbo.TestWindowType AS TABLE
(
	ClientName	varchar(100) NOT NULL, 
	TestId		varchar(200) NOT NULL, 
	WindowId	varchar(200), 
	NumOpps		int NOT NULL, 
	StartDate	datetime, 
	EndDate		datetime
)
GO
GRANT CONTROL ON TYPE::dbo.TestWindowType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestGradeType')
BEGIN
	DROP TYPE dbo.TestGradeType;
END
GO
CREATE TYPE dbo.TestGradeType AS TABLE
(
	ClientName	varchar(100) NOT NULL, 
	TestId		varchar(150) NOT NULL, 
	Grade	varchar(25) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestGradeType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestEligibilityType')
BEGIN
	DROP TYPE dbo.TestEligibilityType;
END
GO
CREATE TYPE dbo.TestEligibilityType AS TABLE
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
GRANT CONTROL ON TYPE::dbo.TestEligibilityType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestEligibilityType')
BEGIN
	DROP TYPE dbo.TestEligibilityType;
END
GO
CREATE TYPE dbo.TestEligibilityType AS TABLE
(
	ClientName		varchar(100) NOT NULL, 
	TestId			varchar(150) NOT NULL, 
	ItemType		varchar(25) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestEligibilityType TO public
GO