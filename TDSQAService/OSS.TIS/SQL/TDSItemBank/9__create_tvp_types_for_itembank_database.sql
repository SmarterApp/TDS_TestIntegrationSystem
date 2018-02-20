USE OSS_Itembank
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SubjectTable')
BEGIN
	DROP TYPE dbo.SubjectTable
END
GO
CREATE TYPE dbo.SubjectTable AS TABLE
(
	[Name]			varchar(100) NOT NULL,
	Grade			varchar(64) NOT NULL,
	SubjectKey		varchar(150) NOT NULL,
	ClientKey		bigint,
	TestVersion		bigint
)
GO
GRANT CONTROL ON TYPE::dbo.SubjectTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'StrandTable')
BEGIN
	DROP TYPE dbo.StrandTable
END
GO
CREATE TYPE dbo.StrandTable AS TABLE
(
	SubjectKey			varchar(150) NOT NULL,
	[Name]				varchar(150) NOT NULL,
	ParentId			varchar(150) NOT NULL,
	BlueprintElementId	varchar(150) NOT NULL,
	ClientKey			bigint,
	TreeLevel			int,
	TestVersion			bigint
)
GO
GRANT CONTROL ON TYPE::dbo.StrandTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'StimulusTable')
BEGIN
	DROP TYPE dbo.StimulusTable
END
GO
CREATE TYPE dbo.StimulusTable AS TABLE
(
	ItemBankKey			bigint NOT NULL,
	ItsKey				bigint NOT NULL,
	ClientId			varchar(100),
	FilePath			varchar(50),
	[FileNme]			varchar(50),
	DateLastUpdated		datetime,
	PassageKey			varchar(150),
	TestVersion			bigint
)
GO
GRANT CONTROL ON TYPE::dbo.StimulusTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ItemTable')
BEGIN
	DROP TYPE dbo.ItemTable
END
GO
CREATE TYPE dbo.ItemTable AS TABLE
(
	ItemBankKey			bigint NOT NULL,
	ItsKey				bigint NOT NULL,
	ItemType			varchar(50),
	FilePath			varchar(50),
	[FileNme]			varchar(50),
	DateLastUpdated		datetime,
	ItemId				varchar(80),
	[Key]				varchar(150) NOT NULL,
	TestVersion			bigint
)
GO
GRANT CONTROL ON TYPE::dbo.ItemTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AaItemClTable')
BEGIN
	DROP TYPE dbo.AaItemClTable
END
GO
CREATE TYPE dbo.AaItemClTable AS TABLE
(
	SegmentKey			varchar(250) NOT NULL,
	ItemKey				varchar(25) NOT NULL,
	ContentLevel		varchar(100)
)
GO
GRANT CONTROL ON TYPE::dbo.AaItemClTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SetOfItemStrandsTable')
BEGIN
	DROP TYPE dbo.SetOfItemStrandsTable
END
GO
CREATE TYPE dbo.SetOfItemStrandsTable AS TABLE
(
	ItemKey			varchar(150) NOT NULL,
	StrandKey		varchar(150) NOT NULL,
	SegmentKey		varchar(250) NOT NULL,
	TestVersion		bigint
)
GO
GRANT CONTROL ON TYPE::dbo.SetOfItemStrandsTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SeOfItemStimuliTable')
BEGIN
	DROP TYPE dbo.SeOfItemStimuliTable
END
GO
CREATE TYPE dbo.SeOfItemStimuliTable AS TABLE
(
	ItemKey			varchar(150) NOT NULL,
	StimulusKey		varchar(150) NOT NULL,
	SegmentKey		varchar(250) NOT NULL,
	TestVersion		bigint
)
GO
GRANT CONTROL ON TYPE::dbo.SeOfItemStimuliTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ItemPropertyTable')
BEGIN
	DROP TYPE dbo.ItemPropertyTable
END
GO
CREATE TYPE dbo.ItemPropertyTable AS TABLE
(
	ItemKey			varchar(150) NOT NULL,
	PropertyName	varchar(50) NOT NULL,
	PropertyValue	varchar(128) NOT NULL,
	SegmentKey		varchar(250) NOT NULL,
	IsActive		bit
)
GO
GRANT CONTROL ON TYPE::dbo.ItemPropertyTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestAdminTable')
BEGIN
	DROP TYPE dbo.TestAdminTable
