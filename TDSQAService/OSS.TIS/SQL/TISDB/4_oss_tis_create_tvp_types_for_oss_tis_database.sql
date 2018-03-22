USE OSS_TIS
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestNameLookupTable')
BEGIN
	DROP TYPE dbo.TestNameLookupTable;
END
GO
CREATE TYPE dbo.TestNameLookupTable AS TABLE
(
	InstanceName	varchar(50) NOT NULL,
	TestName		varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestNameLookupTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'CombinationTestMapTable')
BEGIN
	DROP TYPE dbo.CombinationTestMapTable;
END
GO
CREATE TYPE dbo.CombinationTestMapTable AS TABLE
(
	ComponentTestName		varchar(255) NOT NULL,
	ComponentSegmentName	varchar(255) NOT NULL,
	CombinationTestName		varchar(50) NOT NULL,
	CombinationSegmentName	varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.CombinationTestMapTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'CombinationTestFormMapTable')
BEGIN
	DROP TYPE dbo.CombinationTestFormMapTable;
END
GO
CREATE TYPE dbo.CombinationTestFormMapTable AS TABLE
(
	ComponentSegmentName	varchar(255) NOT NULL,
	ComponentFormKey		varchar(100) NOT NULL,
	CombinationFormKey		varchar(100) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.CombinationTestFormMapTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'QcProjectMetadataTable')
BEGIN
	DROP TYPE dbo.QcProjectMetadataTable;
END
GO
CREATE TYPE dbo.QcProjectMetadataTable AS TABLE
(
	ProjectId		int NOT NULL,
	GroupName		varchar(50) NOT NULL,
	VarName			varchar(128) NOT NULL,
	IntValue		bigint,
	FloatValue		money,
	TextValue		varchar(250),
	Comment			varchar(250)
)
GO
GRANT CONTROL ON TYPE::dbo.QcProjectMetadataTable TO public
GO