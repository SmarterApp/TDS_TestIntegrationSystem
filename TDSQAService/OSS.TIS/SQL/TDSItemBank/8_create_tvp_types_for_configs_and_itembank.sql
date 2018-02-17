USE OSS_Configs
GO
IF NOT EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TesteeAttributeType')
BEGIN
	CREATE TYPE dbo.TesteeAttributeType AS TABLE
	(
		RTSName			varchar(50) NOT NULL,
		TDS_ID			varchar(50) NOT NULL,
		ClientName		varchar(100) NOT NULL,
		ReportName		varchar(100),
		[Type]			varchar(50),
		Label			varchar(50),
		AtLogin			varchar(25),
		SortOrder		int
	)
END
GO
GRANT CONTROL ON TYPE::dbo.TesteeAttributeType TO public
GO