END
GO
CREATE TYPE dbo.TestAdminTable AS TABLE
(
	AdminKey		varchar(150) NOT NULL,
	SchoolYear		varchar(25) NOT NULL,
	Season			varchar(10),
	ClientKey		bigint,
	[Description]	varchar(255),
	TestVersion		bigint
)
GO
GRANT CONTROL ON TYPE::dbo.TestAdminTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SetOfAdminSubjectTable')
BEGIN
	DROP TYPE dbo.SetOfAdminSubjectTable
END
GO
CREATE TYPE dbo.SetOfAdminSubjectTable AS TABLE
(
	SegmentKey					varchar(250) NOT NULL,
	TestAdminKey				varchar(150) NOT NULL,
	SubjectKey					varchar(150),
	TestId						varchar(255),
	StartAbility				float,
	StartInfo					float,
	MinItems					int,
	MaxItems					int,
	Slope						float, 
	Intercept					float,
	FieldTestStartPosition		int,
	FieldTestEndPosition		int,
	FieldTestMinItems			int,
	FieldTestMaxItems			int,
	SelectionAlgorithm			varchar(50),
	BlueprintWeight				float,
	CSet1Size					int,
	CSet2Random					int,
	CSet2InitialRandom			int,
	VirtualTest					varchar(200),
	TestPosition				int,
	IsSegmented					bit,
	ItemWeight					float,
	AbilityOffset				float,
	CSet1Order					varchar(50) NOT NULL,
	TestVersion					bigint,
	[Contract]					varchar(100),
	TestType					varchar(60),
	PrecisionTarget				float,
	AdaptiveCut					float,
	TooCloseSEs					float,
	AbilityWeight				float,
	ComputeAbilityWeights		bit,
	RcAbilityWeight				float,
	PrecisionTargetWeightMet	float,
	PrecisionTargetWeightNotMet	float,
	TerminationOverallInfo		bit,
	TerminationRcInfo			bit,
	TerminationMinCount			bit,
	TerminationTooClose			bit,
	TerminationFlagsAnd			bit,
	BlueprintMetricFunction		varchar(25) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.SetOfAdminSubjectTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SetOfTestGradeTable')
BEGIN
	DROP TYPE dbo.SetOfTestGradeTable
END
GO
CREATE TYPE dbo.SetOfTestGradeTable AS TABLE
(
	TestId				varchar(100) NOT NULL,
	Grade				varchar(25) NOT NULL,
	RequireEnrollment	bit NOT NULL,
	SegmentKey			varchar(250) NOT NULL,
	EnrolledSubject		varchar(100)
)
GO
GRANT CONTROL ON TYPE::dbo.SetOfTestGradeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ItemSelectionParmTable')
BEGIN
	DROP TYPE dbo.ItemSelectionParmTable
END
GO
CREATE TYPE dbo.ItemSelectionParmTable AS TABLE
(
	SegmentKey				varchar(250) NOT NULL,
	BlueprintElementId		varchar(200),
	PropertyName			varchar(100) NOT NULL,
	PropertyValue			varchar(200) NOT NULL,
	PropertyLabel			varchar(200)
)
GO
GRANT CONTROL ON TYPE::dbo.ItemSelectionParmTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestCohortTable')
BEGIN
	DROP TYPE dbo.TestCohortTable
END
GO
CREATE TYPE dbo.TestCohortTable AS TABLE
(
	SegmentKey	varchar(250) NOT NULL,
	Cohort		int NOT NULL,
	ItemRatio	float NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestCohortTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AdminStrandTable')
BEGIN
	DROP TYPE dbo.AdminStrandTable
END
GO
CREATE TYPE dbo.AdminStrandTable AS TABLE
(
	AdminStrandKey				nvarchar(255) NOT NULL,
	SegmentKey					varchar(250) NOT NULL,
	StrandKey					varchar(150) NOT NULL,
	MinItems					int,
	MaxItems					int,
	IsStrictMax					bit NOT NULL,
	BlueprintWeight				float NOT NULL,
	AdaptiveCut					float,
	StartAbility				float,
	StartInfo					float,
	Scalar						float,
	TestVersion					bigint,
	LoadMin						int,
	LoadMax						int,
	PrecisionTarget				float,
	PrecisionTargetMetWeight	float,
	PrecisionTargetNotMetWeight	float,
	AbilityWeight				float
)
GO
GRANT CONTROL ON TYPE::dbo.AdminStrandTable TO public
GO