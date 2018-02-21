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
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SetOfAdminItemTable')
BEGIN
	DROP TYPE dbo.SetOfAdminItemTable
END
GO
CREATE TYPE dbo.SetOfAdminItemTable AS TABLE
(
	ItemKey				varchar(150) NOT NULL,
	SegmentKey			varchar(250) NOT NULL,
	TestVersion			bigint,
	StrandKey			varchar(150),
	TestAdminKey		varchar(150),
	GroupId				varchar(100),
	ItemPosition		int,
	IsFieldTest			int,
	IsActive			bit,
	BlockId				varchar(10),
	IsRequired			bit,
	GroupKey			varchar(100),
	StrandName			varchar(150),
	IrtA				float,
	IrtB				varchar(150),
	IrtC				float,
	IrtModel			varchar(100),
	ClsString			varchar(max),
	UpdatedTestVersion	bigint,
	BVector				varchar(200)
)
GO
GRANT CONTROL ON TYPE::dbo.SetOfAdminItemTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ItemScoreDimensionType')
BEGIN
	DROP TYPE dbo.ItemScoreDimensionType
END
GO
CREATE TYPE dbo.ItemScoreDimensionType AS TABLE
(
	ItemScoreDimensionKey	uniqueidentifier NOT NULL,
	ItemKey					varchar(150) NOT NULL,
	SegmentKey				varchar(250) NOT NULL,
	Dimension				varchar(255) NOT NULL,
	ScorePoints				int NOT NULL,
	[Weight]				float NOT NULL,
	MeasurementModel		int NOT NULL,
	RecodeRule				nvarchar(255)
)
GO
GRANT CONTROL ON TYPE::dbo.ItemScoreDimensionType TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ItemMeasurementParameterTable')
BEGIN
	DROP TYPE dbo.ItemMeasurementParameterTable
END
GO
CREATE TYPE dbo.ItemMeasurementParameterTable AS TABLE
(
	ItemScoreDimensionKey	uniqueidentifier NOT NULL,
	MeasurementParameterKey	int NOT NULL,
	ParmValue				float
)
GO
GRANT CONTROL ON TYPE::dbo.ItemMeasurementParameterTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AdminStimulusTable')
BEGIN
	DROP TYPE dbo.AdminStimulusTable
END
GO
CREATE TYPE dbo.AdminStimulusTable AS TABLE
(
	StimulusKey			varchar(100) NOT NULL,
	SegmentKey			varchar(250) NOT NULL,
	NumItemsRequired	int NOT NULL,
	MaxItems			int NOT NULL,
	TestVersion			bigint,
	GroupId				varchar(50),
	UpdatedTestVersion	bigint
)
GO
GRANT CONTROL ON TYPE::dbo.AdminStimulusTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestFormTable')
BEGIN
	DROP TYPE dbo.TestFormTable
END
GO
CREATE TYPE dbo.TestFormTable AS TABLE
(
	SegmentKey		varchar(250) NOT NULL,
	Cohort			varchar(20) NOT NULL,
	[Language]		varchar(150),
	TestFormKey		varchar(100) NOT NULL,
	FormId			varchar(150),
	ITSBankKey		bigint NOT NULL,
	ITSKey			bigint NOT NULL,
	TestVersion		bigint
)
GO
GRANT CONTROL ON TYPE::dbo.TestFormTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestFormItemTable')
BEGIN
	DROP TYPE dbo.TestFormItemTable
END
GO
CREATE TYPE dbo.TestFormItemTable AS TABLE
(
	ItemKey			varchar(150) NOT NULL,
	ITSFormKey		bigint NOT NULL,
	FormPosition	int NOT NULL,
	SegmentKey		varchar(250) NOT NULL,
	TestFormKey		varchar(100),
	IsActive		bit NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestFormItemTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AffinityGroupTable')
BEGIN
	DROP TYPE dbo.AffinityGroupTable
END
GO
CREATE TYPE dbo.AffinityGroupTable AS TABLE
(
	SegmentKey					varchar(250) NOT NULL,
	GroupId						varchar(100) NOT NULL,
	MinItems					int NOT NULL,
	MaxItems					int NOT NULL,
	[Weight]					float NOT NULL,
	IsStrictMax					bit NOT NULL,
	TestVersion					bigint,
	UpdatedTestVersion			bigint,
	AbilityWeight				float NOT NULL,
	PrecisionTarget				float,
	StartAbility				float,
	StartInfo					float,
	PrecisionTargetMetWeight	float,
	PrecisionTargetNotMetWeight	float
)
GO
GRANT CONTROL ON TYPE::dbo.AffinityGroupTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'AffinityGroupItemTable')
BEGIN
	DROP TYPE dbo.AffinityGroupItemTable
END
GO
CREATE TYPE dbo.AffinityGroupItemTable AS TABLE
(
	SegmentKey	varchar(250) NOT NULL,
	GroupId		varchar(100) NOT NULL,
	ItemKey		varchar(100) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.AffinityGroupItemTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'PerformanceLevelTable')
BEGIN
	DROP TYPE dbo.PerformanceLevelTable
END
GO
CREATE TYPE dbo.PerformanceLevelTable AS TABLE
(
	ContentKey	varchar(250) NOT NULL,
	PLevel		int NOT NULL,
	ThetaLo		float NOT NULL,
	ThetaHi		float NOT NULL,
	ScaledLo	float,
	ScaledHi	float
)
GO
GRANT CONTROL ON TYPE::dbo.PerformanceLevelTable TO public
GO