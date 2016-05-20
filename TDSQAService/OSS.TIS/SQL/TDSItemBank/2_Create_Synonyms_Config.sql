
select 'TODO: needs input' -- comment out after setting the values below
return;

declare @TDSConfigsDatabaseName varchar(255)
declare @TestScoringConfigsDatabaseName varchar(255)

-- TODO: set these to the database names you created
set @TDSConfigsDatabaseName = 'TODO:'
set @TestScoringConfigsDatabaseName = 'TODO:'

---Create TDSConfigs and TestScoringConfigs Synonyms
---NOTE: Make sure TDSConfigs and TestScoringConfgis databases are set up pripr to creating the Synonyms
--Synonyms for TDS Configs
Exec dbo.Create_Synonyms @TDSConfigsDatabaseName, 0, 1
--Synonyms for TestScoringConfigs
Exec dbo.Create_Synonyms @TestScoringConfigsDatabaseName, 0, 0, 1
