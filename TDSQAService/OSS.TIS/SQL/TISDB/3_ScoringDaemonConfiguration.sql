select 'TODO: update local variables'
return; -- comment this out when values have been updated

--TODO: your db and server and SD name...
declare @IP varchar(30)
declare @privateIP varchar(30)
declare @dbname varchar(255)
declare @scoringDaemonName varchar(50)

-- these can be IP or server name
set @IP = 'my.tisdb.org' -- Database Host Name or IP
set @privateIP = NULL -- can be different if you want to use a private IP 
set @dbname = 'OSS_TIS'
set @scoringDaemonName = 'MyTISScoringDaemon'

--TODO: your hosts
--NOTE: just setting up one for the sake of demonstration, and for all item types.
declare @itemScoringHost varchar(256)
declare @rubricCallbackHost varchar(400)

set @itemScoringHost = 'myItemScoringServer.org/itemscoring/Scoring'
set @rubricCallbackHost = 'https://myRubricServer.org/student/'


/****** Object:  Table [dbo].[ScoringDaemonMonitoredSites]    Script Date: 01/15/2015 15:05:20 ******/
INSERT [dbo].[ScoringDaemonMonitoredSites] ([_efk_ClientName], [environment], [IP], [privateIP], [dbname], [ScoringDaemonName], [serviceType], [serviceRole]) VALUES (N'SBAC', N'Dev', @IP, @privateIP, @dbname, @scoringDaemonName, NULL, NULL)

INSERT INTO [dbo].[SDItemScorer]
           ([_efk_ClientName]
           ,[scoreHost]
           ,[rubricCallbackUrl])
     VALUES
           ('SBAC'
           ,@itemScoringHost
           ,@rubricCallbackHost)

INSERT INTO [dbo].[SDItemScoringConfigs]
           ([_efk_ClientName]
           ,[Context]
           ,[ItemType]
           ,[Enabled]
           ,[Priority]
           ,[ServerUrl])
     VALUES
           ('SBAC'
           ,null
           ,'*'
           ,1
           ,1
           ,'http://{host}/ItemScoring.axd')



